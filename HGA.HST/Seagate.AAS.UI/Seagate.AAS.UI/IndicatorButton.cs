//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/04/27] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class IndicatorButton : System.Windows.Forms.Button
	{
		private Image indicatorOn;
		private Image indicatorOff;
        private bool state = false;

		public IndicatorButton()
		{
            Assembly assem = this.GetType().Assembly;
            Stream streamOn  = assem.GetManifestResourceStream("Seagate.AAS.UI.Resources.IndicatorButtonOn.bmp");
            Stream streamOff = assem.GetManifestResourceStream("Seagate.AAS.UI.Resources.IndicatorButtonOff.bmp");
 			this.ImageAlign = ContentAlignment.TopRight;
            indicatorOn  = Image.FromStream(streamOn);
            indicatorOff = Image.FromStream(streamOff);
			//indicatorOn = Image.FromFile(@"C:\AAS_SDK\Assemblies\Seagate.AAS.UI\images\IndicatorButtonOn.bmp");
			//indicatorOff = Image.FromFile(@"C:\AAS_SDK\Assemblies\Seagate.AAS.UI\images\IndicatorButtonOff.bmp");
			this.Image = indicatorOff;

            // check resource names with this
            foreach( string name in assem.GetManifestResourceNames() )
                System.Console.WriteLine(name);
		}

		[DescriptionAttribute("State for the LED. true=On, false=Off")]
		public bool State
		{
            get { 
                return state; 
            }
			set { 
                state = value;
                this.Image = state ? indicatorOn : indicatorOff; 
            }
		}

	}
}
