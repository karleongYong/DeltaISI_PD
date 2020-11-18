using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace XyratexOSC.Settings.UI
{
    /// <summary>
    /// Views and allows to edit all device settings in the system
    /// </summary>
    public partial class 
        PropertyGridEditor : SettingsEditor
    {   
        private object _editableObject;

        /// <summary>
        /// Occurs when a value changed in the property grid.
        /// </summary>
        public event PropertyValueChangedEventHandler PropertyGridValueChanged;
        public event EventHandler OperationModeChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridEditor"/> class.
        /// </summary>
        public PropertyGridEditor()
            : this(null)
        {
            //For the VS designer
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridEditor"/> class.
        /// </summary>
        /// <param name="settingsObject">The settings object.</param>
        /// <param name="constructionArgs">The construction args.</param>
        public PropertyGridEditor(object settingsObject, params object[] constructionArgs) 
            : base()
        {
            InitializeComponent();
            IsDirty = true;

            SetSettingsObject(settingsObject);
        }

        /// <summary>
        /// Creates a clone of the settings object and assigns it to the property grid.
        /// </summary>
        /// <param name="settingsObject">The settings object.</param>
        public override void SetSettingsObject(object settingsObject)
        {
            SetSettingsObject(settingsObject, null);
        }

        /// <summary>
        /// Creates a clone of the settings object and assigns it to the property grid.
        /// </summary>
        /// <param name="settingsObject">The settings object.</param>
        /// <param name="constructionArgs">The construction arguments.</param>
        public void SetSettingsObject(object settingsObject, params object[] constructionArgs)
        {
            _CfgView.PropertyValueChanged -= _CfgView_PropertyGridValueChanged;

            if (settingsObject == null)
            {
                Clear();
                return;
            }

            if (settingsObject == _settingsObject)
                return;

            _settingsObject = settingsObject;
            _editableObject = Activator.CreateInstance(settingsObject.GetType(), constructionArgs);
            
            RefreshSettings();

            _CfgView.PropertyValueChanged += _CfgView_PropertyGridValueChanged;
        }

        public void Refresh()
        {
            _CfgView.Refresh();
        }

        private void DisableReadonly(object value)
        {
            if (value == null)
                return;

            Type type = value.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in propertyInfos)
            {
                Type pType = pi.PropertyType;

                ParameterInfo[] parInfo = pi.GetIndexParameters(); //if represents a array
                if (parInfo != null && parInfo.Length > 0)
                    continue;

                //Set Read Only
                ReadOnlyAttribute[] attribute2 = pi.GetCustomAttributes(typeof(ReadOnlyAttribute), false) as ReadOnlyAttribute[];
                if (attribute2 != null && attribute2.Length > 0)
                {
                    FieldInfo ftoChange = attribute2[0].GetType().GetField("isReadOnly",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.Instance);
                    if (ftoChange != null)
                        ftoChange.SetValue(attribute2[0], false);
                }

                if (pType.IsPrimitive || pType.IsValueType || pType == typeof(string)) //enums are value types too
                {
                    return;
                }
                else
                {
                    //go deeper
                    object pValue = pi.GetValue(value, null);

                    if (SettingsConverter.IsListType(pType))
                    {
                        IList pList = (IList)pValue;

                        foreach (object pListValue in pList)
                            DisableReadonly(pList);
                    }
                    else
                    {
                        DisableReadonly(pValue);
                    }
                }
            }
        }

        /// <summary>
        /// Called when settings are refreshed.
        /// </summary>
        protected override void OnRefreshSettings()
        {
            if (_settingsObject == null)
                return;

            SettingsConverter.CopyConfiguration(_settingsObject, ref _editableObject);
            DisableReadonly(_editableObject);

            int selectedListViewIndex = -1;

            if (_cfgListView.SelectedIndices.Count > 0)
                selectedListViewIndex = _cfgListView.SelectedIndices[0];

            Clear();

            if (_settingsObject == null)
                return;

            PropertyInfo[] propertyInfos = _editableObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<KeyValuePair<string, object>> selectableList = new List<KeyValuePair<string, object>>();

            foreach (PropertyInfo t in propertyInfos)
            {
                if (t == null)
                    continue;

                if (t.PropertyType.IsPrimitive || t.PropertyType.IsValueType || t.PropertyType == typeof(string))
                    continue;

                object value = t.GetValue(_editableObject, null);

                if (value == null)
                    continue;

                selectableList.Add(new KeyValuePair<string, object>(t.Name, value));
            }

            UIUtility.Invoke(this, () =>
            {
                btnSave.Enabled = true;

                foreach (KeyValuePair<string, object> kv in selectableList)
                    _cfgListView.Items.Add(CreateListViewItem(kv.Key, kv.Value));

                if (selectedListViewIndex > -1)
                    _cfgListView.Items[selectedListViewIndex].Selected = true;
            });
        }

        private void Clear()
        {
            UIUtility.Invoke(this, () =>
            {
                _cfgListView.Items.Clear();
                _CfgView.SelectedObject = null;
                btnSave.Enabled = false;
            });
        }

        private ListViewItem CreateListViewItem(string name, object tag)
        {
            int iconIndex = 0;
            ListViewItem lvi = new ListViewItem(name, iconIndex);
            lvi.Name = tag.GetHashCode().ToString();
            lvi.Tag = tag;
            return lvi;
        }

        private void _CfgView_PropertyGridValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            _CfgView.Refresh();

            PropertyGrid selectedItem = sender as PropertyGrid;

            if (selectedItem.SelectedGridItem.Label.Equals("Operation Mode"))
            {
                string newValue = selectedItem.SelectedGridItem.Value.ToString();                
                OperationModeChanged.SafeInvoke(this, new OperationModeEventArgs(newValue));                
            }
            if (PropertyGridValueChanged != null)
            {
                PropertyGridValueChanged(this, e);
            }
            
        }

        /*
        private void CheckChangedItemDifference(GridItem item)
        {
            Stack<string> names = new Stack<string>();
            GridItem gi = item;

            while(gi != null)
            {
                names.Push(gi.PropertyDescriptor.Name);
                gi = gi.Parent;
            }
            names.Push(_cfgListView.SelectedItems[0].Text);

            object obj = _settingsObject;
            while (names.Count > 0)
            {
                if (obj == null)
                    return;

                PropertyInfo[] propertyInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo pi = Array.Find<PropertyInfo>(propertyInfos, p => p.Name == names.Peek());

                if (pi == null)
                    return;

                obj = pi.GetValue(obj, null);
            }
        }
         */

        /// <summary>
        /// Saves settings using settings document.
        /// </summary>
        protected override void OnSave()
        {
            SettingsDocument = SettingsConverter.ConvertObjectToDocument(_settingsObject, FilePath);
            base.OnSave();
        }

        /// <summary>
        /// Loads settings from settings document.
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();
            SettingsConverter.UpdateObjectFromNode(_settingsObject, SettingsDocument);
        }

        /// <summary>
        /// Called when accepting the settings changes to the original settings object.
        /// </summary>
        protected override void OnAccept()
        {
            SettingsConverter.CopyConfiguration(_editableObject, ref _settingsObject);
            _CfgView.Refresh();
        }

        /// <summary>
        /// Restores settings prior to firing the cancelled event.
        /// </summary>
        protected override void OnCancel()
        {
            SettingsConverter.CopyConfiguration(_settingsObject, ref _editableObject);
            _CfgView.Refresh();
        }

        private void _cfgListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.Item != null && e.Item.Tag != null && e.Item.Selected)
                {
                    _CfgView.SelectedObject = e.Item.Tag;

                    GridItem root = _CfgView.SelectedGridItem;
                    while (root.Parent != null)
                        root = root.Parent;

                    foreach (GridItem category in root.GridItems)
                    {
                        category.Expanded = true;

                        foreach (GridItem item in category.GridItems)
                            item.Expanded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error");
            }
        }

        public ImageList getImageList()
        {
            return _imageList;
        }

        public PropertyGrid getConfigView()
        {
            return _CfgView;
        }

        public ListView getConfigListView()
        {
            return _cfgListView;
        }
    }
}
