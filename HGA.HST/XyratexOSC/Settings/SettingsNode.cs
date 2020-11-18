using System;
using System.Collections.Generic;
using System.Linq;

namespace XyratexOSC.Settings
{
    #region SettingsNode Class

    /// <summary>
    /// Represents a single node of a Settings Document (tab-structured).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SettingsNode class represents Settings data by the node's name, child nodes, and (optionally) comment 
    /// information, a defined value type, a read-only setting, and print formatting. The <see cref="SettingsDocument"/> 
    /// class extends SettingsNode and represents a Settings document. You can use SettingsDocument to load and save 
    /// Settings data.
    /// </para>
    /// <para><h3>Value Nodes</h3></para>
    /// <para>
    /// While SettingsNodes maintain a tree structure by collections of other SettingsNodes, the ultimate purpose of 
    /// a SettingsNode is to provide a configuration/setting value for a particular attribute.
    /// </para>
    /// <para>
    /// A SettingsNode is considered a 'value' when
    /// it has no sibling nodes and no child nodes (see <see cref="IsAValue"/>). A node 'has' a value when it contains
    /// only a single child 'value' node (see <see cref="HasAValue"/>). The node path leading up to a value node provides
    /// a unique identifier for an attribute (ie. Motion|Axis 1|MaxVelocity) and the value node provides it's value.
    /// </para>
    /// <para><h3>Value Types</h3></para>
    /// <para>
    /// Values can be strongly typed in-code (see the <see cref="Type"/> property) or through an in-line comment in
    /// a settings file for a node that has a value '[Type:System.Double]'. Defining a type performs a conversion of the
    /// value node to the specified type at load/setting time instead of at read time. It will also cause a node to
    /// throw an InvalidCastException if it cannot set a new value to the specified type.
    /// </para>
    /// </remarks>
    public class SettingsNode
    {
        #region Fields

        /// <summary>
        /// The empty node name
        /// </summary>
        public static readonly string EmptyNodeName = "(empty)";

        /// <summary>The collection of child nodes.</summary>
        protected SettingsNodeList _nodes;

        /// <summary>The parent of this node.</summary>
        protected SettingsNode _parent = null;

        /// <summary>Line-by-line list of comments that preceded this node when read from file</summary>
        protected List<string> _precedingComments;

        /// <summary>The node name.</summary>
        protected string _name = "";

        /// <summary>The node description</summary>
        protected string _info = "";

        /// <summary>The explicitly defined type of value for this node.</summary>
        protected Type _type = null;

        /// <summary>Identifies the read-only property of this node.</summary>
        protected bool _readOnly = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the child <see cref="XyratexOSC.Settings.SettingsNode"/> with the specified name.
        /// </summary>
        /// <remarks>
        /// This indexer also handles node paths that are separated with the node <see cref="PathSeparator"/>.
        /// So this["childNodeName|grandchildNodeName|etc"] will return the specified node if the entire path exists.
        /// </remarks>
        public SettingsNode this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    return null;

                string[] nodes = name.Split(PathSeparator);
                SettingsNode childNode = this;

                foreach (string nodeName in nodes)
                {
                    if (childNode == null)
                        return null;

                    if (childNode.IsList)
                    {
                        if (nodeName.Contains("["))
                        {
                            string[] nameAndIndex = name.Split('[',']');
                            childNode = childNode.Nodes[nameAndIndex[0]][Int32.Parse(nameAndIndex[1])];
                        }
                        else
                            return null;
                    }
                    else
                    {
                        childNode = childNode.Nodes[nodeName];
                    }
                }

