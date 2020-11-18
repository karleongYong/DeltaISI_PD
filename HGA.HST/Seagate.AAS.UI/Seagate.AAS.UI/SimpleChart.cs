//
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
//  [9/12/2005]
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    public enum SymbolShape : int
    {
        Circle = 0,
        Square = 1,
        Cross  = 2
    }

	/// <summary>
	/// Summary description for SimpleChart.
	/// </summary>
    [ToolboxBitmapAttribute(typeof(Seagate.AAS.UI.SimpleChart), "images.SimpleChart.ico")]
    public class SimpleChart : System.Windows.Forms.Control
    {

        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        private System.ComponentModel.Container components = null;
        private double[] yValues = new double[5] {0, 1, 2, -1, 0};
        private double ySum  = 0;
        private double yMean = 0;
        private double yMax  = 1;
        private double yMin  = 0;
        private double yScale  = 1;
        private long   yCount  = 0;
        private int    margin  =6;

        private SymbolStyle symbol      = new SymbolStyle();
        
        private LineStyle boundaryLine  = new LineStyle();
        private LineStyle chartLine     = new LineStyle();
        private LineStyle meanLine      = new LineStyle();

        private Font   labelFont = new Font("Microsoft Sans Serif", (float) 8.25);
        private string labelNumberFormat = "0.000";

        // Constructors & Finalizers -------------------------------------------
        public SimpleChart()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitComponent call
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            meanLine.Style = DashStyle.DashDot;
            
            boundaryLine.Width = 2;

            UpdateStatistics();
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                    components.Dispose();
            }
            base.Dispose( disposing );
        }
        
        // Properties ----------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double[] YValues
        {
            get
            {
                return yValues;
            }
            set
            {
                if (this.yValues != value)
                {
                    this.yValues = value; 
                    UpdateStatistics();
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SymbolStyle Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                if (this.symbol != value)
                {
                    this.symbol = value; 
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineStyle ChartLine
        {
            get
            {
                return chartLine;
            }
            set
            {
                if (this.chartLine != value)
                {
                    this.chartLine = value; 
                    this.Invalidate();
                }
            }
        }


        [CategoryAttribute("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineStyle MeanLine
        {
            get
            {
                return meanLine;
            }
            set
            {
                if (this.meanLine != value)
                {
                    this.meanLine = value; 
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineStyle BoundaryLine
        {
            get
            {
                return boundaryLine;
            }
            set
            {
                if (this.boundaryLine != value)
                {
                    this.boundaryLine = value; 
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Font LabelFont
        {
            get
            {
                return labelFont;
            }
            set
            {
                if (this.labelFont != value)
                {
                    this.labelFont = value; 
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute("0.000")]
        public string LabelNumberFormat
        {
            get
            {
                return labelNumberFormat;
            }
            set
            {
                if (this.labelNumberFormat != value)
                {
                    this.labelNumberFormat = value; 
                    this.Invalidate();
                }
            }
        }

        [CategoryAttribute("Appearance")]
        [DefaultValueAttribute(6)]
        public new int Margin
        {
            get
            {
                return margin;
            }
            set
            {
                if (this.margin != value)
                {
                    this.margin = value; 
                    this.Invalidate();
                }
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        // Methods -------------------------------------------------------------
        
        // Internal methods ----------------------------------------------------
        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            Graphics g = pe.Graphics;
            int xValue;
            int yValue;
            int yMeanValue;

            // draw chart boundary
            using (Pen boundaryPen = new Pen(boundaryLine.Color, boundaryLine.Width))
            using (Pen chartPen = new Pen(chartLine.Color, chartLine.Width))
            using (Pen meanPen = new Pen(meanLine.Color, meanLine.Width))
            using (Pen   symbolPen = new Pen(symbol.Color, 1))
            using (Brush symbolBrush = new SolidBrush(symbol.Color))
            using (Brush foreBrush = new SolidBrush(this.ForeColor))
            using (Brush backBrush = new SolidBrush(this.BackColor))
            {

                // calculate label size
                SizeF labelSize = g.MeasureString(yMean.ToString(labelNumberFormat), labelFont);
                SizeF titleSize = g.MeasureString(this.Text, this.Font);

                // calculate chart rectangle
                Rectangle boundary = this.ClientRectangle;
                //boundary.Inflate(-margin, -margin);
                boundary.X = (int) (labelSize.Width + margin/2);
                boundary.Width -= (boundary.X + 2*margin);
                boundary.Y = (int) (titleSize.Height + margin/2);
                boundary.Height -= (boundary.Y + 2* margin);

                // draw boundary
                boundaryPen.DashStyle = boundaryLine.Style;
                g.DrawRectangle(boundaryPen, boundary);

                if (yCount > 1)
                {
                    // calculate scale & offset
                    // scale = pixels/unit
                    if (yMax-yMin==0)
                    {
                        yScale = 1;
                        yMeanValue = boundary.Y + boundary.Height/2;
                    }
                    else
                    {
                        yScale = boundary.Height/(yMax-yMin);                    
                        yMeanValue = boundary.Y + boundary.Height - (int) ((yMean-yMin)*yScale);
                    }

                    // draw mean line
                    meanPen.DashStyle = meanLine.Style;
                    g.DrawLine(meanPen, boundary.X, yMeanValue, boundary.X + boundary.Width, yMeanValue);
                                    

                    // draw data
                    chartPen.DashStyle = chartLine.Style;
                    int xDivision = boundary.Width/((int) (yCount-1));
                    Point lastPoint = new Point(0, 0);
                    Point currentPoint;
                    bool firstPoint = true;
                    // draw line
                    for (int i=0; i<yCount; i++)
                    {
                        xValue = boundary.X + i*xDivision;
                        yValue = yMeanValue - (int) ((yValues[i]-yMean)*yScale);
                        
                        currentPoint = new Point(xValue, yValue);

                        if (!firstPoint)
                            g.DrawLine(chartPen, lastPoint, currentPoint);

                        lastPoint = currentPoint;
                        firstPoint = false;
                    }

                    // draw symbols
                    for (int i=0; i<yCount; i++)
                    {
                        xValue = boundary.X + i*xDivision;
                        yValue = yMeanValue - (int) ((yValues[i]-yMean)*yScale);
                        
                        currentPoint = new Point(xValue, yValue);

                        switch(symbol.Style) 
                        {
                            case SymbolShape.Circle:
                                g.FillEllipse(symbolBrush, currentPoint.X-symbol.Size/2, currentPoint.Y-symbol.Size/2, symbol.Size, symbol.Size);
                                break;
                            case SymbolShape.Square:
                                g.FillRectangle(symbolBrush, currentPoint.X-symbol.Size/2, currentPoint.Y-symbol.Size/2, symbol.Size, symbol.Size);
                                break;                        
                            case SymbolShape.Cross:
                                g.DrawLine(symbolPen, currentPoint.X-symbol.Size/2, currentPoint.Y,  currentPoint.X+symbol.Size/2, currentPoint.Y);
                                g.DrawLine(symbolPen, currentPoint.X, currentPoint.Y-symbol.Size/2,  currentPoint.X, currentPoint.Y+symbol.Size/2);
                                break;                          
                        }
                    }

                    // draw labels
                    StringFormat fmt = new StringFormat();
                    fmt.Alignment = StringAlignment.Near;
                    fmt.LineAlignment = StringAlignment.Center;
                    fmt.FormatFlags = StringFormatFlags.NoClip;

                    Rectangle textRectangle;

                    textRectangle = new Rectangle(0, yMeanValue-50/2, 100, 50);
                    g.DrawString(yMean.ToString(labelNumberFormat), labelFont, foreBrush, textRectangle, fmt);

                    textRectangle = new Rectangle(0, boundary.Y-25, 100, 50);
                    g.DrawString(yMax.ToString(labelNumberFormat), labelFont, foreBrush, textRectangle, fmt);
                    
                    textRectangle = new Rectangle(0, boundary.Y+boundary.Height - 25, 100, 50);
                    g.DrawString(yMin.ToString(labelNumberFormat), labelFont, foreBrush, textRectangle, fmt);

                    fmt.Alignment = StringAlignment.Center;
                    fmt.LineAlignment = StringAlignment.Near;
                    textRectangle = new Rectangle(boundary.X, 0, boundary.Width, boundary.Height);
                    g.DrawString(this.Text, this.Font, foreBrush, textRectangle, fmt);

                }

            }

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void UpdateStatistics()
        {
            if (yValues == null)
                return;

            bool firstValue = true;
            yCount = 0;
            foreach (double yValue in yValues)
            {
                if (firstValue)
                {
                    yMin = yValue;
                    yMax = yValue;
                    ySum  = yValue;
                    firstValue = false;
                }
                else 
                {
                    if (yValue < yMin) yMin = yValue;
                    if (yValue > yMax) yMax = yValue;
                    ySum  += yValue;
                }
                yCount ++;
            }

            if (yCount > 0) 
                yMean = ySum/yCount;

        }

    }

    
    #region SymbolStyle
    [TypeConverterAttribute (typeof(SymbolStyleConverter)) ]
    public class SymbolStyle
    {
        private Color color = Color.Black;
        private SymbolShape style = SymbolShape.Circle;
        private int size = 10;

        public SymbolStyle()
        {
        }

        public SymbolStyle(Color color,  SymbolShape style, int size)
        {
            this.color = color;
            this.style = style;
            this.size  = size;
        }

        public Color Color
        { 
            get {return color;} 
            set {color = value;}
        }

        public SymbolShape Style
        { 
            get {return style;} 
            set {style = value;}
        }

        public int Size
        { 
            get {return size;} 
            set {size = value;}
        }
    }

    public class SymbolStyleConverter : ExpandableObjectConverter
    {
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        { return true; }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        { return new SymbolStyle((Color)propertyValues["Color"], (SymbolShape)propertyValues["Style"], (int)propertyValues["Size"]); }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) 
            {
                return true;
            }
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string propertyList = (string)value;
                    string[] properties = propertyList.Split(';');
                    return new SymbolStyle(Color.FromName(properties[0].Trim()), (SymbolShape) int.Parse(properties[1]), int.Parse(properties[2]));
                }
                catch {}
                throw new ArgumentException("The arguments are invalid.");
            }
            return base.ConvertFrom (context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is SymbolStyle)
            {
                if (  destinationType == typeof(string) )
                {
                    SymbolStyle symbol = (SymbolStyle) value;
                    string color = (symbol.Color.IsNamedColor ? symbol.Color.Name : symbol.Color.R + ", " + symbol.Color.G + ", " + symbol.Color.B );
                    return string.Format("{0}; {1}; {2}", color, symbol.Style, symbol.Size.ToString());
                }
            }
            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
    #endregion

    
    #region LineStyle
    [TypeConverterAttribute (typeof(LineStyleConverter)) ]
    public class LineStyle
    {
        private Color color = Color.Black;
        private DashStyle style = DashStyle.Solid;
        private int width = 1;

        public LineStyle()
        {
        }

        public LineStyle(Color color,  DashStyle style, int width)
        {
            this.color = color;
            this.style = style;
            this.width  = width;
        }

        public Color Color
        { 
            get {return color;} 
            set {color = value;}
        }

        public DashStyle Style
        { 
            get {return style;} 
            set {style = value;}
        }

        public int Width
        { 
            get {return width;} 
            set {width = value;}
        }
    }

    public class LineStyleConverter : ExpandableObjectConverter
    {
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        { return true; }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        { return new LineStyle((Color)propertyValues["Color"], (DashStyle)propertyValues["Style"], (int)propertyValues["Width"]); }


        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) 
            {
                return true;
            }
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string propertyList = (string)value;
                    string[] properties = propertyList.Split(';');
                    return new LineStyle(Color.FromName(properties[0].Trim()), (DashStyle) int.Parse(properties[1]), int.Parse(properties[2]));
                }
                catch {}
                throw new ArgumentException("The arguments are invalid.");
            }
            return base.ConvertFrom (context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is LineStyle)
            {
                if (  destinationType == typeof(string) )
                {
                    LineStyle lineStyle = (LineStyle) value;
                    string color = (lineStyle.Color.IsNamedColor ? lineStyle.Color.Name : lineStyle.Color.R + ", " + lineStyle.Color.G + ", " + lineStyle.Color.B );
                    return string.Format("{0}; {1}; {2}", color, lineStyle.Style, lineStyle.Width.ToString());
                }
            }
            return base.ConvertTo (context, culture, value, destinationType);
        }
    }
    #endregion


}
