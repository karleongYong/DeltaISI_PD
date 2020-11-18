using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace XyratexOSC.Settings
{
    /// <summary>
    /// Provides methods for converting between objects and <see cref="SettingsNode"/>s.
    /// </summary>
    public static class SettingsConverter
    {
        /// <summary>
        /// Converts the specified object to a <see cref="SettingsDocument"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns></returns>
        public static SettingsDocument ConvertObjectToDocument(object obj, string fileName)
        {
            if (obj == null)
                return null;

            SettingsNode settingsNode = ConvertObjectToNode(obj);

            SettingsDocument settingsDoc = new SettingsDocument();
            settingsDoc.PathSeparator = '.';
            settingsDoc.Name = fileName;

            foreach (SettingsNode node in settingsNode.Nodes)
            {
                if (settingsDoc.Nodes.ContainsName(node.Name))
                    UpdateNodeFromNode(node, settingsDoc.Nodes[node.Name]);
                else if (!node.IsAValue)
                    settingsDoc.AddChild(node);
            }

            return settingsDoc;
        }

        /// <summary>
        /// Converts the specified object to a <see cref="SettingsNode"/>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="nodeName">The node name.</param>
        /// <returns></returns>
        public static SettingsNode ConvertObjectToNode(object obj, string nodeName = "")
        {
            if (obj == null)
                return null;

            Type type = obj.GetType();

            if (String.IsNullOrEmpty(nodeName))
                nodeName = type.Name;

            if (obj is ISettings)
            {
                return ((ISettings)obj).ConvertToSettingsNode();
            }
            else
            {
                SettingsNode node = new SettingsNode(nodeName, "", type);
                GetNodes(obj, type, node);
                return node;
            }
        }

        /// <summary>
        /// Creates an object of the generic type from the specified <see cref="SettingsNode"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="node">The settings node.</param>
        /// <returns>The new object.</returns>
        public static T CreateObjectFromNode<T>(SettingsNode node) where T : new()
        {
            if (node == null)
                return default(T);

            T newObject = new T();

            UpdateObjectFromNode(newObject, node);

            return newObject;
        }

        /// <summary>
        /// Creates an object of the generic type from the specified <see cref="SettingsNode" />.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="node">The settings node.</param>
        /// <param name="args">The optional constructor arguments for this node object.</param>
        /// <returns>
        /// The new object.
        /// </returns>
        public static T CreateObjectFromNode<T>(SettingsNode node, params object[] args)
        {
            if (node == null)
                return default(T);

            Type type = typeof(T);

            return (T)CreateObjectFromNode(node, type, args);
        }

        /// <summary>
        /// Creates an object of the generic type from the specified <see cref="SettingsNode" />.
        /// </summary>
        /// <param name="node">The settings node.</param>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">The optional constructor arguments for this node object.</param>
        /// <returns>
        /// The new object.
        /// </returns>
        public static object CreateObjectFromNode(SettingsNode node, Type type, params object[] args)
        {
            if (node == null)
                return null;

            if (node.IsAValue && node.Value == null)
                return null;

            if (node.HasAValue && node.Value == null)
                return null;

            object newObject = null;

            if (type.IsArray)
                newObject = Array.CreateInstance(type.GetElementType(), node.ListLength);
            else
                newObject = Activator.CreateInstance(type, args);

            UpdateObjectFromNode(newObject, node);

            return newObject;
        }

        /// <summary>
        /// Updates an object from the specified <see cref="SettingsNode" />.
        /// </summary>
        /// <param name="settingsObject">The settings object.</param>
        /// <param name="node">The settings node.</param>
        public static void UpdateObjectFromNode(object settingsObject, SettingsNode node)
        {
            if (settingsObject == null || node == null)
                return;     //An updated null object is still null

            Type objectType = settingsObject.GetType();

            if (settingsObject is ISettings)
            {
                ((ISettings)settingsObject).UpdateFromSettingsNode(node);
            }
            else if (IsListType(objectType))
            {
                if (!objectType.IsGenericType)
                {
                    if (!objectType.BaseType.IsGenericType)
                        return;

                    objectType = objectType.BaseType;
                }

                Type[] typeArguments = objectType.GetGenericArguments();

                if (typeArguments.Length < 1)
                    return;

                Type elementType = typeArguments[0];

                IList list = settingsObject as IList;
                list.Clear();
                
                for (int i = 0; i < node.ListLength; i++)
                {
                    list.Add(CreateObjectFromNode(node[i], elementType));
                }
            }
            else
            {
                PropertyInfo[] propertyInfos = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                SetValues(settingsObject, propertyInfos, node.Nodes);
            }
        }

        private static void UpdateNodeFromNode(SettingsNode sourceNode, SettingsNode destinationNode)
        {
            if (sourceNode.IsList)
            {
                destinationNode.RemoveAllChildren();
                destinationNode.ListLength = sourceNode.ListLength;
            }

            foreach (SettingsNode node in sourceNode.Nodes)
            {
                if (destinationNode.HasAValue)
                    destinationNode.RemoveAllChildren();

                if (destinationNode.Nodes.ContainsName(node.Name))
                    UpdateNodeFromNode(node, destinationNode.Nodes[node.Name]);
                else
                    destinationNode.AddChild(node.Clone());
            }
        }

        /// <summary>
        /// Used internally to deeply convert the specified object and attach to the specified node.
        /// </summary>
        private static void GetNodes(object obj, Type type, SettingsNode node)
        {
            if (obj == null)
            {
                node.AddChild("");
                return;
            }

            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                node.AddChild(obj.ToString());
                return;
            }

            if (obj is ISettings)
            {
                SettingsNode objNode = ((ISettings)obj).ConvertToSettingsNode();
                node.Nodes = objNode.Nodes;
                return;
            }

            if (IsListType(type))
            {
                IList list = obj as IList;

                node.ListLength = list.Count;

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == null)
                    {
                        node.AddChild("");
                        continue;
                    }

                    Type itemType = list[i].GetType();

                    if (itemType.IsPrimitive || itemType.IsEnum || itemType == typeof(string))
                    {
                        node.AddChild(list[i].ToString());
                        continue;
                    }

                    SettingsNode itemNode = node.AddChild(itemType.Name + " " + (i + 1), "", itemType);

                    GetNodes(list[i], itemType, itemNode);
                }

                return;
            }

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in propertyInfos)
            {
                if (pi == null)
                    continue;

                ParameterInfo[] parInfo = pi.GetIndexParameters(); //if represents a array
                if (parInfo != null && parInfo.Length > 0)
                    continue;

                object value = pi.GetValue(obj, null);

                if (value == obj)
                    return;

                if (node.Level > 15)
                    return;

                string description = "";
                DescriptionAttribute[] da = pi.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];

                if (da.Length > 0)
                    description = da[0].Description;

                SettingsNode nextChild = node.AddChild(pi.Name, description, pi.PropertyType);
                GetNodes(value, pi.PropertyType, nextChild);
            }
        }

        /// <summary>
        /// Used internally to deeply load from the SettingsDocument.
        /// </summary>
        private static void SetValues(object parent, PropertyInfo[] properties, SettingsNodeList docNodes)
        {
            if (properties == null)
                return;

            if (parent == null)
                return;

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] == null)
                    continue;

                if (!docNodes.ContainsName(properties[i].Name))
                    continue;

                SettingsNode node = docNodes.GetNode(properties[i].Name);

                ParameterInfo[] parInfo = properties[i].GetIndexParameters(); //if represents a array
                if (parInfo != null && parInfo.Length > 0)
                    continue;

                Type propertyType = properties[i].PropertyType;

                if (propertyType.IsPrimitive || propertyType.IsEnum || propertyType == typeof(string))
                {
                    if (!node.HasAValue || !properties[i].CanWrite)
                        continue;

                    try
                    {
                        object value = ConvertValue(node.Value, propertyType);
                        properties[i].SetValue(parent, value, null);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to load setting: " + node.FullPath, ex);
                    }
                }

                if (propertyType.GetInterface("ISettings", true) != null)
                {
                    ISettings settingsObject = properties[i].GetValue(parent, null) as ISettings;

                    if (settingsObject == null)
                        settingsObject = Activator.CreateInstance(propertyType) as ISettings;

                    settingsObject.UpdateFromSettingsNode(node);
                    continue;
                }

                if (IsListType(propertyType))
                {
                    IList list = properties[i].GetValue(parent, null) as IList;

                    if (list is Array)
                    {
                        Type elementType = list.GetType().GetElementType();

                        if (list == null || list.Count == 0)
                            list = Array.CreateInstance(elementType, node.Nodes.Count);

                        for (int j = 0; j < Math.Min(list.Count, node.ListLength); j++)
                        {
                            if (list[j] == null)
                                list[j] = CreateObjectFromNode(node[j], elementType);
                            else
                                UpdateObjectFromNode(list[j], node[j]);
                        }

                        properties[i].SetValue(parent, list, null);
                    }
                    else
                    {
                        Type listType = list.GetType();

                        if (!listType.IsGenericType)
                        {
                            if (!listType.BaseType.IsGenericType)
                                continue;

                            listType = listType.BaseType;
                        }

                        Type[] typeArguments = listType.GetGenericArguments();

                        if (typeArguments.Length < 1)
                            continue;

                        Type elementType = typeArguments[0];

                        if (list == null)
                            list = Activator.CreateInstance(list.GetType()) as IList;

                        list.Clear();

                        foreach (SettingsNode subNode in node.Nodes)
                        {
                            object item = null;

                            if (elementType.IsPrimitive || elementType.IsEnum || elementType == typeof(string))
                                item = ConvertValue(subNode.Value, elementType);
                            else
                                item = CreateObjectFromNode(subNode, elementType);

                            list.Add(item);
                        }

                        properties[i].SetValue(parent, list, null);
                    }

                    continue;
                }

                PropertyInfo[] propertyInfos = properties[i].PropertyType.GetProperties(BindingFlags.Public |
                                                                                        BindingFlags.Instance);

                object child = properties[i].GetValue(parent, null);

                if (child == null && node != null)
                {
                    Type childType = null;

                    try
                    {
                        childType = properties[i].PropertyType;
                        child = Activator.CreateInstance(childType);
                        properties[i].SetValue(parent, child, null);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("Failed to load setting: {0}. Could not create new {1}. ", node.FullPath, childType), ex);
                    }
                }

                SetValues(child, propertyInfos, node.Nodes);
            }
        }

        /// <summary>
        /// Converts a given object to the specified type. If conversion is invalid an InvalidCastException is thrown.
        /// Used primarily to convert strings to primitives and enums.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ConvertValue(object value, Type type)
        {
            if (value == null)
                return null;

            if (type == null)
                throw new InvalidCastException("Invalid conversion: Type is not specified.");

            try
            {
                if (type.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(type, (string)value, true);
                    else
                        return Enum.ToObject(type, value);
                }

                return Convert.ChangeType(value, type);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidCastException("Invalid conversion between the value-type arguments.", ex);
            }
        }

        /// <summary>
        /// Copies the properties of the specified object to the output object.
        /// </summary>
        /// <param name="copyFrom">The copy from.</param>
        /// <param name="copyTo">The copy to.</param>
        public static void CopyConfiguration(object copyFrom, ref object copyTo)
        {
            CopyConfiguration(copyFrom, ref copyTo, 0);
        }

        private static void CopyConfiguration(object copyFrom, ref object copyTo, int level)
        {
            if (copyFrom == null)
            {
                copyTo = null;
                return;
            }

            if (copyTo == null)
            {
                copyTo = DeepClone(copyFrom);
                return;
            }

            Type copyType = copyFrom.GetType();
            PropertyInfo[] propertyInfos = copyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in propertyInfos)
            {
                Type pType = pi.PropertyType;
                object fromValue = null;

                ParameterInfo[] parInfo = pi.GetIndexParameters(); //if represents a array
                if (parInfo != null && parInfo.Length > 0)
                    continue;
                else
                    fromValue = pi.GetValue(copyFrom, null);

                if (pType.IsPrimitive || pType.IsValueType || pType == typeof(string)) //enums are value types too
                {
                    if (!pi.CanWrite)
                        continue;

                    pi.SetValue(copyTo, fromValue, null); //just copy value
                    continue;
                }

                //go deeper
                object toValue = pi.GetValue(copyTo, null);
                    
                if (IsListType(pType))
                {
                    IList toList = (IList)toValue;
                    IList fromList = (IList)fromValue;

                    if (fromValue is Array)
                    {
                        if (toList == null)
                            toList = Array.CreateInstance(fromValue.GetType().GetElementType(), fromList.Count);

                        for (int i = 0; i < fromList.Count; i++)
                            toList[i] = DeepClone(fromList[i]);
                    }
                    else
                    {
                        if (toList == null)
                            toList = Activator.CreateInstance(fromValue.GetType()) as IList;

                        toList.Clear();

                        foreach (object value in fromList)
                            toList.Add(DeepClone(value));
                    }
                }
                else
                {
                    if (level < 10)
                    {
                        bool appendToParent = (toValue == null);

                        CopyConfiguration(fromValue, ref toValue, level + 1);

                        if (pi.CanWrite && appendToParent)
                            pi.SetValue(copyTo, toValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a deep clone of the specified object based on ICloneable, serialization, or public properties.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static object DeepClone(object obj)
        {
            if (obj == null)
                return null;

            Type type = obj.GetType();

            if (obj is ICloneable)
                return ((ICloneable)obj).Clone();

            if (type.IsPrimitive || type.IsValueType)
                return obj;

            if (type.IsSerializable)
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    ms.Position = 0;

                    return formatter.Deserialize(ms);
                }
            }
            else
            {
                object clone = Activator.CreateInstance(type);
                CopyConfiguration(obj, ref clone);

                return clone;
            }
        }

        /// <summary>
        /// Determines whether the specified type is a list type. This includes generic lists and generic arrays.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> ifthe specified type is a list type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsListType(Type type)
        {
            if (type == null)
                return false;

            if (type == typeof(IList))
                return true;

            if (type.IsGenericType)
                if (type.GetGenericTypeDefinition() == typeof(IList<>))
                    return true;

            foreach (Type iface in type.GetInterfaces())
            {
                if (String.Equals(iface.Name, "IList", StringComparison.CurrentCultureIgnoreCase))
                    return true;

                if (iface.IsGenericType)
                    if (iface.GetGenericTypeDefinition() == typeof(IList<>))
                        return true;
            }

            return false;
        }
    }
}
