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
//  [9/16/2005]
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// ErrorMessage: base class for messages sent to the error handler service
	/// Classes should derive from here to get any sort of real functionality out of 
	/// Error Handling service.
	/// </summary>
	public class ErrorMessage : IErrorMessage
	{

        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        protected int         _uniqueID;
        protected string _text = null;
        protected string _fullDescription = null;
        protected string _maintenanceDescription = null;
        private string      _source;
        private string _workInstruction;
        private int         _priority;
        private UserControl _customUI        = null;
        private ButtonList  _buttonList;
        private DateTime    _timeIn;
        private int         _itemIndex;
        private ErrorButton _buttonSelected;
        private ErrorButton _defaultButton = ErrorButton.Retry;

        // Properties ----------------------------------------------------------
        
        public int UniqueID
        { get { return _uniqueID; } }

        public string Text				// error message to display, i.e. "Move to xxx timeout"
        { get { return _text; } }

		public string FullDescription
		{ get { return _fullDescription; } }

        public string MaintenanceDescription
        { get { return _maintenanceDescription; } }

        public string Source				// _source of the error, i.e. "Tray Unstacker"
        { 
            get { return _source; }
            set { _source = value; }	
        }

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public UserControl	CustomUI		// i.e. vision display.  null uses default display
        {
            get { return _customUI; }
            set { _customUI = value; }      // embed the UI in the display window somehow
        }

        public ButtonList BtnList
        { 
            get { return _buttonList; }
            set
            {
                _buttonList = value;
                //int left    = (int)_buttonList.Left;
                //int middle  = (int)_buttonList.Middle;
                //int right   = (int)_buttonList.Right;
            }
        }

        public DateTime TimeIn
        { 
            get { return _timeIn; }
            set { _timeIn = value; }
        }

        public int ItemIndex
        {
            get { return _itemIndex; }
            set { _itemIndex = value; }
        }

        public ErrorButton ButtonSelected
        {
            get { return _buttonSelected; }
            set { _buttonSelected = value;}
        }

        public ErrorButton DefaultButton
        {
            get { return _defaultButton; }
            set { _defaultButton = value; }
        }

        // Constructors & Finalizers -------------------------------------------
       
        public ErrorMessage(string src, ButtonList lst)
        {
            _source     = src;
            _buttonList = lst;
            _text       = src;
        }

        public ErrorMessage(System.Exception e)
        {
            Format(e);
            _source = "dunno yet";
            _buttonList = new ButtonList(ErrorButton.NoButton, ErrorButton.OK, ErrorButton.NoButton);
            // _source = CustomException._source ?
            // _buttonList = CustomException.options ?
            // CustomUI = CustomException.UI ?
        }        

        public ErrorMessage()
        {
        }

        // Methods -------------------------------------------------------------

		public virtual void OnInput()
		{
			// reset the UI when the operator hits a button.  This forces the _customUI to be 
			// created every time the error message is re-posted as a work-around for the bug that
			// causes the Error Handler to not display the error message properly
			_customUI = null;
		}

        public virtual void Format(System.Exception e)
		{
            _text            = e.Message;
			_fullDescription = GetExceptionMessages(e);
            _uniqueID        = (_source+_text).GetHashCode();
		}

        public virtual void Format(string msg)
        {
            _text            = msg;
            _fullDescription = _text;
            _uniqueID        = (_source + _text).GetHashCode();
        }

        public virtual void Format(string msgFormat, params object[] args)
        {
            _text            = String.Format(msgFormat,args);
            _fullDescription = _text;
            _uniqueID        = (_source + _text).GetHashCode();
        }

        protected string GetExceptionMessages(System.Exception e)
		{
			string msg = "";
			do 
			{
				msg += e.Message + Environment.NewLine;
				e = e.InnerException;
			} while (e != null);

			return msg;
		}

		public virtual void CreateErrorPanel()
		{
			if(_customUI == null)
				_customUI = new DefaultErrorPanel(this);
		}

        #region IComparable Members

		public virtual int CompareTo(object obj)
		{
			if(obj is ErrorMessage)
			{
				ErrorMessage m = (ErrorMessage) obj;
				return this.Priority.CompareTo(m.Priority);
			}
			return 0;
		}

		#endregion
	}
}
