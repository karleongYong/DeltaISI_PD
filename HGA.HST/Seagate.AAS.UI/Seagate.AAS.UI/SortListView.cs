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
//  [2006/12/04] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    public partial class SortListView : System.Windows.Forms.ListView
    {
        // Nested declarations -------------------------------------------------

        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            //C#
            //public int Compare(object x, object y)
            //{
            //    int returnVal = -1;
            //    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
            //        ((ListViewItem)y).SubItems[col].Text);
            //    // Determine whether the sort order is descending.
            //    if (order == SortOrder.Descending)
            //        // Invert the value returned by String.Compare.
            //        returnVal *= -1;
            //    return returnVal;
            //}
            public int Compare(object x, object y)
            {
                int returnVal = 0;
                //bool isDate = false;

                // Determine whether the type being compared is a date type.
                try
                {
                    // Parse the two objects passed as a parameter as a DateTime.
                    System.DateTime firstDate = DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
                    System.DateTime secondDate = DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
                    returnVal = DateTime.Compare(firstDate, secondDate); 
                    
                }
                catch
                {
                    try
                    {
                        // Parse the two objects passed as a parameter as double
                        double firstValue = Convert.ToDouble(((ListViewItem)x).SubItems[col].Text);
                        double secondValue = Convert.ToDouble(((ListViewItem)y).SubItems[col].Text);
                        returnVal = Math.Sign(firstValue - secondValue); 

                    }
                    catch
                    {
                        returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                    }
                }

                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
            }
        }

        // Member variables ----------------------------------------------------

        private int sortColumn = -1;
        string[] columnNames;
        private int maxRows = 100;

        // Constructors & Finalizers -------------------------------------------

        public SortListView()
        {
            InitializeComponent();
        }

        public SortListView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SortListView
            // 
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.ShowItemToolTips = true;
            this.View = System.Windows.Forms.View.Details;
            this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.SortListView_ColumnClick);
            this.ResumeLayout(false);

        }
        // Properties ----------------------------------------------------------

        public string[] ColumnNames
        {
            get
            {
                return columnNames;
            }
            set
            {
                columnNames = value;
                CreateColumns();
            }
        }

        public int MaxRows
        {
            get { return maxRows; }
            set { maxRows = value; }
        }


        // Methods -------------------------------------------------------------

        public void AddRowCSV(string commaSeparatedString)
        {
            string[] rowValues = commaSeparatedString.Split(',');
            AddRow(rowValues);
        }

        public void AddRow(string[] rowValues)
        {
            ListViewItem item = new ListViewItem(rowValues);
            this.Items.Add(item);
            if (maxRows > 0)
            {
                while (this.Items.Count > maxRows)
                {
                    this.Items.RemoveAt(0);
                }

            }
        }

        // Event handlers ------------------------------------------------------

        private void SortListView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                this.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (this.Sorting == SortOrder.Ascending)
                    this.Sorting = SortOrder.Descending;
                else
                    this.Sorting = SortOrder.Ascending;
            }


            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.ListViewItemSorter = new ListViewItemComparer(e.Column, this.Sorting);

            // Call the sort method to manually sort.
            this.Sort();
        }
        
        // Internal methods ----------------------------------------------------

        private void CreateColumns()
        {
            this.Columns.Clear();
            if (columnNames == null)
                return;

            bool firstColumn = true;
            foreach (string name in columnNames)
            {
                this.Columns.Add(name, firstColumn ? 120 : 70, HorizontalAlignment.Center);
                firstColumn = false;
            }

        }

  
    }
}
