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
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for IErrorMessage
	/// </summary>

	public interface IErrorMessage : IComparable
	{
		string		Text			{ get; }		// error message to display, i.e. "Move to xxx timeout"
		string		FullDescription { get; }		// the messages from the full exception hierarchy
		void		Format(string formatString, params object[] args);
		void		Format(string msg);
		void		CreateErrorPanel();				// errorMessage instance will create its UI from the right thread.

		string		Source			{ get; set; }	// source of the error, i.e. "Tray Unstacker"
		int			Priority		{ get; set; }	// priority, i.e. warning, serious, critical, et.c.
		ButtonList	BtnList			{ get; set; }
		DateTime	TimeIn			{ get; set; }	// doesn't need to be part of the interface
		int			ItemIndex		{ get; set; }	// doesn't need to be part of the interface
		int			UniqueID		{ get; }		

        ErrorButton ButtonSelected  { get; set; }
		
		void OnInput();								// a method to handle the button press for the client class

		UserControl CustomUI		{ get; set; }	// to override default display
	}

}