//
//  (c) Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/01/09] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Forms;

/// <summary>
/// Create an instance of this class in the constructor of a form. 
/// Will save and restore window state, size, and position.
/// Uses DesktopBounds (instead of just Form.Location/Size) 
/// to place window correctly on a multi screen desktop.
/// source: http://www.codeproject.com/useritems/FormState.asp
/// </summary>
public class FormPosition
{
    // Nested declarations -------------------------------------------------
    
    // Member variables ----------------------------------------------------

    public const int Undefined = -9999999;

    private Form _parent;
    private int _x = Undefined;
    private int _y = Undefined;
    private int _width = Undefined;
    private int _height = Undefined;
    private int _windowState = Undefined;

    // Constructors & Finalizers -------------------------------------------

    /// <summary>
    /// Initializes an instance of the FormPosition class.
    /// </summary>
    /// <param name="parent">
    /// The form to store settings for.
    public FormPosition(Form parent)
    {
        Parent = parent;
    }  
  
    // Properties ----------------------------------------------------------

    public int X
    { get { return _x; } set { _x = value; } }

    public int Y
    { get { return _y; } set { _y = value; } }

    public int Width
    { get { return _width; } set { _width = value; } }

    public int Height
    { get { return _height; } set { _height = value; } }

    public int WindowState
    { get { return _windowState; } set { _windowState = value; } }

    public Form Parent
    {
        get 
        { 
            return _parent; 
        }
        set 
        {
            this._parent = value;
            this._parent.Load += new EventHandler(On_Load);
            this._parent.FormClosed += new FormClosedEventHandler(On_FormClosed);
        }
    }

    // Methods -------------------------------------------------------------
    
    // Event handlers ------------------------------------------------------
    
    private void On_Load(object sender, EventArgs e)
    {
        if (_x == Undefined)
            _x = this._parent.DesktopBounds.X;
        if (_y == Undefined)
            _y = this._parent.DesktopBounds.Y;
        if (_width == Undefined)
            _width = this._parent.DesktopBounds.Width;
        if (_height == Undefined)
            _height = this._parent.DesktopBounds.Height;
        if (_windowState == Undefined)
            _windowState = (int)this._parent.WindowState;

        // In case of multi screen desktops, check if we got the
        // screen the form was when closed.
        // If not there we put it in upper left corner of nearest 
        // screen.
        // We don't bother checking size (as long as the user see
        // the form ...).
        Rectangle screen_bounds = Screen.GetBounds(new Point(_x, _y));
        if (_x > screen_bounds.X + screen_bounds.Width)
        {
            _x = screen_bounds.X;
            _y = screen_bounds.Y;
        }

        // set parent window
        this._parent.DesktopBounds = new Rectangle(_x, _y, _width, _height);
        this._parent.WindowState = (FormWindowState)_windowState;
    }

    private void On_FormClosed(object sender, FormClosedEventArgs e)
    {
        // There may be cases where the event is raised twice.
        // To avoid handling it twice we remove the handler.
        this._parent.Load -= new EventHandler(On_Load);
        this._parent.FormClosed -= new FormClosedEventHandler(On_FormClosed);

        _windowState = (int)this._parent.WindowState;

        // save pos & size in normal window state
        if (this._parent.WindowState != FormWindowState.Normal)
            this._parent.WindowState = FormWindowState.Normal;
        
        _x = this._parent.DesktopBounds.X;
        _y = this._parent.DesktopBounds.Y;
        _width = this._parent.DesktopBounds.Width;
        _height = this._parent.DesktopBounds.Height;
        
        // clear parent
        _parent = null;
    }

    // Internal methods ----------------------------------------------------

}