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
//  [5/22/2006]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Serialization;

namespace Seagate.AAS.Utils.PointStore
{
	/// <summary>
	/// Summary description for TeachPoint.
	/// </summary>
    public class TeachPoint : NPoint
    {
        // Nested declarations -------------------------------------------------
    
        // Member variables ----------------------------------------------------
                
        public delegate void PointChangedEventHandler();
        public event PointChangedEventHandler PointChanged;

        protected TeachPointHistory PointHistory;
        protected string category;

        // Constructors & Finalizers -------------------------------------------
        
        public TeachPoint() 
        {}

        public TeachPoint(int pointID, string name, string category, int historySize, string[] dimensionNames, double[] defaultValues)
            : base(name, dimensionNames, defaultValues)
        {
            base.pointID  = pointID;
            base.misc     = "n/a";
            this.category = category;
            PointHistory  = new TeachPointHistory(this, historySize);
        }

        public TeachPoint(int pointID, string name, int historySize, string[] dimensionNames, double[] defaultValues)
            : base(name, dimensionNames, defaultValues)
        {
            base.pointID  = pointID;
            base.misc     = "n/a";
            this.category = "n/a";
            PointHistory  = new TeachPointHistory(this, historySize);
        }

        public TeachPoint(string name, int historySize, string[] dimensionNames, double[] defaultValues)
            : base(name, dimensionNames, defaultValues)
        {
            base.pointID  = -1;
            base.misc     = "n/a";
            this.category = "n/a";
            PointHistory  = new TeachPointHistory(this, historySize);
        }

        // Properties ----------------------------------------------------------
        
        public string SavedBy
        { get { return base.misc; } }

        public string Category
        {
            get { return category; } 
            set { category = value; }
        }

        public TeachPointHistory History
        {
            get { return PointHistory; } 
            set { PointHistory = value; }
        }

        public new double this [int index]
        {
            get { return GetCoordinate(index); }
            set 
            {
                SetCoordinate(index, value); 
                PointHistory.ArchiveTeachPoint(this);
                
                if (PointChanged != null)
                    PointChanged();
            }
        }

        // Methods -------------------------------------------------------------

        public new void Set(params double[] coordinates)
        {
            if (IsSameCoordinates(coordinates))
                return;

            base.Set(coordinates);
            PointHistory.ArchiveTeachPoint(this);

            if (PointChanged != null)
                PointChanged();
        }

        public void ShallowClone(TeachPoint point)
        {
            NPoint npoint = point as NPoint;
            base.ShallowClone(npoint);
            
            // clone the rest
            this.History = point.History;

            // initialize history
            PointHistory.LoadInit(this);
        }

        // Internal methods ----------------------------------------------------
    }
}
