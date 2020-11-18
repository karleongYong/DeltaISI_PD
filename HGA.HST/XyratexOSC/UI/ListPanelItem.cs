using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.UI;

namespace XyratexOSC.UI
{
    /// <summary>
    /// An item displayed in a list panel control.
    /// </summary>
    public partial class ListPanelItem : UserControl
    {
        private int _startingResize;
        private bool _resizing = false;

        private bool _moveStarted = false;
        private bool _mouseDown = false;
        private int _relativeToScreen = 0;
        private int _lastY;

        private int _propertyGridHeight = 120;

        /// <summary>
        /// Gets or sets the item to display.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public object Item
        {
            get
            {
                return propertyGrid.SelectedObject;
            }
            set
            {
                propertyGrid.SelectedObject = value;
                propertyGrid.ExpandAllGridItems();
            }
        }

        /// <summary>
        /// Gets or sets the icon representing this item.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public Image Icon
        {
            get
            {
                return btnMove.BackgroundImage;
            }
            set
            {
                UIUtility.Invoke(this, () => btnMove.BackgroundImage = value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ListPanelItem"/> is moving.
        /// </summary>
        /// <value>
        ///   <c>true</c> if moving; otherwise, <c>false</c>.
        /// </value>
        public bool Moving
        {
            get;
            private set;
        }

        /// <summary>
        /// Occurs when a property of this <see cref="Item"/> has changed.
        /// </summary>
        public event EventHandler PropertyChanged;

        /// <summary>
        /// Occurs when this item begins a move via mouse drag.
        /// </summary>
        public event EventHandler StartMove;

        /// <summary>
        /// Occurs when this item has been released.
        /// </summary>
        public event EventHandler EndMove;

        /// <summary>
        /// Occurs when this item's location is changing in the parent list.
        /// </summary>
        public event EventHandler<LocationChangingEventArgs> LocationChanging;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListPanelItem"/> class.
        /// </summary>
        public ListPanelItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListPanelItem"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ListPanelItem(object item)
            : this()
        {
            Item = item;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.Parent.Controls.Remove(this);
            this.Dispose();
        }

        private void btnMove_MouseDown(object sender, MouseEventArgs e)
        {
            _moveStarted = false;
            _mouseDown = true;
            _lastY = e.Y;

            Cursor = Cursors.SizeAll;

            EventHandler startingMove = StartMove;

            if (startingMove != null)
                startingMove(this, new EventArgs());
        }

        private void btnMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown && !Moving)
            {
                this.BringToFront();
                _relativeToScreen = this.Top - this.PointToScreen(new Point(e.X, e.Y)).Y;

                Moving = true;
                _moveStarted = true;
            }

            if (Moving)
            {
                Point newLoc = new Point(this.Left, this.PointToScreen(new Point(e.X, e.Y)).Y + _relativeToScreen);

                EventHandler<LocationChangingEventArgs> locationChanging = LocationChanging;
                LocationChangingEventArgs locationArgs = new LocationChangingEventArgs(newLoc);

                if (locationChanging != null)
                    locationChanging(this, locationArgs);

                this.Top = locationArgs.NewLocation.Y;
            }
        }

        private void btnMove_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;

            _mouseDown = false;
            Moving = false;

            EventHandler endMove = EndMove;

            if (endMove != null)
                endMove(this, new EventArgs());
        }

        private void PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateName();

            EventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new EventArgs());
        }

        private void SelectedObjectsChanged(object sender, EventArgs e)
        {
            UpdateName();
        }

        private void UpdateName()
        {
            UIUtility.Invoke(this, () =>
            {
                if (propertyGrid.SelectedObject != null)
                    btnMove.Text = propertyGrid.SelectedObject.ToString();
                else
                    btnMove.Text = "(none)";
            });
        }

        /// <summary>
        /// Shrinks this item's property grid.
        /// </summary>
        public void Shrink()
        {
            if (propertyGrid.Height > 20)
                _propertyGridHeight = propertyGrid.Height;

            this.Height = headerPanel.Height + panelSizer.Height + 1;
        }

        /// <summary>
        /// Expands this item's property grid.
        /// </summary>
        public void Expand()
        {
            this.Height = headerPanel.Height + _propertyGridHeight + panelSizer.Height;
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            if (_moveStarted)
                return;

            if (this.Height > this.MinimumSize.Height)
                Shrink();
            else
                Expand();
        }

        private void panelSizer_MouseDown(object sender, MouseEventArgs e)
        {
            _startingResize = e.Y;
            _resizing = true;
        }

        private void panelSizer_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_resizing)
                return;

            int delta = e.Y - _startingResize;

            if (propertyGrid.Height + delta > propertyGrid.MinimumSize.Height)
                this.Height += delta;
        }

        private void panelSizer_MouseUp(object sender, MouseEventArgs e)
        {
            _resizing = false;
        }

        private void panelSizer_Leave(object sender, EventArgs e)
        {
            _resizing = false;
        }
    }

    /// <summary>
    /// A <see cref="System.Windows.Forms.Button"/> that does not show Focus Cues.
    /// </summary>
    public class NoFocusCueButton : Button
    {
        /// <summary>
        /// Gets a value indicating whether the control should display focus rectangles.
        /// </summary>
        /// <returns>true if the control should display focus rectangles; otherwise, false.</returns>
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Event data for a location changing event.
    /// </summary>
    public class LocationChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the new location point.
        /// </summary>
        /// <value>
        /// The new location.
        /// </value>
        public Point NewLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationChangingEventArgs"/> class.
        /// </summary>
        /// <param name="newLocation">The new location.</param>
        public LocationChangingEventArgs(Point newLocation)
            : base()
        {
            NewLocation = newLocation;
        }
    }
}
