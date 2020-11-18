using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Seagate.AAS.UI;

namespace Seagate.AAS.Parsel.Hw
{
	/// <summary>
	/// Summary description for PanelIO.
	/// </summary>
	//[ToolboxBitmapAttribute(typeof(Seagate.AAS.Parsel.Hw.Copley.PanelLED), "bitmaps.PanelLedIcon.bmp")]
	public class PanelIO : System.Windows.Forms.UserControl
	{
		// Internal Declarations
		private enum IOType
		{
//			AnalogInput,
//			AnalogOutput,
//			DigitalInput,
//			DigitalOutput,
			Analog,
			Digital,
			None
		}
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private ArrayList ioObjList = new ArrayList();
		private Size ledSize = new Size(136, 42);
		private Size anSize = new Size(200, 72);
		private const int padding = 4;
		private IOType ioType = IOType.None;

		//Constructors and Destructors
		public PanelIO()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// PanelIO
			// 
			this.AutoScroll = true;
			this.Name = "PanelIO";
			this.DockChanged += new System.EventHandler(this.PanelIO_Resize);
			this.Resize += new System.EventHandler(this.PanelIO_Resize);
			this.Load += new System.EventHandler(this.PanelIO_Load);

		}
		#endregion

		//Properties
		public Size SizeLed
		{
			get { return ledSize; }
			set { ledSize = (Size)value; }
		}
		public Size SizeAnalog
		{
			get { return anSize; }
			set { anSize = (Size)value; }
		}
		//Public Methods
		public void Clear()
		{
			this.Controls.Clear();
			this.ioObjList.Clear();
			ioType = IOType.None;
		}

		public void AddIO(IDigitalInput digIn, string label)
		{
			SetType(IOType.Digital);
			Led newLED = new Led();
			switch(digIn.State) //newLED.State = digIn.State;
			{
				case	DigitalIOState.On:	newLED.State = true; break;
				case	DigitalIOState.Off: newLED.State = false; break;
				default:					newLED.State = false; break;
			}
			newLED.DisplayAsButton = false;
			newLED.Text = label;
			this.ioObjList.Add(digIn);
			this.Controls.Add(newLED);
		}

		public void AddIO(IDigitalOutput digOut, string label)
		{
			SetType(IOType.Digital);
			Led newLED = new Led();
			switch(digOut.State) //newLED.State = digOut.State;
			{
				case DigitalIOState.On: newLED.State = true; break;
				case DigitalIOState.Off: newLED.State = false; break;
				default: newLED.State = false; break;
			}
			newLED.DisplayAsButton = true;
			newLED.Click += new EventHandler(this.LED_Click);
			newLED.Text = label;
			this.ioObjList.Add(digOut);
			this.Controls.Add(newLED);
		}

		public void AddIO(IAnalogInput anIn, string label)
		{
			SetType(IOType.Analog);
			AnalogControl newAC = new AnalogControl();
			newAC.Value = anIn.Get();
			newAC.IOTypeOutput = false;
			newAC.Label = label;
			newAC.Units = anIn.Unit;
			this.ioObjList.Add(anIn);
			this.Controls.Add(newAC);
		}

		public void AddIO(IAnalogOutput anOut, string label)
		{
			SetType(IOType.Analog);
			AnalogControl newAC = new AnalogControl();
			newAC.Value = anOut.Get();
			newAC.IOTypeOutput = true;
			newAC.OutputMin = anOut.LimitMin;
			newAC.OutputMax = anOut.LimitMax;
			newAC.ValueChanged += new EventHandler(this.AnalogControl_ValueChanged);
			newAC.Label = label;
			newAC.Units = anOut.Unit;
			this.ioObjList.Add(anOut);
			this.Controls.Add(newAC);
		}

		public void UpdateLayout()
		{
			for(int i=0; i<Controls.Count; i++)
			{
				if(ioType == IOType.Analog)
					Controls[i].Size = anSize;
				else
					Controls[i].Size = ledSize;
				Controls[i].Location = GetPosition(i);
			}
		}

		public void UpdateStates()
		{
			for(int i=0; i<Controls.Count; i++)
			{
				switch (ioType)
				{
					case IOType.Analog:						
						if(((AnalogControl)Controls[i]).IOTypeOutput)
							((AnalogControl)Controls[i]).Value = ((IAnalogOutput)ioObjList[i]).Get();
						else
							((AnalogControl)Controls[i]).Value = ((IAnalogInput)ioObjList[i]).Get();
						break;
					case IOType.Digital:
						DigitalIOState state;
						
						if(((Led)Controls[i]).DisplayAsButton)
							state = ((IDigitalOutput)ioObjList[i]).Get();
						else
							state = ((IDigitalInput)ioObjList[i]).Get();

						if(state == DigitalIOState.On)
							((Led)Controls[i]).State = true;
						else   //for Off and Unknown, maybe need to update LED to accept DigitalIOState?
							((Led)Controls[i]).State = false;

						break;
					default:
						return;
				}
			}
		}

		//Private Methods
		private void PanelIO_Load(object sender, System.EventArgs e)
		{
			if(this.DesignMode)
				return;
		}

		private void AnalogControl_ValueChanged(object sender, EventArgs e)
		{
			AnalogControl AC = (AnalogControl)sender;
			if(AC.IOTypeOutput)
			{
				int index = this.Controls.IndexOf(AC);
				double val = AC.Value;
				((IAnalogOutput)ioObjList[index]).Set(val);
			}
		}
		
		private Point GetPosition(int index)
		{
			Size mySize = Controls[index].Size;
			int newX = Controls[index].Location.X;
			int newY = Controls[index].Location.Y;
			int newRow = 0;
			index = Controls.Count - index - 1;

			int maxRows = this.Height / (mySize.Height + (padding *2));

			if (maxRows != 0)
			{
				newX = ((index) / maxRows) * (mySize.Width + padding * 2) + padding;
				Math.DivRem(index, maxRows, out newRow);
				newY = newRow * (mySize.Height + padding * 2) + padding;
			}
			return new Point(newX,newY);
		}

		private void LED_Click(object sender, EventArgs e)
		{
			Led LED = (Led)sender;
			if(LED.DisplayAsButton)
			{
				int index = this.Controls.IndexOf(LED);
				((IDigitalOutput)ioObjList[index]).Toggle();
				LED.State = !LED.State;
			}
		}

		private void SetType(IOType type)
		{
			if(this.ioType == IOType.None)
			{
				ioType = type;
			}
			else if(this.ioType != type)
			{
				string s = "Cannot add " + type.ToString() + " control while panel contains " + ioType.ToString() + " controls.";
				throw new ArgumentException(s);
			}
		}

		private void PanelIO_Resize(object sender, System.EventArgs e)
		{
			UpdateLayout();
		}

	}
}
