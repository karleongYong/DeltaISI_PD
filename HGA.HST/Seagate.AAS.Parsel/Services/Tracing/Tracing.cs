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
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;


namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for ErrorHandler.
	/// </summary>
	public class Tracing : IService
	{
		// Nested declarations -------------------------------------------------
        
		// Member variables ----------------------------------------------------
        
		private Hashtable _traceTable   = new Hashtable();
		private Hashtable _traceWindows = new Hashtable();
		private int _maxTraces = 150;
		private ArrayList _panelList = new ArrayList();

		// Constructors & Finalizers -------------------------------------------
        
		internal Tracing()
		{
			System.Diagnostics.Trace.WriteLine("Seagate.AAS.Parsel()");
			Trace("Loading Tracing Service");
		}

		// Properties ----------------------------------------------------------
		public Hashtable TraceTable
		{
			get { return _traceTable; }
		}

        public int MaxTraces { set { _maxTraces = value; } }

		// Methods -------------------------------------------------------------
        
		public void InitializeService()
		{
			_traceTable.Clear();
			_traceWindows.Clear();
			_panelList.Clear();
		}
		
		public void UnloadService()
		{
			_traceTable.Clear();
			_traceWindows.Clear();
		}

		public void Trace(string sTraceMsg)
		{
			System.Diagnostics.Trace.WriteLine(sTraceMsg);

//			if (Seagate.AAS.UI.SplashScreen.SplashForm != null) 
//			{
//			    Seagate.AAS.UI.SplashScreen.SetStatus(sTraceMsg);
//			}
		}

		public void Trace(string source, string msg)
		{
			Trace(source + ": " + msg);
			TraceMessage tm = new TraceMessage(source,msg);
			Monitor.Enter(_traceTable);
			if(_traceTable.Contains(source))
			{
				ArrayList l = (ArrayList) _traceTable[source];
				while(l.Count > _maxTraces)		// remove oldest message until list size is below the limit
					l.RemoveAt(0);
				l.Add(tm);						// add message to the end of the list
			}
			else
			{
				ArrayList l = new ArrayList();
				l.Add(tm);
				_traceTable.Add(source,l);
			}
			// update any visible tracePanels
			foreach (PanelTrace panel in _panelList)
			{
				if (panel.TracesAreVisible(source))
					panel.UpdateTraces();
			}
			// display the message if a Control has been assigned to this channel
			Control ctrl = (Control) _traceWindows[source];
			if(ctrl != null)
			{
				if(ctrl.InvokeRequired)
					ctrl.BeginInvoke(new SetControlTextDelegate(SetControlText),new object[] { ctrl, msg });
				else
					ctrl.Text = msg;
			}

			Monitor.Exit(_traceTable);
		}

		// similar to Trace, except that no history is kept
		public void SetText(string source, string msg)
		{
			Monitor.Enter(_traceTable);
			if(_traceWindows.Contains(source))
			{
				// display the message if a Control has been assigned to this channel
				Control ctrl = (Control) _traceWindows[source];
				if(ctrl != null)
				{
					if(ctrl.InvokeRequired)
						ctrl.BeginInvoke(new SetControlTextDelegate(SetControlText),new object[] { ctrl, msg });
					else
						ctrl.Text = msg;
				}
			}
			Monitor.Exit(_traceTable);
		}

		// set's Control's color and text
		public void SetText(string source, string msg, System.Drawing.Color color)
		{
			Monitor.Enter(_traceTable);
			if(_traceWindows.Contains(source))
			{
				// display the message if a Control has been assigned to this channel
				Control ctrl = (Control) _traceWindows[source];
				if(ctrl != null)
				{
					if(ctrl.InvokeRequired)
						ctrl.BeginInvoke(new SetControlTextAndColorDelegate(SetControlTextAndColor),new object[] { ctrl, msg, color });
					else
					{
						ctrl.ForeColor = color;
						ctrl.Text = msg;
					}
				}
			}
			Monitor.Exit(_traceTable);
		}

		public void ShowTraces()
		{
			FormTrace ft = new FormTrace(_traceTable);
			ft.ShowDialog();
		}

		public void SetTraceWindow(string source, Control traceWin)
		{
			if(_traceWindows.Contains(source))
			{
				_traceWindows[source] = traceWin;
			}
			else
			{
				_traceWindows.Add(source,traceWin);
			}
		}

		public void ClearTraceWindow(string source)
		{
			if(_traceWindows.Contains(source))
				_traceWindows.Remove(source);
		}

//		public void SetTraceWindow(string source, MessageChannel.ReceiveMessageHandler handler)
//		{
//			return;
//
//
//			MessageChannel msgCh = ServiceManager.Messaging.GetMessageChannel(source);
//			if(msgCh == null)
//			{
//				msgCh = ServiceManager.Messaging.CreateMessageChannel(source);
//			}
//			else
//			{
//				// need to (BE ABLE TO) unregister current handler!!!
//			}
//			msgCh.RegisterReceviceMessageHandler(handler);
//		}

		// message tracing test
		private delegate void SetControlTextDelegate(Control ctrl, string msg);
		public void SetControlText(Control ctrl, string msg) 
		{ 
			ctrl.Text = msg;
		}
		
		private delegate void SetControlTextAndColorDelegate(Control ctrl, string msg, System.Drawing.Color color);
		public void SetControlTextAndColor(Control ctrl, string msg, System.Drawing.Color color)
		{
			ctrl.ForeColor = color;
			ctrl.Text = msg;
		}

		internal void SetPanelTraceVisible(PanelTrace panel, bool visible)
		{
			if(visible)
				_panelList.Add(panel);
			else
			{
				if(_panelList.Contains(panel))
					_panelList.Remove(panel);
			}
		}
	}
}
