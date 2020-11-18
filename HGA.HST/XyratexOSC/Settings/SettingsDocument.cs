using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.Settings
{
    /// <summary>
    /// Represents a settings-file document. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides an in-memory (cache) tree representation of tab-delimited settings files.
    /// A SettingsDocument tree can be created in-code, or loaded from a file.
    /// </para>
    /// <para>
    /// A SettingsDocument contains a keyed and indexed collection of nodes (<see cref="SettingsNode"/>)
    /// that each have <see cref="SettingsNodeList"/> collections of child nodes. A SettingsDocument is
    /// actually a special top-level <see cref="SettingsNode"/> itself. 
    /// </para>
    /// </remarks>
    public class SettingsDocument : SettingsNode
    {
        #region Constants

        private readonly int _streamBufferSize = 0x1000; //this is the default .NET FileStream buffer size
        private readonly byte[] _encryptionKey = new ASCIIEncoding().GetBytes("XYRATEXYRATEXYRATEXYRATEXYRATEX0");
        private readonly byte[] _encryptionIV = new byte[16];

        #endregion

        #region Fields

        private char _pathSeparator = '|';
        private bool _persistToFile = false;
        private List<string> _trailingComments;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the filename of the most recently loaded settings file. Returns an empty string if no file loaded.
        /// </summary>
        /// <value>The loaded file name.</value>
        public string LoadedFile
        {
            get
            {
                if (Name != "Document")
                    return Name;
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets or sets the file options.
        /// </summary>
        /// <value>
        /// The file options.
        /// </value>
        public SettingsFileOption FileOption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path separator charater used for path-specified grabbing of nodes. The default is '|'.
        /// </summary>
        /// <value>The path separator.</value>
        public override char PathSeparator
        {
            get { return _pathSeparator; }
            set { _pathSeparator = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new, empty instance of the <see cref="SettingsDocument"/> class.
        /// </summary>
        public SettingsDocument() : base("Document")
        { 
            _trailingComments = new List<string>();
            _readOnly = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDocument"/> class and loads the specified file.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        public SettingsDocument(string filename) : base(filename)
        {
            _trailingComments = new List<string>();

            if (!File.Exists(filename))
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }

            Load(filename);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDocument"/> class and loads the specified file.
        /// If persistToFile is true, then every insertion/removal/change of a <see cref="SettingsNode"/> will 
        /// be written back to the loaded settings file.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <param name="persistToFile">if set to <c>true</c> save this SettingsDocument to file on every manipulation.</param>
        public SettingsDocument(string filename, bool persistToFile) : this(filename) 
        {
            _persistToFile = persistToFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDocument"/> class and loads the specified file.
        /// The provided <see cref="PathSeparator"/> will be the node delimiter used when traversing the document by paths.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <param name="pathSeparator">The path separator character.</param>
        public SettingsDocument(string filename, char pathSeparator) : this(filename) 
        {
            _pathSeparator = pathSeparator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDocument"/> class and loads the specified file.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <param name="pathSeparator">The path separator charater.</param>
        /// <param name="persistToFile">if set to <c>true</c> save this SettingsDocument to file on every manipulation.</param>
        public SettingsDocument(string filename, char pathSeparator, bool persistToFile) : this(filename, pathSeparator)
        {
            _persistToFile = persistToFile;
        }

        /// <summary>
        /// The document's child nodes should be at level 0. Overriden from <see cref="SettingsNode.GetLevel"/> to always return -1.
        /// </summary>
        /// <returns>-1, so that the child nodes of a document are level 0.</returns>
        protected override int GetLevel()
        {
            return -1;
        }

        /// <summary>
        /// Loads the specified settings file into this settings document by creating the settings node tree based on the file's
        /// tab structure. Any line that contains only whitespace or only a comment will not become a <see cref="SettingsNode"/>
        /// but will be retained when the file is saved.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="FileNotFoundException">The file specified in <c>filename</c> was not found.</exception>
        private void LoadFromStream(StreamReader stream)
        {
            List<string> lineComments = new List<string>();

            Nodes.Clear();
            SettingsNode prevNode = this;

            string line;
            while ((line = stream.ReadLine()) != null)
            {
                //Check the line for a valid node name
                if (String.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith(";"))
                {
                    lineComments.Add(line);
                    continue;
                }

                // Find the node's level
                int level = 0;
                while (line.ElementAt(level) == '\t')
                    level++;

                // Create a new settings node
                SettingsNode currentNode = CreateNode(line);

                // Attach preceding file comments to the current node
                if (lineComments.Count > 0)
                {
                    for (int i = 0; i < lineComments.Count; i++)
                        currentNode.AddPrecedingComment(lineComments[i]);

                    lineComments.Clear();
                }

                // Add the current node at the proper level
                if (level == 0)
                    _nodes.Add(currentNode);
                else if (level == prevNode.Level)
                    prevNode.Parent.Nodes.Add(currentNode);
                else if (level > prevNode.Level)
                    prevNode.Nodes.Add(currentNode);
                else // level < prevNode.Level
                {
                    while (level <= prevNode.Level)
                        prevNode = prevNode.Parent;

                    prevNode.Nodes.Add(currentNode);
                }

                // Update the previous node
                prevNode = currentNode;
            }

            if (lineComments.Count > 0)
                _trailingComments = lineComments;
        }

        /// <summary>
        /// Loads the specified settings file into this settings document by creating the settings node tree based on the file's
        /// tab structure. Any line that contains only whitespace or only a comment will not become a <see cref="SettingsNode"/>
        /// but will be retained when the file is saved.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="fileOption">The file option that was used when the file was saved.</param>
        /// <exception cref="FileNotFoundException">The file specified in <c>filename</c> was not found.</exception>
        public void Load(string filename, SettingsFileOption fileOption = SettingsFileOption.Default)
        {
            if (!File.Exists(filename))
            {
                File.Create(filename).Dispose();
            }

            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                if (fileOption == SettingsFileOption.Default)
                {
                    string extension = Path.GetExtension(filename);
                    if (String.Equals(extension, ".stpe", StringComparison.InvariantCultureIgnoreCase))
                        fileOption = SettingsFileOption.Encrypted;
                    else if (String.Equals(extension, ".stpc", StringComparison.InvariantCultureIgnoreCase))
                        fileOption = SettingsFileOption.Compressed;
                }

                Stream readable = null;
                ICryptoTransform decrypter = null;
                try
                {
                    switch (fileOption)
                    {
                        case SettingsFileOption.Encrypted:
                            Aes aes = AesManaged.Create();
                            aes.BlockSize = 128;
                            aes.KeySize = 256;
                            aes.IV = _encryptionIV;
                            aes.Padding = PaddingMode.PKCS7;
                            aes.Mode = CipherMode.CBC;
                            aes.Key = _encryptionKey;

                            decrypter = aes.CreateDecryptor();
                            readable = new CryptoStream(file, decrypter, CryptoStreamMode.Read);
                            break;
                        case SettingsFileOption.Compressed:
                            readable = new GZipStream(file, CompressionMode.Decompress);
                            break;
                        default:
                            readable = file;
                            break;
                    }

                    try
                    {
                        Logging.Log.Info("Settings", "Opening file '{0}'", filename);

                        LoadFromStream(new StreamReader(readable));

                        Logging.Log.Info("Settings", "Loaded file '{0}'", filename);
                    }
                    catch (Exception ex)
                    {
                        if (readable != file)
                        {
                            Logging.Log.Error("Settings", ex);
                            LoadFromStream(new StreamReader(file));
                        }
                        else
                            throw;
                    }
                    
                    FileOption = fileOption;
                    Name = filename;
                }
                finally
                {
                    if (readable != null)
                        readable.Dispose();

                    if (decrypter != null)
                        decrypter.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates a settings node from a line of the settings file.
        /// </summary>
        /// <param name="fileLine">The file line.</param>
        /// <returns>The newly created <see cref="SettingsNode"/>.</returns>
        /// <exception cref="ArgumentException">The specified <c>fileLine</c> is a zero-length string, or contains only white space.</exception>
        private SettingsNode CreateNode(string fileLine)
        {
            string name = "";
            string nodeInfo = "";
            string nodeFormatting = "{0}";
            Type nodeType = null;
            bool nodeReadOnly = false;
            int listLength = -1;

            if (String.IsNullOrWhiteSpace(fileLine))
                throw new ArgumentException("Cannot create settings node from an empty line.", fileLine);

            fileLine = fileLine.Trim();

            int commentBlockStart = fileLine.IndexOf(';');
            string nameBlock = fileLine;
            string commentBlock = "";

            if (commentBlockStart > 0)
            {
                nameBlock = fileLine.Substring(0, commentBlockStart);
                commentBlock = fileLine.Substring(commentBlockStart + 1);
            }

            //Parse name block
            if (nameBlock.Contains("(empty)"))
            {
                return new SettingsNode("");
            }
            else if (nameBlock.Contains('['))
            {
                string[] chunks = nameBlock.Split('[',']');

                listLength = Int32.Parse(chunks[chunks.Length - 2]);
                name = chunks[0].Trim();
            }
            else
            {
                name = nameBlock.Trim();
            }

            if (!String.IsNullOrEmpty(commentBlock))
            {
                //Parse comment block
                nodeFormatting = "{0}" + nameBlock.TrimStart().Substring(name.Length) + ";";

                string[] comments = commentBlock.Split('[', ']');

                foreach (string comment in comments)
                {
                    if (comment.Contains("*") && !nodeReadOnly)
                    {
                        nodeReadOnly = true;
                        nodeFormatting += "{2}";
                    }

                    string[] chunks = comment.Split(':');

                    if (chunks.Length == 1)
                    {
                        if (String.IsNullOrEmpty(nodeInfo))
                            nodeFormatting += "{1}";

                        nodeInfo += comment + " ";
                    }
                    else if (String.Equals(chunks[0], "Type") && nodeType == null)
                    {
                        nodeType = Type.GetType(chunks[1]);
                        nodeFormatting += "{3}";
                    }
                }
            }

            SettingsNode node = new SettingsNode(name, nodeInfo, nodeType, nodeReadOnly);
            node.Formatting = nodeFormatting;

            if (listLength > -1)
                node.ListLength = listLength;

            return node;
        }

        private string NodeToFileLine(SettingsNode node)
        {
            string tabs = "";
            string readOnly = "";
            string type = "";
            string formattedName = node.Name;

            for (int i = 0; i < node.Level; i++)
                tabs += '\t';

            if (String.IsNullOrEmpty(node.Name))
                formattedName = EmptyNodeName;

            if (node.IsList)
                formattedName += String.Format(" [{0}]", node.ListLength);

            if (node.IsAValue || node.HasAValue)
            {
                if (node.ReadOnly)
                    readOnly = "[*]";

                if (node.Type != null && node.Type != typeof(System.String))
                    type = "[Type:" + node.Type.ToString() + "]";
            }

            if (String.IsNullOrEmpty(node.Formatting))
            {
                node.Formatting = "{0}";

                if (node.Info != "" || readOnly != "" || type != "")
                    node.Formatting += "                ;{1}{2}{3}";
            }

            return tabs + String.Format(node.Formatting, formattedName, node.Info, readOnly, type);
        }

        /// <summary>
        /// Saves this document to the last loaded file.
        /// </summary>
        public void Save()
        {
            Save(LoadedFile, FileOption);
        }

        /// <summary>
        /// Saves this document to the specified file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            Save(filename, FileOption);
        }

        /// <summary>
        /// Saves this document to the specified file with the specified file options.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="fileOption">The file options.</param>
        public void Save(string filename, SettingsFileOption fileOption)
        {
            if (!File.Exists(filename))
            {
                string directory = Path.GetDirectoryName(filename);

                if (!String.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);
            }

            //FileOptions.WriteThrough specifies that this file will be written directly to disk, and not to a disk cache.
            //SettingsDocuments can be used to save App configuration values, and this flag would ensure we've written to disk in an EMO situation
            using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, _streamBufferSize, FileOptions.WriteThrough))
            {
                Stream writable = null;
                ICryptoTransform encrypter = null;

                try
                {
                    switch (fileOption)
                    {
                        case SettingsFileOption.Encrypted:
                            Aes aes = AesManaged.Create();
                            aes.BlockSize = 128;
                            aes.KeySize = 256;
                            aes.IV = _encryptionIV;
                            aes.Padding = PaddingMode.PKCS7;
                            aes.Mode = CipherMode.CBC;
                            aes.Key = _encryptionKey;

                            encrypter = aes.CreateEncryptor();
                            writable = new CryptoStream(fileStream, encrypter, CryptoStreamMode.Write);
                            break;
                        case SettingsFileOption.Compressed:
                            writable = new GZipStream(fileStream, CompressionMode.Compress);
                            break;
                        default:
                            writable = fileStream;
                            break;
                    }

                    using (TextWriter file = TextWriter.Synchronized(new StreamWriter(writable)))
                    {
                        WriteToFile(file);
                    }
                }
                finally
                {
                    if (writable != null)
                        writable.Dispose();

                    if (encrypter != null)
                        encrypter.Dispose();
                }
            }
        }

        /// <summary>
        /// Writes this document to the specified TextWriter.
        /// </summary>
        /// <param name="file">The file.</param>
        private void WriteToFile(TextWriter file)
        {
            SettingsNode currentNode = this[0];

            while (currentNode != null)
            {
                // Write node to file
                List<string> fileComments = currentNode.GetPrecedingComments();

                for (int i = 0; i < fileComments.Count; i++)
                    file.WriteLine(fileComments[i]);

                file.WriteLine(NodeToFileLine(currentNode));
                file.Flush();

                // Find next line
                if (currentNode.Nodes.Count > 0)
                    currentNode = currentNode.Nodes[0];
                else if (currentNode.NextSibling != null)
                    currentNode = currentNode.NextSibling;
                else
                {
                    // Trace back to the next unread sibling
                    currentNode = currentNode.Parent;
                    while (currentNode != null)
                    {
                        if (currentNode.NextSibling == null)
                            currentNode = currentNode.Parent;
                        else
                        {
                            currentNode = currentNode.NextSibling;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < _trailingComments.Count; i++)
                file.WriteLine(_trailingComments[i]);

            file.Flush();
        }

        /// <summary>
        /// Returns a list of all of the lines in this file.
        /// </summary>
        public IList<string> GetFileLines()
        {
            IList<string> lines = new List<string>();
            SettingsNode currentNode = this[0];

            while (currentNode != null)
            {
                // Write node to file
                List<string> fileComments = currentNode.GetPrecedingComments();

                for (int i = 0; i < fileComments.Count; i++)
                    lines.Add(fileComments[i]);

                lines.Add(NodeToFileLine(currentNode));

                // Find next line
                if (currentNode.Nodes.Count > 0)
                    currentNode = currentNode.Nodes[0];
                else if (currentNode.NextSibling != null)
                    currentNode = currentNode.NextSibling;
                else
                {
                    // Trace back to the next unread sibling
                    currentNode = currentNode.Parent;
                    while (currentNode != null)
                    {
                        if (currentNode.NextSibling == null)
                            currentNode = currentNode.Parent;
                        else
                        {
                            currentNode = currentNode.NextSibling;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < _trailingComments.Count; i++)
                lines.Add(_trailingComments[i]);

            return lines;
        }

        #endregion

        #region Events

        /// <summary>
        /// Used internally to invoke a node changing event.
        /// </summary>
        /// <param name="src">The node that raised this event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        sealed internal override void RaiseChanging(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (NodeChanging != null)
                NodeChanging(src, e);
        }

        /// <summary>
        /// Used internally to invoke a node changed event.
        /// </summary>
        /// <param name="src">The node that raised this event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        sealed internal override void RaiseChanged(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (NodeChanged != null)
                NodeChanged(src, e);

            if (_persistToFile)
                Save(LoadedFile);
        }

        /// <summary>
        /// Used internally to invoke a node inserting event.
        /// </summary>
        /// <param name="src">The node that raised this event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        sealed internal override void RaiseInserting(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (NodeInserting != null)
                NodeInserting(src, e);
        }

        /// <summary>
        /// Used internally to invoke a node inserted event.
        /// </summary>
        /// <param name="src">The node that raised this event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        sealed internal override void RaiseInserted(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (NodeInserted != null)
                NodeInserted(src, e);

            if (_persistToFile)
                Save(LoadedFile);
        }

        /// <summary>
        /// Used internally to invoke a node removing event.
        /// </summary>
        /// <param name="src">The node that raised this event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        sealed internal override void RaiseRemoving(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (NodeRemoving != null)
                NodeRemoving(src, e);
        }

        /// <summary>
        /// Used internally to invoke a node removed event.
        /// </summary>
        /// <param name="src">The node that raised this event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        sealed internal override void RaiseRemoved(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (NodeRemoved != null)
                NodeRemoved(src, e);

            if (_persistToFile)
                Save(LoadedFile);
        }

        /// <summary>
        /// Occurs when the <see cref="SettingsNode.Value"/> of a node belonging to this document is about to be changed.
        /// </summary>
        public event SettingsNodeChangeEventHandler NodeChanging;

        /// <summary>
        /// Occurs when the <see cref="SettingsNode.Value"/> of a node belonging to this document has been changed.
        /// </summary>
        public event SettingsNodeChangeEventHandler NodeChanged;

        /// <summary>
        /// Occurs when a node belonging to this document is about to be inserted.
        /// </summary>
        public event SettingsNodeChangeEventHandler NodeInserting;

        /// <summary>
        /// Occurs when a node belonging to this document has been inserted.
        /// </summary>
        public event SettingsNodeChangeEventHandler NodeInserted;

        /// <summary>
        /// Occurs when a node belonging to this document is about to be removed.
        /// </summary>
        public event SettingsNodeChangeEventHandler NodeRemoving;

        /// <summary>
        /// Occurs when a node belonging to this document has been removed.
        /// </summary>
        public event SettingsNodeChangeEventHandler NodeRemoved;

        #endregion
    }

    #region XyratexOSC.Settings Namespace Documentation

    /// <summary>
    /// <para>The <see cref="XyratexOSC.Settings"/> namespace is used for processing tab-structured settings files. It contains document, 
    /// node, and collection classes that can parse, manipulate, and save tab-delimitted settings files.</para>
    /// <para>This namespace is used in sub-system configuration and for loading user/application settings.</para>
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="SettingsDocument"/> class represents a settings file in memory with reading/writing to file capabilities.
    /// The settings documents are built from <see cref="SettingsNode"/>s that contain a <see cref="SettingsNodeList"/> collection of
    /// child nodes.</para>
    /// </remarks>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class NamespaceDoc
    {
        //An empty class used by Sandcastle Help File Builder (http://shfb.codeplex.com/)
        //to create a summary for our Namespace
    }

    #endregion
}
