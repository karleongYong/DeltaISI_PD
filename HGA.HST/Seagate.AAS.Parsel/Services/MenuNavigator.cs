//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/15/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for MenuNavigator.
	/// </summary>
	public class MenuNavigator : IService
	{
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private string _signalMainCurrent = "";
        private string _signalSubCurrent = "";
        private Point _location = new Point(0,0);
        private Hashtable _panelMap = new Hashtable();
        private UserControl _activePanel = null;
        private Form _mainForm = null;
		private Seagate.AAS.Parsel.Services.MessageChannel _currentViewChannel = null;
        private bool shortPanelNames = false;

        public delegate void ViewChangedHandler(string newViewName);
        public event ViewChangedHandler OnViewChanged;

        // Constructors & Finalizers -------------------------------------------

        internal MenuNavigator()
        {
        }

        // Properties ----------------------------------------------------------

        public Form MainForm
        { get { return _mainForm; } }

        public bool ShortPanelNames
        { set { shortPanelNames = value; } }

        // Methods -------------------------------------------------------------

        void IService.InitializeService()
        {
        }
		
        void IService.UnloadService()
        {
        }

        public void SetMainForm(Form mainForm, int xLocation, int yLocation, MessageChannel MsgCh)
        {
            _mainForm = mainForm;
            _location.X = xLocation;
            _location.Y = yLocation;
			_currentViewChannel = MsgCh;
        }

        public UserControl GetPanel(string panelName)
        {        
            UserControl panel = null;
            if (_panelMap.ContainsKey(panelName))
            {
                panel = (UserControl)_panelMap[panelName];
            }
            return panel;
        }

        public UserControl GetActivePanel()
        {
            return _activePanel;
        }

        public void RegisterPanel(string mainMenu, UserControl panel)
        {
            string key = GenerateKey(mainMenu, _signalSubCurrent);
            if (_panelMap.ContainsKey(key))
            {
                // Note modify to allow reload panel 19-Apr-13
                //throw new ArgumentException("Panel is already registered.");
                _panelMap[key] = panel;
                panel.Location = _location;
                panel.Dock = DockStyle.Fill;
                panel.Visible = false;
            }
            else
            {
                // Note modify to allow reload panel 19-Apr-13
                //panel.Visible = false;
                //_panelMap.Add(key, panel);
                panel.Location = _location;
                panel.Dock = DockStyle.Fill;
                panel.Visible = false;
                _panelMap.Add(key, panel);
            }
        }

        public void RegisterPanel(string mainMenu, string subMenu, UserControl panel)
        {
            string key = GenerateKey(mainMenu, subMenu);
            if (_panelMap.ContainsKey(key))
            {
                throw new ArgumentException("Panel is already registered.");
            }
            else
            {
                panel.Visible = false;
                _panelMap.Add(key, panel);
            }
        }

        public void OnMainMenu(string mainMenu)
        {
            _signalMainCurrent = mainMenu;
            SwitchToPanel();
        }

        public void OnSubMenu(string subMenu)
        {
            _signalSubCurrent = subMenu;
            SwitchToPanel();
        }

		public void OnNavMenu(string mainMenu, string subMenu)
		{
			this._signalMainCurrent = mainMenu;
			this._signalSubCurrent = subMenu;
			SwitchToPanel();
		}

        // Internal methods ----------------------------------------------------

        private string GenerateKey(string mainMenu, string subMenu)
        {
            return (mainMenu + subMenu);
        }

        private void SwitchToPanel()
        {
            string key = GenerateKey(_signalMainCurrent, _signalSubCurrent);
            UserControl panelToShow = (UserControl)_panelMap[key];

            if (_activePanel != null)
            {
                _activePanel.Visible = false;
                _mainForm.Controls.Remove(_activePanel);  
            }
            if (panelToShow != null)
            {

                panelToShow.Location = _location;
                //panelToShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
                panelToShow.Dock = DockStyle.Fill;
                _mainForm.Controls.Add(panelToShow);

                string panelName = "";
                if (shortPanelNames)
                    panelName = _signalMainCurrent;
                else
                    panelName = _signalSubCurrent + " " + _signalMainCurrent + " Panel";

				_currentViewChannel.SendMessage(null, new Seagate.AAS.Parsel.Services.Message(panelName));

                if (OnViewChanged != null)
                    OnViewChanged(panelName);

                panelToShow.BringToFront();
                panelToShow.Visible = true;
                _activePanel = panelToShow;
            }
        }
    }
}
