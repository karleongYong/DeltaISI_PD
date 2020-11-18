using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC
{
    /// <summary>
    /// Provides a generic list of named objects that has both indexed access and named indexing.
    /// Can be used as an ordered, but un-hashed dictionary.
    /// </summary>
    /// <typeparam name="T">Named type.</typeparam>
    [TypeConverter(typeof(NamedListConverter))]
    public class NamedList<T> : Collection<T>, INamedList<T>, ICustomTypeDescriptor where T : INamed
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public new T this[int index]
        { 
            get
            {
                if (index < 0 || index >= Items.Count)
                    return default(T);

                return this.Items[index];
            }
            set
            {
                this.Items[index] = value;

                EventHandler listChanged = ListChanged;
                if (listChanged != null)
                    listChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    return default(T);

                return this.Items.FirstOrDefault(item => String.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Occurs when the list has changed.
        /// </summary>
        public event EventHandler ListChanged;
        
        /// <summary>
        /// Gets the names of all of the items in this collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetNames()
        {
            return this.Items.Select(item => item.Name);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="T:INamedList`1" />.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="T:INamedList`1" />. The collection itself cannot be null, but it can contain elements that are null, if type <c>T</c> is a reference type.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            this.InsertRange(this.Count, collection);
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="T:NamedList`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="T:System.Collections.Generic.List`1"/>. The collection itself cannot be null, but it can contain elements that are null, if type <c>T</c> is a reference type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        ///                     -or-
        /// <paramref name="index"/> is greater than <see cref="P:NamedList`1.Count"/>.
        /// </exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if ((uint)index > (uint)this.Count)
                throw new ArgumentOutOfRangeException("index", "index is greater than NamedList.Count");

            List<T> list = Items as List<T>;
            if (list != null)
            {
                list.InsertRange(index, collection);
            }
            else
            {
                foreach (T obj in collection)
                    this.Insert(index++, obj);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, T item)
        {
            //Instead of throwing an exception when an item of the same name is added, 
            //we change the name of the new item (for adding in the property grid)
            if (this.Contains(item.Name))
                item.Name = String.Format("{0} {1}", item.GetType().Name, this.Items.Count + 1);

            base.InsertItem(index, item);

            EventHandler listChanged = ListChanged;
            if (listChanged != null)
                listChanged(this, new EventArgs());
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            EventHandler listChanged = ListChanged;
            if (listChanged != null)
                listChanged(this, new EventArgs());
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.Collection`1"/> contains a specific name.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <returns>
        ///   <c>true</c> if the specified name is found; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            return this.Items.Any(item => String.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        #region ICustomTypeDescriptor Implementation

        /// <summary>
        /// Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the filtered properties for this component instance.
        /// </returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        /// <summary>
        /// Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties for this component instance.
        /// </returns>
        public PropertyDescriptorCollection GetProperties()
        {
            // Create a new collection object PropertyDescriptorCollection
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < this.Items.Count; i++)
            {
                NamedListPropertyDescriptor<T> pd = new NamedListPropertyDescriptor<T>(this, i);
                pds.Add(pd);
            }

            return pds;
        }

        /// <summary>
        /// Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>
        /// The class name of the object, or null if the class does not have a name.
        /// </returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for this object.
        /// </returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Returns the name of this instance of a component.
        /// </summary>
        /// <returns>
        /// The name of the object, or null if the object does not have a name.
        /// </returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter" /> that is the converter for this object, or null if there is no <see cref="T:System.ComponentModel.TypeConverter" /> for this object.
        /// </returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptor" /> that represents the default event for this object, or null if this object does not have events.
        /// </returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the default property for this object, or null if this object does not have properties.
        /// </returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the editor for this object.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> of the specified type that is the editor for this object, or null if the editor cannot be found.
        /// </returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the filtered events for this component instance.
        /// </returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Returns the events for this instance of a component.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the events for this component instance.
        /// </returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the property whose owner is to be found.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the owner of the specified property.
        /// </returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        
        #endregion
    }

    #region NamedListConverter

    /// <summary>
    /// Provides a type converter explicitly indicate the button available for editing the items contained in a <see cref="NamedList{T}"/>
    /// </summary>
    public class NamedListConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(string) == destinationType;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string) == destinationType)
            {
                //CustomCollection<T> ftc = value as CustomCollection<T>;
                if (value != null)
                    return "Click to add/edit ->";
            }

            return "(none)";
        }
    }

    #endregion

    #region NamedListPropertyDescriptor

    /// <summary>
    /// Provides an abstraction of a property on a named list item.
    /// </summary>
    /// <typeparam name="T">Named type.</typeparam>
    public class NamedListPropertyDescriptor<T> : PropertyDescriptor where T : INamed
    {
        private NamedList<T> _collection = null;
        private int _index = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedListPropertyDescriptor{T}"/> class.
        /// </summary>
        /// <param name="list">The named list.</param>
        /// <param name="index">The index.</param>
        public NamedListPropertyDescriptor(NamedList<T> list, int index) 
            : base( "#"+index.ToString(), null )
        {
            this._collection = list;
            this._index = index;
        }

        /// <summary>
        /// Gets the collection of attributes for this member.
        /// </summary>
        public override AttributeCollection Attributes
        {
            get 
            { 
                return new AttributeCollection(null);
            }
        }

        /// <summary>
        /// Returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>
        /// true if resetting the component changes its value; otherwise, false.
        /// </returns>
        public override bool CanResetValue(object component)
        {
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get 
            { 
                return this._collection.GetType();
            }
        }

        /// <summary>
        /// Gets the name that can be displayed in a window, such as a Properties window. Handles removed items names.
        /// </summary>
        public override string DisplayName
        {
            get 
            {
                if (_index < _collection.Count)
                    return this._collection[_index].Name;
                else
                    return "(removed)";
            }
        }

        /// <summary>
        /// Gets the description of the member, as specified in the <see cref="T:System.ComponentModel.DescriptionAttribute" />.
        /// </summary>
        public override string Description
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            return this._collection[_index];
        }

        /// <summary>
        /// Gets a value indicating whether this property is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return true;  }
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public override string Name
        {
            get { return "#"+_index.ToString(); }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get { return this._collection[_index].GetType(); }
        }

        /// <summary>
        /// Resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component) {}

        /// <summary>
        /// Determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        /// <summary>
        /// Sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            // this.collection[index] = value;
        }
    }

    #endregion

    #region NamedListEditor

    /// <summary>
    /// Provides a user interface that can edit a <see cref="NamedList{T}"/> collection at design time. 
    /// The editor will display each item's name instead of their generic type in the properties window.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NamedListEditor<T> : CollectionEditor where T : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedListEditor{T}"/> class.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public NamedListEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.CollectionEditor.CollectionForm" /> to provide as the user interface for editing the collection.
        /// </returns>
        /// <remarks>
        /// Override this method in order to access the containing user controls 
        /// from the default Collection Editor form or to add new ones... 
        /// </remarks>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();

            collectionForm.Text = String.Format("Add or Remove {0}s...", typeof(T).Name);

            if (collectionForm.Controls.Count > 0)
            {
                TableLayoutPanel mainPanel = collectionForm.Controls[0]
                    as TableLayoutPanel;
                if ((mainPanel != null) && (mainPanel.Controls.Count > 7))
                {
                    // Get a reference to the inner PropertyGrid and hook  
                    // an event handler to it. 
                    PropertyGrid propertyGrid = mainPanel.Controls[5]
                        as PropertyGrid;
                    if (propertyGrid != null)
                    {
                        propertyGrid.PropertyValueChanged +=
                            new PropertyValueChangedEventHandler(
                                propertyGrid_PropertyValueChanged);
                    }
                }
            }
            return collectionForm;
        }

        private static void propertyGrid_PropertyValueChanged(object sender,
            PropertyValueChangedEventArgs e)
        {
            PropertyGrid grid = sender as PropertyGrid;
            CollectionForm collectionForm = (CollectionForm)grid.Parent.Parent;

            NamedList<T> items = collectionForm.EditValue as NamedList<T>;
            GridItem changedGridItem = e.ChangedItem;

            while (changedGridItem.GridItemType != GridItemType.Root)
                changedGridItem = changedGridItem.Parent;

            T changedItem = (T)changedGridItem.Value;

            if (e.ChangedItem.PropertyDescriptor.Name == "Name")
            {
                foreach (T item in items)
                {
                    if (item.Equals(changedItem))
                        continue;

                    if (String.Equals(item.Name, changedItem.Name))
                        changedItem.Name = e.OldValue as string;
                }
            }
        }
    } 

    #endregion

    #region NamedListExtensions

    /// <summary>
    /// Provides an extension method to any enumerable collection of <see cref="INamed"/> objects to convert to a <see cref="NamedList{T}"/>
    /// </summary>
    public static class NamedListExtensions
    {
        /// <summary>
        /// Creates a <see cref="NamedList{T}"/> from this enumeration.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static NamedList<TSource> ToNamedList<TSource>(this IEnumerable<TSource> source) where TSource : INamed
        {
            NamedList<TSource> collection = new NamedList<TSource>();
            
            foreach (TSource item in source)
            {
                collection.Add(item);
            }

            return collection;
        }
    }

    #endregion
}
