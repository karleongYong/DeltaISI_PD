using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A control for editing a list of items including control over each item's properties.
    /// </summary>
    public partial class ListPanel : UserControl
    {
        private int _snapToTop = 0;
        private Point _autoScrollPosition;
        private IList _list;
        private Image _itemIcon;
        private IEnumerable<Control> _prevSortedControls;

        /// <summary>
        /// Gets or sets the icon to be used for each item.
        /// </summary>
        /// <value>
        /// The item icon.
        /// </value>
        public Image ItemIcon
        {
            get
            {
                return _itemIcon;
            }
            set
            {
                if (_itemIcon == value)
                    return;

                _itemIcon = value;

                foreach (ListPanelItem control in Panel.Controls.OfType<ListPanelItem>())
                    control.Icon = _itemIcon;
            }
        }

        /// <summary>
        /// Gets or sets the list of objects to display.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public IList List
        {
            get
            {
                if (_list == null)
                    return null;

                _list.Clear();

                foreach (ListPanelItem control in Panel.Controls.OfType<ListPanelItem>().OrderBy(c => c.Top))
                    _list.Add(control.Item);

                return _list;
            }
            set
            {
                _list = value;

                btnAdd.Enabled = (_list != null);

                Panel.Controls.Clear();
                btnAdd.Location = new Point(btnAdd.Margin.Left, btnAdd.Margin.Right);
                Panel.Controls.Add(btnAdd);

                if (_list != null)
                    foreach (object item in _list)
                        AddItem(item);

                OnListChanged();
            }
        }

        /// <summary>
        /// Occurs when the list changed.
        /// </summary>
        public event EventHandler ListChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListPanel"/> class.
        /// </summary>
        public ListPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.ClientSizeChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            this.Panel.Width = this.ClientSize.Width - this.scrollBar.Width;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_list == null)
                return;

            try
            {
                Type listType = _list.GetType();
                Type elementType = null;

                if (listType.IsGenericType)
                {
                    Type[] typeArguments = listType.GetGenericArguments();

                    if (typeArguments.Length > 0)
                        elementType = typeArguments[0];
                }
                else if (listType.BaseType.IsGenericType)
                {
                    Type[] typeArguments = listType.BaseType.GetGenericArguments();

                    if (typeArguments.Length > 0)
                        elementType = typeArguments[0];
                }
                else
                {
                    elementType = listType.GetElementType();
                }

                AddItem(Activator.CreateInstance(elementType));

                OnListChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to Add New Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddItem(object item)
        {
            ListPanelItem control = new ListPanelItem(item);

            control.Expand();
            control.Icon = _itemIcon;
            control.Left = control.Margin.Left;
            control.Top = btnAdd.Top - btnAdd.Margin.Top + control.Margin.Top;
            control.Width = Panel.Width - control.Margin.Horizontal;
            control.StartMove += ControlStartingMove;
            control.EndMove += ControlEndMove;
            control.LocationChanging += ControlLocationChanging;
            control.LocationChanged += ControlLocationChanged;
            control.SizeChanged += ControlSizeChanged;
            control.PropertyChanged += ControlPropertyChanged;

            Panel.Controls.Add(control);

            control.Focus();
        }

        private void ControlPropertyChanged(object sender, EventArgs e)
        {
            OnListChanged();
        }

        private void ControlSizeChanged(object sender, EventArgs e)
        {
            int totalHeight = 0;
            int nextTop = 0;            
            IOrderedEnumerable<Control> SortedControls = Panel.Controls.OfType<Control>().OrderBy(control => control.Top);

            Panel.SuspendLayout();

            foreach (Control control in SortedControls)
            {
                control.Top = nextTop + control.Margin.Top;
                nextTop = control.Bottom + control.Margin.Bottom;

                totalHeight += control.Height + control.Margin.Vertical;
            }

            Panel.Height = totalHeight;

            Panel.ResumeLayout();
        }

        private void ControlEndMove(object sender, EventArgs e)
        {
            int nextTop = 0;
            IOrderedEnumerable<Control> SortedControls = Panel.Controls.OfType<Control>().OrderBy(control => control.Top);

            Panel.SuspendLayout();

            foreach (Control control in SortedControls)
            {
                control.Top = nextTop + control.Margin.Top;
                nextTop = control.Bottom + control.Margin.Bottom;
            }

            Panel.ResumeLayout();

            if (_prevSortedControls == null || _prevSortedControls.Count() != SortedControls.Count())
            {
                OnListChanged();
                return;
            }

            List<Control> prevSortedControls = _prevSortedControls.ToList();
            List<Control> sortedControls = SortedControls.ToList();

            for (int i = 0; i < sortedControls.Count; i++)
            {
                if (prevSortedControls[i] == sortedControls[i])
                {
                    OnListChanged();
                    return;
                }
            }
        }

        private void OnListChanged()
        {
            EventHandler listChanged = ListChanged;
            if (listChanged != null)
                listChanged(this, new EventArgs());
        }

        private void ControlStartingMove(object sender, EventArgs e)
        {
            Control movingControl = sender as Control;
            if (movingControl == null)
                return;

            _prevSortedControls = Panel.Controls.OfType<Control>().OrderBy(control => control.Top);

            _snapToTop = movingControl.Top;
            _autoScrollPosition = this.AutoScrollPosition;
        }

        private void ControlLocationChanging(object sender, LocationChangingEventArgs e)
        {
            Control movingControl = sender as Control;
            if (movingControl == null)
                return;

            if (e.NewLocation.Y - movingControl.Margin.Top < 0)
                e.NewLocation = new Point(e.NewLocation.X, movingControl.Margin.Top);
            else if (e.NewLocation.Y + movingControl.Height + movingControl.Margin.Bottom > btnAdd.Top - btnAdd.Margin.Top)
                e.NewLocation = new Point(e.NewLocation.X, btnAdd.Top - btnAdd.Margin.Top - movingControl.Height - movingControl.Margin.Bottom);

            if (Panel.Height <= this.Height)
                return;

            int viewDelta = -Panel.Location.Y - e.NewLocation.Y;

            if (e.NewLocation.Y < -Panel.Location.Y)
                AutoScrollPosition = new Point(_autoScrollPosition.X, e.NewLocation.Y + viewDelta);
        }

        private void ControlLocationChanged(object sender, EventArgs e)
        {
            ListPanelItem movingControl = sender as ListPanelItem;
            if (movingControl == null || !movingControl.Moving)
                return;

            IOrderedEnumerable<Control> SortedControls = Panel.Controls.OfType<Control>().OrderBy(control => control.Top);

            Panel.SuspendLayout();

            foreach (Control control in SortedControls)
            {
                if (control == movingControl)
                    continue;

                int controlMiddle = control.Top + (control.Height / 2);

                if (control.Top < _snapToTop)
                {
                    if (controlMiddle > movingControl.Top)
                    {
                        if (control.Top < _snapToTop)
                            _snapToTop = control.Top;

                        control.Top += (movingControl.Height + movingControl.Margin.Bottom);
                    }
                }
                else if (control.Top > _snapToTop + movingControl.Height)
                {
                    if (controlMiddle < movingControl.Bottom)
                    {
                        control.Top = _snapToTop;
                        _snapToTop = control.Bottom + control.Margin.Bottom + movingControl.Margin.Top;
                    }
                }
            }

            Panel.ResumeLayout();
        }
        
        private void Panel_ControlAdded(object sender, ControlEventArgs e)
        {
            bool moveEnabled = (Panel.Controls.Count != 2);

            Panel.SuspendLayout();

            int totalHeight = 0;
            foreach (Control control in Panel.Controls)
            {
                totalHeight += control.Height + control.Margin.Vertical;

                if (control == e.Control)
                    continue;

                if (control.Top >= e.Control.Top)
                    control.Top += e.Control.Height + e.Control.Margin.Vertical;
            }

            Panel.Height = totalHeight;

            Panel.ResumeLayout();
        }

        private void Panel_ControlRemoved(object sender, ControlEventArgs e)
        {
            int oldTop = e.Control.Top;
            int oldHeight = e.Control.Height + e.Control.Margin.Vertical;
            bool moveEnabled = (Panel.Controls.Count != 2);

            Panel.SuspendLayout();

            int totalHeight = 0;
            foreach (Control control in Panel.Controls)
            {
                if (control.Top > oldTop)
                    control.Top -= oldHeight;

                totalHeight += control.Height + control.Margin.Vertical;
            }

            Panel.Height = totalHeight;

            Panel.ResumeLayout();

            OnListChanged();
        }

        private void Panel_ClientSizeChanged(object sender, EventArgs e)
        {
            Panel.SuspendLayout();

            foreach (Control control in Panel.Controls)
            {
                if (control == btnAdd)
                    continue;

                control.Width = Panel.Width - control.Margin.Horizontal;
            }

            if (Panel.Height > this.Height)
            {
                scrollBar.Enabled = true;
                scrollBar.Minimum = 0;
                scrollBar.SmallChange = this.Height / 20;
                scrollBar.LargeChange = this.Height / 10;

                scrollBar.Maximum = Panel.Height - this.Height + scrollBar.LargeChange;

                if (-Panel.Top > scrollBar.Maximum)
                    Panel.Top = -scrollBar.Maximum;
                else if (-Panel.Top < 0)
                    Panel.Top = 0;

                scrollBar.Value = -Panel.Top;
            }
            else
            {
                Panel.Top = 0;
                scrollBar.Enabled = false;
            }

            Panel.ResumeLayout();
        }

        private void scrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Panel.Top = -scrollBar.Value;
        }

        private void ListPanel_Resize(object sender, EventArgs e)
        {
            Panel_ClientSizeChanged(sender, e);
        }
    }
}
