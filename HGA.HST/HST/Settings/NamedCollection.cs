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
using XyratexOSC;

namespace Seagate.AAS.HGA.HST.Settings
{
    [Serializable]
    [TypeConverter(typeof(NamedCollectionConverter))]
    public class NamedCollection<T> : Collection<T>, ICustomTypeDescriptor where T : INamed
    {
        public new T this[int index] 
        { 
            get
            {
                if (index < 0 || index >= Items.Count)
                    return default(T);

                return this.Items[index];
            }
        }

        public T this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    return default(T);

                return this.Items.FirstOrDefault(item => String.Equals(item.Name, name));
            }
        }

        public event EventHandler CollectionChanged;

        /// <summary>
        /// Gets the names of all of the items in this collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetNames()
        {
            return this.Items.Select(item => item.Name);
        }

        protected override void InsertItem(int index, T item)
        {
            //Instead of throwing an exception when an item of the same name is added, 
            //we change the name of the new item (for adding in the property grid)
            if (this.Contains(item.Name))
                item.Name = String.Format("{0} {1}", item.GetType().Name, this.Items.Count + 1);

            base.InsertItem(index, item);

            EventHandler collectionChanged = CollectionChanged;
            if (collectionChanged != null)
                collectionChanged(this, new EventArgs());
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            EventHandler collectionChanged = CollectionChanged;
            if (collectionChanged != null)
                collectionChanged(this, new EventArgs());
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
            return this.Items.Any(item => String.Equals(item.Name, name));
        }

        #region ICustomTypeDescriptor Implementation

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            // Create a new collection object PropertyDescriptorCollection
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            for (int i = 0; i < this.Items.Count; i++)
            {
                NamedCollectionPropertyDescriptor<T> pd = new NamedCollectionPropertyDescriptor<T>(this, i);
                pds.Add(pd);
            }

            return pds;
        }

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        
        #endregion
    }

    #region NamedCollectionConverter

    public class NamedCollectionConverter : ExpandableObjectConverter
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

    #region NamedCollectionPropertyDescriptor

    public class NamedCollectionPropertyDescriptor<T> : PropertyDescriptor where T : INamed
    {
        private NamedCollection<T> _collection = null;
        private int _index = -1;

        public NamedCollectionPropertyDescriptor(NamedCollection<T> collection, int index) 
            : base( "#"+index.ToString(), null )
        {
            this._collection = collection;
            this._index = index;
        }

        public override AttributeCollection Attributes
        {
            get 
            { 
                return new AttributeCollection(null);
            }
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get 
            { 
                return this._collection.GetType();
            }
        }

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

        public override string Description
        {
            get
            {
                return "";
            }
        }

        public override object GetValue(object component)
        {
            return this._collection[_index];
        }

        public override bool IsReadOnly
        {
            get { return true;  }
        }

        public override string Name
        {
            get { return "#"+_index.ToString(); }
        }

        public override Type PropertyType
        {
            get { return this._collection[_index].GetType(); }
        }

        public override void ResetValue(object component) {}

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override void SetValue(object component, object value)
        {
            // this.collection[index] = value;
        }
    }

    #endregion

    #region NamedCollectionEditor

    public class NamedCollectionEditor<T> : CollectionEditor where T : INamed
    {
        public static EventHandler CollectionChanged;

        // Inherit the default constructor from CollectionEditor 
        public NamedCollectionEditor(Type type)
            : base(type)
        {
        }

        // Override this method in order to access the containing user controls 
        // from the default Collection Editor form or to add new ones... 
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();
            collectionForm.Height = 580;
            collectionForm.Width = 700;

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

                TableLayoutPanel buttonPanel = mainPanel.Controls[6]
                    as TableLayoutPanel;
                if ((buttonPanel != null) && (buttonPanel.Controls.Count > 1))
                {
                    // Get a reference to the Ok button and hook  
                    // an event handler to it. 
                    Button okayButton = buttonPanel.Controls[0] as Button;
                    if (okayButton != null)
                    {
                        okayButton.Click += new EventHandler(OnCollectionChanged); //+= new EventHandler(okayButton_Click);
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

            NamedCollection<T> items = collectionForm.EditValue as NamedCollection<T>;
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

        private static void okayButton_Click(object sender, EventArgs e)
        {
            Button addButton = (Button)sender; 
        }

        private static void OnCollectionChanged(object sender, EventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }
    } 

    #endregion
}