                return childNode;
            }
            set
            {
                RemoveChild(name);

                if (value != null)
                    AddChild(value);
            }
        }

        /// <summary>
        /// Gets or sets the child <see cref="XyratexOSC.Settings.SettingsNode"/> at the specified index.
        /// </summary>
        public SettingsNode this[int index]
        {
            get
            {
                if (index < Nodes.Count)
                    return Nodes[index];
                else
                    return null;
            }
            set
            {
                RemoveChildAt(index);

                if (value != null)
                    InsertChild(index, value);
            }
        }

        /// <summary>
        /// Gets the full path from the top-level <see cref="SettingsDocument"/> node to this node. 
        /// Each node is delimited by the SettingsDocument <see cref="PathSeparator"/>.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get
            {
                if (_parent == null || _parent.GetType() == typeof(SettingsDocument))
                    return Name;
                else
                    return _parent.FullPath + GetPathSeparator() + Name;
            }
        }

        /// <summary>
        /// Gets or sets the format string for translating this node to string when writing to file.
        /// </summary>
        /// <value>
        /// The formatting string.
        /// </value>
        public string Formatting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the top-level document node PathSeparator or '|' if node does not belong to a document.
        /// </summary>
        /// <value>The path separator.</value>
        public virtual char PathSeparator
        {
            get { return GetPathSeparator(); }
            set { }
        }

        /// <summary>
        /// Gets the 0-based tree level of this node.
        /// </summary>
        /// <value>The node's level.</value>
        public int Level
        {
            get { return GetLevel(); }
        }

        /// <summary>
        /// Gets a count of immediate child nodes.
        /// </summary>
        /// <value>The number of nodes that belong to this node.</value>
        public int Count
        {
            get { return _nodes.Count; }
        }

        /// <summary>
        /// Gets the count all child nodes below this node.
        /// </summary>
        /// <value>The deep-count of all nodes belonging to this node.</value>
        public int CountAll
        {
            get
            {
                int count = 0;
                foreach (SettingsNode node in _nodes)
                    count += node.CountAll + 1;

                return count;
            }
        }

        /// <summary>
        /// Gets or sets the collection of child nodes.
        /// </summary>
        /// <value>The child nodes.</value>
        /// <seealso cref="SettingsNodeList"/>
        public SettingsNodeList Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
                _nodes.Parent = this;
            }
        }

        /// <summary>
        /// Gets the first child belonging to this node.
        /// </summary>
        /// <value>The first child.</value>
        public SettingsNode FirstChild
        {
            get
            {
                if (_nodes.Count > 0)
                    return _nodes[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the last child belonging to this node.
        /// </summary>
        /// <value>The last child.</value>
        public SettingsNode LastChild
        {
            get
            {
                if (_nodes.Count > 0)
                    return _nodes[_nodes.Count - 1];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the next sibling of this node.
        /// </summary>
        /// <value>The next sibling.</value>
        public SettingsNode NextSibling
        {
            get
            {
                if (_parent == null)
                    return null;
                else
                {
                    int myIndex = _parent.Nodes.IndexOf(this);

                    if (myIndex < _parent.Count - 1)
                        return _parent[myIndex + 1];
                    else
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent of this node.
        /// </summary>
        /// <value>The parent.</value>
        public SettingsNode Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }

        /// <summary>
        /// <para>Determines whether this node 'is a value'.</para>
        /// <para>A node is considered a value node when it has no siblings and no children.</para>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a value; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// A node that 'is a value' can get/set a value, get/set a type, and be accessed from 
        /// it's parent node (which would 'have' a value) as a value. 
        /// See <see cref="Value"/> for more specifics.
        /// </remarks>
        public bool IsAValue
        {
            get 
            { 
                if (_nodes.Count > 0)
                    return false;

                if (_parent == null)
                    return false;

                return (_parent.IsList || _parent.Nodes.Count == 1); 
            }
        }

        /// <summary>
        /// <para>Determines whether this node 'has a value'.</para>
        /// <para>A node is considered to have a value node when it has a single child 'value' node.</para>
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has a value; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// A node that 'has a value' can get/set it's value, get/set it's type, and can be validated for
        /// reading as a setting by <see cref="ExistsAndHasAValue(string)"/>.
        /// See <see cref="Value"/> for more specifics.
        /// </remarks>
        public bool HasAValue
        {
            get 
            {
                return (_nodes.Count == 1 && _nodes[0].Count == 0); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this represents an empty (null) node.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                return (String.IsNullOrEmpty(Name) || String.Equals(Name, EmptyNodeName));
            }
        }

        /// <summary>
        /// Gets or sets the node name. If this node is a value node then setting the name will cause the
        /// value to change to the type-converted value of the passed string (based on the node type).
        /// </summary>
        /// <value>The name.</value>
        /// <exception cref="InvalidCastException">
        /// This node is a value node, and this conversion is not supported. (or) The value is null and 
        /// conversionType is a value type. (or) The value does not implement the IConvertible interface.
        /// </exception>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;

                _name = value.Replace("[", "").Replace("]", "").Trim();
            }
        }

        /// <summary>
        /// Gets or sets the nodes comments/description. This information is retained in when written 
        /// to file as a semicolon comment.
        /// </summary>
        /// <value>The node comment.</value>
        public string Info
        {
            get 
            { 
                return _info; 
            }
            set 
            { 
                _info = value; 
            }
        }

        /// <summary>
        /// Gets or sets the type for this node. Used only when this node 'is' or 'has' a value. 
        /// See <see cref="IsAValue"/> and <see cref="HasAValue"/>.
        /// </summary>
        /// <value>The node value type.</value>
        /// <seealso cref="IsAValue"/>
        /// <seealso cref="HasAValue"/>
        public Type Type
        {
            get 
            {
                if (IsAValue)
                {
                    if (_type == null)
                    {
                        if (_parent.IsList && _parent._type != null)
                            return _parent._type.GetElementType();
                        else if (_parent._type != null)
                            return _parent._type;
                        else
                            return typeof(string);
                    }
                    else
                        return _type;
                }
                else if (HasAValue)
                    return _nodes[0].Type;
                else
                    return null;
            }
            internal set 
            {
                _type = value;

                if (HasAValue)
                    foreach (SettingsNode node in _nodes)
                        node.Type = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node represents a list.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is list; otherwise, <c>false</c>.
        /// </value>
        public bool IsList
        {
            get
            {
                return (ListLength > -1) || (_type != null && _type.GetInterface("IList", true) != null);
            }
            set
            {
                if (IsList == value)
                    return;

                if (value)
                    ListLength = _nodes.Count;
                else
                    ListLength = -1;
            }
        }

        /// <summary>
        /// Gets the list length.
        /// </summary>
        public int ListLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value for this node if 'is' or 'has' a value. Otherwise, returns null.
        /// </summary>
        /// <value>The node value.</value>
        /// <remarks>
        /// <para><h3>Getting</h3></para>
        /// <para>
        /// If the node <see cref="IsAValue"/>, return the previously set value or, if none exists, return the node name 
        /// converted to the node value <see cref="Type"/>.
        /// If the node <see cref="HasAValue"/>, return the value from it's child 'value' node.
        /// </para>
        /// <para><h3>Setting</h3></para>
        /// <para>
        /// If the node <see cref="HasAValue"/>, set it's child node's value, type (based on the value), and name (value.ToString).
        /// If the node <see cref="IsAValue"/>, setting a new value will convert this to 'having' a value node and create a new child.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsAValue"/>
        /// <seealso cref="HasAValue"/>
        public object Value
        {
            get 
            {
                if (IsAValue)
                {
                    if (String.IsNullOrEmpty(Name))
                        return null;

                    if (Type != null && Type != typeof(String))
                        return SettingsConverter.ConvertValue(Name, Type);
                    else
                        return Name;
                }
                else if (HasAValue)
                {
                    return _nodes[0].Value;
                }
                else
                    return null;
            }
            set 
            {
                if (this.ReadOnly)
                    return;

                if (_nodes.Count == 0)
                {
                    if (value == null)
                        AddChild(new SettingsNode(""));
                    else
                        AddChild(new SettingsNode(value.ToString(), "", value.GetType()));
                }
                else if (HasAValue)
                {
                    SettingsNodeChangeEventArgs e = new SettingsNodeChangeEventArgs(this, _parent, _parent, Value, value, SettingsNodeChangeAction.Change);
                    RaiseChanging(this, e);

                    if (value == null)
                    {
                        _nodes[0].Name = "";
                        _nodes[0]._type = null;
                    }
                    else
                    {
                        _nodes[0].Name = value.ToString();
                        _nodes[0]._type = value.GetType();
                    }

                    RaiseChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node's value is Read-Only.
        /// </summary>
        /// <value><c>false</c> if node 'is' or 'has' a value and is not read only; 
        /// otherwise, <c>true</c>.</value>
        /// <seealso cref="IsAValue"/>
        /// <seealso cref="HasAValue"/>
        public virtual bool ReadOnly
        {
            get
            {
                if (IsAValue)
                    return _readOnly;
                else if (HasAValue)
                    return _nodes[0].ReadOnly;
                else
                    return false;
            }
            set { SetReadOnly(value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// An empty settings node for representing null values.
        /// </summary>
        public static readonly SettingsNode Empty = new SettingsNode(EmptyNodeName);

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNode"/> class 
        /// with the specified node name.
        /// </summary>
        /// <param name="name">The node name.</param>
        public SettingsNode(string name)
        {
            Formatting = "{0}";
            ListLength = -1;
            _nodes = new SettingsNodeList(this);
            _precedingComments = new List<string>();
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNode"/> class 
        /// with the specified node name and description.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="info">The node description.</param>
        public SettingsNode(string name, string info) : this(name) 
        {
            _info = info;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNode"/> class 
        /// with the specified node name, description, and value type.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="info">The node description.</param>
        /// <param name="type">The value type.</param>
        public SettingsNode(string name, string info, Type type) : this(name, info)
        {
            _type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNode"/> class 
        /// with the specified node name, description, value type, value, and read-only setting.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="info">The node description.</param>
        /// <param name="type">The value type.</param>
        /// <param name="readOnly">if set to <c>true</c> value is read-only.</param>
        public SettingsNode(string name, string info, Type type, bool readOnly) : this(name, info, type)
        {
            _readOnly = readOnly;
        }

        /// <summary>
        /// Checks if a specified node exists and has a value, that we can inquire before attempting to get a <see cref="Value"/>.
        /// </summary>
        /// <param name="node">The node being validated.</param>
        /// <returns><c>true</c> if the node is not null and <see cref="HasAValue"/>; otherwise, <c>false</c></returns>
        public static bool ExistsAndHasAValue(SettingsNode node)
        {
            if (node == null)
                return false;

            return node.HasAValue;
        }

        /// <summary>
        /// Checks if a specified node exists and has a value of the specified Generic type, that we can inquire before attempting to get a <see cref="Value"/>.
        /// </summary>
        /// <typeparam name="T">The specified type to check a value for.</typeparam>
        /// <param name="node">The node being validated.</param>
        /// <returns>
        /// 	<c>true</c> if the node is not null and <see cref="HasAValue"/> of type T; otherwise, <c>false</c>
        /// </returns>
        public static bool ExistsAndHasAValue<T>(SettingsNode node)
        {
            if (node == null)
                return false;

            if (!node.HasAValue)
                return false;

            try
            {
                node.GetValueAs<T>();
                return true;
            }
            catch ( FormatException )
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a preceding comment/whitespace line used when saving this node back to a file.
        /// </summary>
        /// <param name="precedingComment">The preceding comment line.</param>
        internal void AddPrecedingComment(string precedingComment)
        {
            _precedingComments.Add(precedingComment);
        }

        /// <summary>
        /// Gets all the preceding comments to write to file before writing this node.
        /// </summary>
        /// <returns></returns>
        internal List<string> GetPrecedingComments()
        {
            return _precedingComments;
        }

        /// <summary>
        /// Gets this node's tree level.
        /// </summary>
        /// <returns>Number representing node level.</returns>
        /// <remarks>
        /// A level of zero would be a node directly under a <see cref="SettingsDocument"/> node.
        /// </remarks>
        protected virtual int GetLevel()
        {
            if (_parent == null)
                return 0;
            else
                return _parent.Level + 1;
        }

        /// <summary>
        /// Gets the node value as a specified type. Follows the <see cref="IsAValue"/> and <see cref="HasAValue"/> rules.
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <returns>The node value converted to the specified type.</returns>
        /// <exception cref="InvalidCastException">
        /// This conversion is not supported. (or) The value is null and conversionType is a value type.
        /// (or) The value does not implement the IConvertible interface.
        /// </exception>
        /// <seealso cref="Value"/>
        public T GetValueAs<T>()
        {
            Type returnType = typeof(T);

            if (returnType == Type)
                return (T)Value;
            else
                return (T)SettingsConverter.ConvertValue(Value, returnType);
        }

        /// <summary>
        /// Determines whether the specified value can be converted to the Node's <see cref="Type"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValueValid(object value)
        {
            try
            {
                SettingsConverter.ConvertValue(value, this.Type);
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a specified node-path exists within this SettingsDocument.
        /// </summary>
        /// <param name="nodePath">The node path.</param>
        /// <returns><c>true</c> if the node is not null; otherwise, <c>false</c></returns>
        public bool Exists(string nodePath)
        {
            if (String.IsNullOrWhiteSpace(nodePath))
                return false;

            SettingsNode node = null;

            try
            {
                node = this.GetNode(nodePath);
            }
            catch (Exception)
            {
                //Node does not exist
            }

            if (node == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks if a specified node-path exists within this SettingsDocument and if the node has a value, 
        /// that we can inquire before attempting to get a <see cref="SettingsNode.Value"/>.
        /// </summary>
        /// <param name="nodePath">The node path.</param>
        /// <returns><c>true</c> if the node is not null and <see cref="SettingsNode.HasAValue"/>; otherwise, <c>false</c></returns>
        public bool ExistsAndHasAValue(string nodePath)
        {
            if (String.IsNullOrWhiteSpace(nodePath))
                return false;

            SettingsNode node = this.TryGetNode(nodePath);

            if (node == null)
                return false;
            else
                return node.HasAValue;
        }

        /// <summary>
        /// Checks if a specified node-path exists and if the node has a value of the specified Generic type, that we can inquire before attempting to get a <see cref="SettingsNode.Value"/>.
        /// </summary>
        /// <typeparam name="T">The specified type to check a value for.</typeparam>
        /// <param name="nodePath">The node path.</param>
        /// <returns>
        /// 	<c>true</c> if the node is not null and <see cref="SettingsNode.HasAValue"/> of type T; otherwise, <c>false</c>
        /// </returns>
        public bool ExistsAndHasAValue<T>(string nodePath)
        {
            if (String.IsNullOrWhiteSpace(nodePath))
                return false;

            SettingsNode node = this.TryGetNode(nodePath);

            if (node == null || !node.HasAValue)
                return false;

            try
            {
                node.GetValueAs<T>();
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the read only field depending on if the node <see cref="IsAValue"/> or <see cref="HasAValue"/>.
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> the value is read-only.</param>
        private void SetReadOnly(bool readOnly)
        {
            if (IsAValue)
                _readOnly = readOnly;
            else if (HasAValue)
                _nodes[0].ReadOnly = readOnly;
        }

        /// <summary>
        /// Creates a deep copy of this node.
        /// </summary>
        /// <returns>The node copy.</returns>
        public SettingsNode Clone()
        {
            return CloneNode(false);
        }

        /// <summary>
        /// Creates a copy of this node shallowly or deeply depending on the specified paramter.
        /// </summary>
        /// <param name="shallowCopy">if set to <c>true</c> create a shallow copy of the node (does not copy child nodes).</param>
        /// <returns>The node copy.</returns>
        public SettingsNode CloneNode(bool shallowCopy)
        {
            SettingsNode clone = new SettingsNode(Name, _info, _type, _readOnly);
            clone.Formatting = Formatting;

            if (!shallowCopy)
                for (int i = 0; i < _nodes.Count; i++)
                    clone.AddChild(_nodes[i].Clone());

            return clone;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance formatted according to the formatting string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (IsAValue)
                return String.Format("{0} ({1})", Name, Type);

            if (IsList)
                return String.Format("{0} : List[{1}]", Name, ListLength);

            if (HasAValue)
                return String.Format("{0} = {1}", Name, Value);

            return Name;
        }

        /// <summary>
        /// Gets the path separator that this node's <see cref="SettingsDocument"/> uses for
        /// delimiting each node when getting a setting using a single string (ie. "Settings|Generic|Attribute").
        /// </summary>
        /// <returns>The node delimiter character.</returns>
        /// <seealso cref="GetNode(string, string[])"/>
        internal char GetPathSeparator()
        {
            if (this.GetType() == typeof(SettingsDocument))
                return ((SettingsDocument)this).PathSeparator;
            else if (_parent != null)
                return _parent.GetPathSeparator();
            else
                return '|';
        }

        /// <summary>
        /// Gets the child node with the specified name.
        /// </summary>
        /// <param name="name">The child node name.</param>
        /// <returns>The child node with the specified name</returns>
        public SettingsNode SelectNode(string name)
        {
            return _nodes.SelectNode(name);
        }

        /// <summary>
        /// Gets a collection (<see cref="SettingsNodeList"/>) of all child nodes
        /// with names that start with the specified string.
        /// </summary>
        /// <param name="nameBeginning">The child nodes name beginning.</param>
        /// <returns>The child nodes whose names start with the specified string.</returns>
        public SettingsNodeList SelectNodesStartingWith(string nameBeginning)
        {
            return _nodes.SelectNodesStartingWith(nameBeginning);
        }

        /// <summary>
        /// Gets a collection (<see cref="SettingsNodeList"/>) of all child nodes
        /// with names that contain the specified string.
        /// </summary>
        /// <param name="namePartial">The child nodes partial name.</param>
        /// <returns>The child nodes whose names contain the specified string.</returns>
        public SettingsNodeList SelectNodesContaining(string namePartial)
        {
            return _nodes.SelectNodesContaining(namePartial);
        }

        /// <summary>
        /// Adds the specified <see cref="SettingsNode"/> as a child to this node.
        /// </summary>
        /// <param name="node">The child node.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode AddChild(SettingsNode node)
        {
            _nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified name and adds it as a child to this node.
        /// </summary>
        /// <param name="name">The child node name.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode AddChild(string name)
        {
            return AddChild(new SettingsNode(name));
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified name, description, type, and value;
        /// and adds it as a child to this node.
        /// </summary>
        /// <param name="name">The child node name.</param>
        /// <param name="info">The child node info.</param>
        /// <param name="type">The child node value type.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode AddChild(string name, string info, Type type)
        {
            return AddChild(new SettingsNode(name, info, type));
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified name, description, type, and value;
        /// and adds it as a child to this node.
        /// </summary>
        /// <param name="name">The child node name.</param>
        /// <param name="info">The child node info.</param>
        /// <param name="type">The child node value type.</param>
        /// <param name="value">The child node value.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode AddChild(string name, string info, Type type, object value)
        {
            SettingsNode newChild = AddChild(name, info, type);

            if (value != null)
                newChild.AddChild(value.ToString());
            else
                newChild.AddChild("");

            return newChild;
        }

        /// <summary>
        /// Determines whether the specified node is a child node of this node.
        /// </summary>
        /// <param name="node">The specified node.</param>
        /// <returns>
        /// 	<c>true</c> if the specified node is a child; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(SettingsNode node)
        {
            return _nodes.Contains(node);
        }

        /// <summary>
        /// Determines whether the node specified by name is a child node of this node.
        /// </summary>
        /// <param name="name">The specified node name.</param>
        /// <returns>
        /// 	<c>true</c> if the node specified by name is a child; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsName(string name)
        {
            return _nodes.ContainsName(name);
        }

        /// <summary>
        /// Gets the child node following the specified delimited path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The specified node.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <remarks>
        /// Nodes in the path should be separated by the parent <see cref="SettingsDocument.PathSeparator"/>.
        /// </remarks>
        public SettingsNode GetNode(string path)
        {
            return _nodes.GetNode(path);
        }

        /// <summary>
        /// Gets the child node following the specified delimited path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The specified node or null if the path is invalid.</returns>
        /// <remarks>
        /// Nodes in the path should be separated by the parent <see cref="SettingsDocument.PathSeparator"/>.
        /// </remarks>
        public SettingsNode TryGetNode(string path)
        {
            return _nodes.TryGetNode(path);
        }

        /// <summary>
        /// Gets the child node following the specified delimited path and the following array of node names
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The specified node.</returns>
        /// <example>
        /// <code>
        /// topNode.GetNode("child1|childOfchild","thirdLevelChild","targetNode");
        /// </code>
        /// -or-
        /// <code>
        /// topNode.GetNode("child1","childOfchild","thirdLevelChild","targetNode");
        /// </code>
        /// </example>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <remarks>
        /// Nodes in the path should be separated by the parent <see cref="SettingsDocument.PathSeparator"/>.
        /// </remarks>
        public SettingsNode GetNode(string path, params string[] nodes)
        {
            return _nodes.GetNode(path, nodes);
        }

        /// <summary>
        /// Gets the child node following the specified delimited path and the following array of node names
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The specified node or null if the path is invalid.</returns>
        /// <example>
        /// <code>
        /// topNode.GetNode("child1|childOfchild","thirdLevelChild","targetNode");
        /// </code>
        /// -or-
        /// <code>
        /// topNode.GetNode("child1","childOfchild","thirdLevelChild","targetNode");
        /// </code>
        /// </example>
        /// <remarks>
        /// Nodes in the path should be separated by the parent <see cref="SettingsDocument.PathSeparator"/>.
        /// </remarks>
        public SettingsNode TryGetNode(string path, params string[] nodes)
        {
            return _nodes.TryGetNode(path, nodes);
        }

        /// <summary>
        /// Gets the child node following the specified delimited path using a special delimiter.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The specified node.</returns>
        /// <param name="delimiter">The node delimiter.</param>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        public SettingsNode GetNode(string path, char delimiter)
        {
            return GetNode(null, path.Split(delimiter));
        }

        /// <summary>
        /// Gets the child node following the specified delimited path using a special delimiter.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The specified node or null if the path is invalid.</returns>
        /// <param name="delimiter">The path delimiter.</param>
        public SettingsNode TryGetNode(string path, char delimiter)
        {
            return TryGetNode(null, path.Split(delimiter));
        }

        /// <summary>
        /// Gets the value of the child node following the specified delimited path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The value of the specified node.</returns>
        /// <remarks>
        /// Nodes in the path should be separated by the parent <see cref="SettingsDocument.PathSeparator"/>.
        /// </remarks>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <seealso cref="GetNode(string)"/>
        public object GetNodeValue(string path)
        {
            return GetNode(path).Value;
        }

        /// <summary>
        /// Gets the value of the node found by <see cref="GetNode(string, string[])"/>.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The value of the specified node.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <seealso cref="GetNode(string, string[])"/>
        public object GetNodeValue(string path, params string[] nodes)
        {
            return GetNode(path, nodes).Value;
        }

        /// <summary>
        /// Gets the value of the child node following the specified delimited path using a special delimiter.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="delimiter">The node delimiter.</param>
        /// <returns>The value of the specified node.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <seealso cref="GetNode(string, char)"/>
        public object GetNodeValue(string path, char delimiter)
        {
            return GetNode(path, delimiter).Value;
        }

        /// <summary>
        /// Gets the value-type of the child node following the specified delimited path.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <returns>The value of the specified node.</returns>
        /// <remarks>
        /// Nodes in the path should be separated by the parent <see cref="SettingsDocument.PathSeparator"/>.
        /// </remarks>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <seealso cref="GetNode(string)"/>
        public Type GetNodeType(string path)
        {
            return GetNode(path).Type;
        }

        /// <summary>
        /// Gets the value-type of the node found by <see cref="GetNode(string, string[])"/>.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The value of the specified node.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <seealso cref="GetNode(string, string[])"/>
        public Type GetNodeType(string path, params string[] nodes)
        {
            return GetNode(path, nodes).Type;
        }

        /// <summary>
        /// Gets the value-type of the child node following the specified delimited path using a special delimiter.
        /// </summary>
        /// <param name="path">The node path.</param>
        /// <param name="delimiter">The node delimiter.</param>
        /// <returns>The value of the specified node.</returns>
        /// <exception cref="KeyNotFoundException">
        /// The node path is invalid.
        /// </exception>
        /// <seealso cref="GetNode(string, char)"/>
        public Type GetNodeType(string path, char delimiter)
        {
            return GetNode(path, delimiter).Type;
        }

        /// <summary>
        /// Finds all children of this node with the specified name, optionally including the entire tree of nodes that belong to this node.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="searchAllChildren">if set to <c>true</c> search for children deeply.</param>
        /// <returns>A list of children nodes that have the specified name</returns>
        public SettingsNodeList Find(string name, bool searchAllChildren)
        {
            return _nodes.Find(name, searchAllChildren);
        }

        /// <summary>
        /// Returns the index of the specified child node within this node.        
        /// </summary>
        /// <param name="node">The specified node.</param>
        /// <returns>The specified node index relative to this node.</returns>
        public int IndexOf(SettingsNode node)
        {
            return _nodes.IndexOf(node);
        }

        /// <summary>
        /// Inserts the specified <see cref="SettingsNode"/> as a child to this node at the specified index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <param name="node">The child node.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChild(int index, SettingsNode node)
        {
            _nodes.Insert(index, node);
            return node;
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified name 
        /// and inserts it as a child to this node at the specified index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <param name="name">The child node name.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChild(int index, string name)
        {
            return InsertChild(index, new SettingsNode(name));
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified name, info, type, and value 
        /// and inserts it as a child to this node at the specified index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <param name="name">The child node name.</param>
        /// <param name="info">The child node info.</param>
        /// <param name="type">The child node value type.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChild(int index, string name, string info, Type type)
        {
            return InsertChild(index, new SettingsNode(name, info, type));
        }

        /// <summary>
        /// Inserts the specified <see cref="SettingsNode"/> as a child to this node before another specified child node.
        /// </summary>
        /// <param name="refNode">The child reference .</param>
        /// <param name="newNode">The child node.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChildBefore(SettingsNode refNode, SettingsNode newNode)
        {
            _nodes.InsertBefore(refNode, newNode);
            return newNode;
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified new name 
        /// and inserts it as a child to this node before the referenced node.
        /// </summary>
        /// <param name="refName">The reference child node name.</param>
        /// <param name="newName">The new child node name.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChildBefore(string refName, string newName)
        {
            return InsertChildBefore(_nodes[refName], new SettingsNode(newName));
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified new name, info, type, and value 
        /// and inserts it as a child to this node before the referenced node.
        /// </summary>
        /// <param name="refName">The reference child node name.</param>
        /// <param name="newName">The new child node name.</param>
        /// <param name="newInfo">The new child node info.</param>
        /// <param name="newType">The new child node value type.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChildBefore(string refName, string newName, string newInfo, Type newType)
        {
            return InsertChildBefore(_nodes[refName], new SettingsNode(newName, newInfo, newType));
        }

        /// <summary>
        /// Inserts the specified <see cref="SettingsNode"/> as a child to this node after another specified child node.
        /// </summary>
        /// <param name="refNode">The child index.</param>
        /// <param name="newNode">The child node.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChildAfter(SettingsNode refNode, SettingsNode newNode)
        {
            _nodes.InsertAfter(refNode, newNode);
            return newNode;
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified new name 
        /// and inserts it as a child to this node after the referenced node.
        /// </summary>
        /// <param name="refName">The reference child node name.</param>
        /// <param name="newName">The new child node name.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChildAfter(string refName, string newName)
        {
            return InsertChildAfter(_nodes[refName], new SettingsNode(newName));
        }

        /// <summary>
        /// Instantiates a <see cref="SettingsNode"/> by the specified new name, info, type, and value 
        /// and inserts it as a child to this node after the referenced node.
        /// </summary>
        /// <param name="refName">The reference child node name.</param>
        /// <param name="newName">The new child node name.</param>
        /// <param name="newInfo">The new child node info.</param>
        /// <param name="newType">The new child node value type.</param>
        /// <returns>A reference to the newly added child node.</returns>
        public SettingsNode InsertChildAfter(string refName, string newName, string newInfo, Type newType)
        {
            return InsertChildAfter(_nodes[refName], new SettingsNode(newName, newInfo, newType));
        }

        /// <summary>
        /// Removes the specified child node.
        /// </summary>
        /// <param name="node">The specified node.</param>
        public void RemoveChild(SettingsNode node)
        {
            _nodes.Remove(node);
        }

        /// <summary>
        /// Removes the child node by the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        public void RemoveChild(string name)
        {
            _nodes.Remove(name);
        }

        /// <summary>
        /// Removes the child node at the specified index.
        /// </summary>
        /// <param name="index">The specified index.</param>
        public void RemoveChildAt(int index)
        {
            _nodes.RemoveAt(index);
        }

        /// <summary>
        /// Removes all child nodes.
        /// </summary>
        public void RemoveAllChildren()
        {
            _nodes.RemoveAll();
        }

        #endregion

        #region Events

        /// <summary>
        /// Notifies the parent node that a child node is changing.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        internal virtual void RaiseChanging(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseChanging(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node has changed.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        internal virtual void RaiseChanged(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseChanged(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node is inserting into this node's children.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        internal virtual void RaiseInserting(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseInserting(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node has inserted into this node's children.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        internal virtual void RaiseInserted(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseInserted(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node is removing from this node's children.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        internal virtual void RaiseRemoving(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseRemoving(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node has removed from this node's children.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        internal virtual void RaiseRemoved(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseRemoved(src, e);
        }

        #endregion
    }

    #endregion

    #region SettingsNodeChangeEventHandler Delegate

    /// <summary>
    /// Represents the method that handles <see cref="SettingsDocument.NodeChanging"/>, 
    /// <see cref="SettingsDocument.NodeChanged"/>, <see cref="SettingsDocument.NodeInserting"/>, 
    /// <see cref="SettingsDocument.NodeInserted"/>, <see cref="SettingsDocument.NodeRemoving"/>, 
    /// and <see cref="SettingsDocument.NodeRemoved"/> events.
    /// </summary>
    public delegate void SettingsNodeChangeEventHandler(SettingsNode sender, SettingsNodeChangeEventArgs e);

    #endregion

    #region SettingsNodeChangeEventArgs Class

    /// <summary>
    /// Provides data for the <see cref="SettingsDocument.NodeChanging"/>, 
    /// <see cref="SettingsDocument.NodeChanged"/>, <see cref="SettingsDocument.NodeInserting"/>, 
    /// <see cref="SettingsDocument.NodeInserted"/>, <see cref="SettingsDocument.NodeRemoving"/>, 
    /// and <see cref="SettingsDocument.NodeRemoved"/> events.
    /// </summary>
    public class SettingsNodeChangeEventArgs : EventArgs
    {
        private SettingsNode _node;
        private SettingsNode _oldParent;
        private SettingsNode _newParent;
        private object _oldValue;
        private object _newValue;
        private SettingsNodeChangeAction _action;

        /// <summary>
        /// Gets a value indicating what type of node change event is occuring.
        /// </summary>
        /// <value>A <see cref="SettingsNodeChangeAction"/> value describing the node change event.</value>
        public SettingsNodeChangeAction Action
        {
            get { return _action; }
        }

        /// <summary>
        /// Gets the <see cref="SettingsNode"/> that is being changed, added, or removed.
        /// </summary>
        /// <value>The node that is being changed.</value>
        public SettingsNode Node
        {
            get { return _node; }
        }

        /// <summary>
        /// Gets the old <see cref="SettingsNode.Parent"/> node before the change began.
        /// </summary>
        /// <value>The old parent node.</value>
        public SettingsNode OldParent
        {
            get { return _oldParent; }
        }

        /// <summary>
        /// Gets the new <see cref="SettingsNode.Parent"/> node after the change completes.
        /// </summary>
        /// <value>The new parent node.</value>
        public SettingsNode NewParent
        {
            get { return _newParent; }
        }


        /// <summary>
        /// Gets the original <see cref="SettingsNode.Value"/> of the node.
        /// </summary>
        /// <value>The old node value.</value>
        public object OldValue
        {
            get { return _oldValue; }
        }

        /// <summary>
        /// Gets the new <see cref="SettingsNode.Value"/> of the node.
        /// </summary>
        /// <value>The new node value.</value>
        public object NewValue
        {
            get { return _newValue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNodeChangeEventArgs"/> class.
        /// </summary>
        /// <param name="node">The <see cref="SettingsNode"/> that is being changed, added, or removed.</param>
        /// <param name="oldParent">The old <see cref="SettingsNode.Parent"/> of the node.</param>
        /// <param name="newParent">The new <see cref="SettingsNode.Parent"/> of the node.</param>
        /// <param name="oldValue">The old value of the node.</param>
        /// <param name="newValue">The new value of the node.</param>
        /// <param name="action">The <see cref="SettingsNodeChangeAction"/>.</param>
        public SettingsNodeChangeEventArgs(SettingsNode node, SettingsNode oldParent, SettingsNode newParent, object oldValue, object newValue, SettingsNodeChangeAction action)
        {
            _node = node;
            _oldParent = oldParent;
            _newParent = newParent;
            _oldValue = oldValue;
            _newValue = newValue;
            _action = action;
        }
    }

    #endregion

    #region SettingsNodeChangeAction Enum

    /// <summary>
    /// Specifies the type of node change.
    /// </summary>
    public enum SettingsNodeChangeAction
    {
        /// <summary>
        /// A node is being inserted in the tree.
        /// </summary>
        Insert,
        /// <summary>
        /// A node is being removed from the tree.
        /// </summary>
        Remove,
        /// <summary>
        /// A node value or value-type is being changed.
        /// </summary>
        Change,
    };

    #endregion

}
