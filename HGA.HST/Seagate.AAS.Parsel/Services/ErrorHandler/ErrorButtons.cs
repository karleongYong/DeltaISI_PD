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
	public enum ErrorButton
	{
		NoButton = 0,
		OK,
		Cancel,
		Yes,
		No,
		Abort,
		Ignore,
		Retry,
		Reject,
		Stop,
		Skip,
		Bypass,
		Continue,
		Accept,
		RejectPart,
		RejectHDA,
		RejectPallet,
		RejectCaddy,
		RejectTray,
		RetryPallet,
		StackerCleared,
		StackerLoaded,
		ReInspect,
		RePick,
		ResetAndRetry,
		ReScan,
		Last
	}

	public class ButtonList
	{
		public ButtonList()
		{
			Left	= ErrorButton.NoButton;
			Middle	= ErrorButton.Cancel;
			Right	= ErrorButton.OK;
		}

		public ButtonList(ErrorButton left, ErrorButton middle, ErrorButton right)
		{
			this.Left = left;
			this.Middle = middle;
			this.Right = right;
		}

		public ErrorButton Left;
		public ErrorButton Middle;
		public ErrorButton Right;
	}
}