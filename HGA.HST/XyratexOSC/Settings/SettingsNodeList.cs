using System;
using System.Collections;
using System.Collections.Generic;

namespace XyratexOSC.Settings
{
    /// <summary>
    /// Represents an ordered collection of SettingsNodes
    /// </summary>
    /// <remarks>
    /// A SettingsNodeList consists of a name-keyed Dictionary and List of SettingsNodes. 
    /// While inserting and deleting nodes takes more time, the SettingsNodes can be searched
    /// and read quickly by index or name.
    /// </remarks>
    /// <seealso cref="SettingsNode"/>
    public class SettingsNodeList : IEnumerable<SettingsNode>
    {
        #region Fields

        private Dictionary<string, SettingsNode> _nodeDict;
        private List<SettingsNode> _nodeIndices;
        private SettingsNode _parent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="XyratexOSC.Settings.SettingsNode"/> with the specified name.
        /// </summary>
        /// <value></value>
        public SettingsNode this[string name]
        {
            get 
            {
                if (_nodeDict.ContainsKey(name))
                    return _nodeDict[name];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="XyratexOSC.Settings.SettingsNode"/> at the specified index.
        /// </summary>
        /// <value></value>
        public SettingsNode this[int index]
        {
            get
            {
                SettingsNode childNode = null;

                if (index < _nodeIndices.Count)
                    childNode = _nodeIndices[index];
                else
                    return null;

                if (childNode == null && _parent.IsList)
                    childNode = SettingsNode.Empty;
                //return _nodeDict[_nodeIndices[index].Key][_nodeIndices[index].Value];

                return childNode;
            }
        }

        /// <summary>
        /// Sets the parent node of this collection (and updates all nodes in the collection).
        /// </summary>
        /// <value>The parent.</value>
        internal SettingsNode Parent
        {
            set 
            { 
                _parent = value;

                foreach (SettingsNode node in _nodeIndices)
                    node.Parent = _parent;
            }
        }

        /// <summary>
        /// Gets the count of nodes in this collection.
        /// </summary>
        /// <value>The node count.</value>
        public int Count
        {
            get { return _nodeIndices.Count; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNodeList"/> class.
        /// </summary>
        public SettingsNodeList()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsNodeList"/> class.
        /// </summary>
        /// <param name="parent">The parent node of this collection.</param>
        internal SettingsNodeList(SettingsNode parent)
        {
            _parent = parent;
            _nodeIndices = new List<SettingsNode>();
            _nodeDict = new Dictionary<string, SettingsNode>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified node exists in this collection.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// 	<c>true</c> if the specified node exists in this collection; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(SettingsNode node)
        {
            foreach (SettingsNode sn in _nodeIndices)
                if (sn == node)
                    return true;

            return false;
        }

        /// <summary>
        /// Determines whether a node with the specified name exists in this collection.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <returns>
        /// 	<c>true</c> if the collection contains a node by the specified name; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsName(string name)
        {
            return _nodeDict.ContainsKey(name);
        }

        /// <summary>
        /// Gets the node following the specified delimited path and, optionally, the following array of node names
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
            SettingsNode node = TryGetNode(path, nodes);

            if (node != null)
                return node;

            string pathSeparator = "|";

            if (_parent != null)
                pathSeparator = _parent.GetPathSeparator().ToString();

            throw new KeyNotFoundException(String.Format("Setting node '{0}' not found.", path, string.Join(pathSeparator, nodes)));
        }

        /// <summary>
        /// Gets the node following the specified delimited path and, optionally, the following array of node names
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The specified node or null if it does not exist.</returns>
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
            if (!String.IsNullOrEmpty(path))
            {
                char pathSeparator = '|';

                if (_parent != null)
                    pathSeparator = _parent.GetPathSeparator();

                string[] pathNodes = path.Split(pathSeparator);

                if (nodes.Length == 0)
                {
                    nodes = pathNodes;
                }
                else
                {
                    string[] allNodes = new string[pathNodes.Length + nodes.Length];
                    for (int i = 0; i < pathNodes.Length; i++)
                        allNodes[i] = pathNodes[i];

                    for (int i = 0; i < nodes.Length; i++)
                        allNodes[pathNodes.Length + i] = nodes[i];

                    nodes = allNodes;
                }
            }

            if (nodes == null || nodes.Length == 0)
                return null;

            string nodeName = nodes[0].Trim();
            int nodeArrayIndex = -1;

            if (nodeName.Contains("["))
            {
                string[] arrayNodeChunks = nodeName.Split('[', ']');
                nodeName = arrayNodeChunks[0].Trim();
                nodeArrayIndex = Int32.Parse(arrayNodeChunks[1]);
            }

            if (!this.ContainsName(nodeName))
                return null;

            SettingsNode childNode = null;

            if (nodeArrayIndex < 0)
                childNode = _nodeDict[nodeName];
            else
                childNode = _nodeDict[nodeName][nodeArrayIndex];

            if (nodes.Length > 1)
            {
                string[] nextNodes = new string[nodes.Length - 1];
                Array.Copy(nodes, 1, nextNodes, 0, nextNodes.Length);
                return childNode.Nodes.GetNode(null, nextNodes);
            }

            return childNode;
        }

        /// <summary>
        /// Gets the value of the node found under the path and, optionally, the following array of node names.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The value of the found node.</returns>
        public object GetNodeValue(string path, params string[] nodes)
        {
            return GetNode(path, nodes).Value;
        }

        /// <summary>
        /// Gets the type of the node found under the path and, optionally, the following array of node names.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="nodes">The remainder of the node path.</param>
        /// <returns>The type of the found node.</returns>
        public Type GetNodeType(string path, params string[] nodes)
        {
            return GetNode(path, nodes).Type;
        }

        /// <summary>
        /// Returns a list of all nodes in the collection that have names starting with the specified string.
        /// </summary>
        /// <param name="nameBeginning">The name beginning.</param>
        /// <returns>A SettingsNodeList of found nodes.</returns>
        public SettingsNodeList SelectNodesStartingWith(string nameBeginning)
        {
            SettingsNodeList nodes = new SettingsNodeList(_parent);

            foreach (SettingsNode node in _nodeIndices)
                if (node.Name.StartsWith(nameBeginning))
                    nodes.Add(node);

            return nodes;
        }

        /// <summary>
        /// Returns a list of all nodes in the collection that have names which contain the specified string.
        /// </summary>
        /// <param name="namePartial">The partial node name.</param>
        /// <returns>A SettingsNodeList of found nodes.</returns>
        public SettingsNodeList SelectNodesContaining(string namePartial)
        {
            SettingsNodeList nodes = new SettingsNodeList(_parent);

            foreach (SettingsNode node in _nodeIndices)
                if (node.Name.Contains(namePartial))
                    nodes.Add(node);

            return nodes;
        }

        /// <summary>
        /// Returns the node specified by name.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <returns>The node with the specified name.</returns>
        public SettingsNode SelectNode(string name)
        {
            return _nodeDict[name];
        }

        /// <summary>
        /// Finds all nodes specified by name either shallowly or deeply (the sub-tree created by all nodes in this collection).
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="searchAllChildren">if set to <c>true</c> search within the tree created by all nodes in this collection.</param>
        /// <returns>A <see cref="SettingsNodeList"/> of all found nodes.</returns>
        public SettingsNodeList Find(string name, bool searchAllChildren)
        {
            return this.FindInternal(name, searchAllChildren, this, new SettingsNodeList(_parent));
        }

        /// <summary>
        /// Recursive search that returns a list of all nodes with the specified name
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="searchAllChildren">if set to <c>true</c> search deeply.</param>
        /// <param name="collection">The collection under inspection.</param>
        /// <param name="foundNodes">The previously found nodes.</param>
        /// <returns>A collection of the previously found nodes with any newly found nodes from the collection under inspection.</returns>
        private SettingsNodeList FindInternal(string name, bool searchAllChildren, SettingsNodeList collection, SettingsNodeList foundNodes)
        {
            if ((collection == null) || (foundNodes == null))
                return null;

            for (int i = 0; i < collection.Count; i++)
                if (collection[i] != null && collection[i].Name == name)
                    foundNodes.Add(collection[i]);

            if (searchAllChildren)
                for (int i = 0; i < collection.Count; i++)
                    if (collection[i] != null && collection[i].Nodes != null && collection[i].Nodes.Count > 0)
                        foundNodes = this.FindInternal(name, searchAllChildren, collection[i].Nodes, foundNodes);

            return foundNodes;
        }

        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of this collection. This iterates by node index.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<SettingsNode> GetEnumerator()
        {
            for (int i = 0; i < _nodeIndices.Count; i++)
                yield return _nodeIndices[i];
        }

        /// <summary>
        /// Gets the index of the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The node index.</returns>
        public int IndexOf(SettingsNode node)
        {
            for (int i = 0; i < _nodeIndices.Count; i++)
                if (_nodeIndices[i] == node)
                    return i;

            return -1;
        }

        /// <summary>
        /// Gets the index of the node specified by name.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <returns>The node index.</returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < _nodeIndices.Count; i++)
                if (_nodeIndices[i].Name == name)
                    return i;

            return -1;
        }

        /// <summary>
        /// Adds the specified node to this collection.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Add(SettingsNode node)
        {
            SettingsNode oldParent = null;
            object value = null;

            if (node != null)
            {
                oldParent = node.Parent;
                value = node.Value;
            }

            SettingsNodeChangeEventArgs e = new SettingsNodeChangeEventArgs(node, oldParent, _parent, value, value, SettingsNodeChangeAction.Insert);
            RaiseInserting(node, e);

            if (node != null)
            {
                // Add node to dictionary (for key retrieval)
                if (_parent == null || !_parent.IsList)
                {
                    if (_nodeDict.ContainsKey(node.Name))
                    {
                        string exMessage;

                        if (_parent != null)
                            exMessage = String.Format("Cannot add node '{0}' to '{1}' because a node with the same name already exists at that level.", node.Name, _parent.FullPath);
                        else
                            exMessage = String.Format("Cannot add node '{0}' because a node with the same name already exists.", node.Name);

                        throw new ArgumentException(exMessage);
                    }

                    _nodeDict.Add(node.Name, node);
                }

                // Add node to list (for index retrieval)
                _nodeIndices.Add(node);

                node.Parent = _parent;
            }

            RaiseInserted(node, e);
        }

        /// <summary>
        /// Inserts the specified node at the specified index.
        /// </summary>
        /// <param name="index">The node index.</param>
        /// <param name="node">The node.</param>
        public void Insert(int index, SettingsNode node)
        {
            if (node == null)
                return;

            SettingsNodeChangeEventArgs e = new SettingsNodeChangeEventArgs(node, node.Parent, _parent, node.Value, node.Value, SettingsNodeChangeAction.Insert);
            RaiseInserting(node, e);

            if (_parent == null || !_parent.IsList)
                _nodeDict.Add(node.Name, node);

            _nodeIndices.Insert(index, node);

            node.Parent = _parent;

            RaiseInserted(node, e);
        }

        /// <summary>
        /// Inserts the specified node before the reference node.
        /// </summary>
        /// <param name="refNode">The reference node.</param>
        /// <param name="newNode">The new node.</param>
        /// <exception cref="ArgumentException">The reference node does not exist in this list.</exception>
        public void InsertBefore(SettingsNode refNode, SettingsNode newNode)
        {
            int index = IndexOf(refNode);
            if (index == -1)
                throw new ArgumentException("SettingsNode '" + refNode.Name + "' was not found in the collection.");

            Insert(index, newNode);
        }

        /// <summary>
        /// Inserts the specified node after the reference node.
        /// </summary>
        /// <param name="refNode">The reference node.</param>
        /// <param name="newNode">The new node.</param>
        /// <exception cref="ArgumentException">The reference node does not exist in this list.</exception>
        public void InsertAfter(SettingsNode refNode, SettingsNode newNode)
        {
            int index = IndexOf(refNode);
            if (index == -1)
                throw new ArgumentException("SettingsNode '" + refNode.Name + "' was not found in the collection.");

            Insert(index + 1, newNode);
        }

        /// <summary>
        /// Removes the old node from the collection and inserts the new node at the old node's index.
        /// </summary>
        /// <param name="oldNode">The old node.</param>
        /// <param name="newNode">The new node.</param>
        /// <exception cref="ArgumentException">The node-to-be-replaced does not exist in this list.</exception>
        public void Replace(SettingsNode oldNode, SettingsNode newNode)
        {
            int index = IndexOf(oldNode);
            if (index == -1)
                throw new ArgumentException("SettingsNode '" + oldNode.Name + "' was not found in the collection.");

            SettingsNodeChangeEventArgs eRemove = new SettingsNodeChangeEventArgs(oldNode, oldNode.Parent, null, oldNode.Value, oldNode.Value, SettingsNodeChangeAction.Remove);
            SettingsNodeChangeEventArgs eInsert = new SettingsNodeChangeEventArgs(newNode, newNode.Parent, _parent, newNode.Value, newNode.Value, SettingsNodeChangeAction.Insert);

            RaiseRemoving(oldNode, eRemove);
            RaiseInserting(newNode, eInsert);

            _nodeIndices[index] = newNode;

            if (_nodeDict.ContainsKey(oldNode.Name))
                _nodeDict.Remove(oldNode.Name);

            if (_parent == null || !_parent.IsList)
                _nodeDict.Add(newNode.Name, newNode);

            newNode.Parent = _parent;
            oldNode.Parent = null;

            RaiseRemoved(oldNode, eRemove);
            RaiseInserted(newNode, eInsert);
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="ArgumentNullException"><c>node</c> is null.</exception>
        public void Remove(SettingsNode node)
        {
            if (node == null)
                throw new ArgumentNullException("Cannot remove a null SettingsNode object from the collection.");

            SettingsNodeChangeEventArgs e = new SettingsNodeChangeEventArgs(node, node.Parent, null, node.Value, node.Value, SettingsNodeChangeAction.Remove);
            RaiseRemoving(node, e);

            _nodeIndices.RemoveAt(IndexOf(node));

            if (_nodeDict.ContainsKey(node.Name))
                _nodeDict.Remove(node.Name);

            node.Parent = null;

            RaiseRemoved(node, e);
        }

        /// <summary>
        /// Removes the node specified by name.
        /// </summary>
        /// <param name="name">The node name.</param>
        public void Remove(string name)
        {
            Remove(_nodeDict[name]);
        }

        /// <summary>
        /// Removes the node at the specified index.
        /// </summary>
        /// <param name="index">The node index.</param>
        public void RemoveAt(int index)
        {
            Remove(_nodeIndices[index]);
        }

        /// <summary>
        /// Removes all nodes from this collection individually. 
        /// </summary>
        /// <remarks>
        /// This method of removing all nodes from the collection 
        /// will allow all nodes to raise their removing/removed events.
        /// </remarks>
        public void RemoveAll()
        {
            while (_nodeIndices.Count > 0)
                Remove(_nodeIndices[_nodeIndices.Count - 1]);
        }

        /// <summary>
        /// Clears all nodes from the collection. 
        /// Note: this will not raise the node removing events.
        /// </summary>
        public void Clear()
        {
            foreach (SettingsNode node in _nodeIndices)
                node.Parent = null;

            _nodeDict.Clear();
            _nodeIndices.Clear();
        }

        /// <summary>
        /// Converts this collection to an array of SettingsNodes.
        /// </summary>
        /// <returns>An array representation of this collection.</returns>
        public SettingsNode[] ToArray()
        {
            return _nodeIndices.ToArray();
        }

        /// <summary>
        /// Converts this collection to a List of SettingsNodes.
        /// </summary>
        /// <returns>A List representation of this collection.</returns>
        public List<SettingsNode> ToList()
        {
            return _nodeIndices;
        }

        /// <summary>
        /// Converts this collection to a Dictionary of SettingsNodes.
        /// </summary>
        /// <returns>A Dictionary representation of this collection.</returns>
        public Dictionary<string, SettingsNode> ToDict()
        {
            return _nodeDict;
        }

        #endregion

        #region Events

        /// <summary>
        /// Notifies the parent node that a child node is inserting.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        protected void RaiseInserting(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseInserting(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node has been inserted.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        protected void RaiseInserted(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseInserted(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node is removing.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        protected void RaiseRemoving(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseRemoving(src, e);
        }

        /// <summary>
        /// Notifies the parent node that a child node has been removed.
        /// </summary>
        /// <param name="src">The node source of the event.</param>
        /// <param name="e">The <see cref="XyratexOSC.Settings.SettingsNodeChangeEventArgs"/> instance containing the event data.</param>
        protected void RaiseRemoved(SettingsNode src, SettingsNodeChangeEventArgs e)
        {
            if (_parent != null)
                _parent.RaiseRemoved(src, e);
        }

        #endregion
    }
}
