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
//  [9/7/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Windows.Forms;
//using ADOX;

namespace Seagate.AAS.Utils
{
	/// <summary>
	/// PersistentMDB 
	///   A utility class to facilitate mdb read/write.
	///   Connects to the specified mdb FileName.
	///   LoadTable loads the specified table into the Dataset.
	/// </summary>
	public class PersistentMDB
	{
        // Nested declarations -------------------------------------------------
            
        // Member variables ----------------------------------------------------
        private OleDbConnection     dbConnection;
        private OleDbDataAdapter    dbAdapter;
        private OleDbCommandBuilder dbCommandBuilder;   
        private DataSet             dataset;
        private string              fileName;
        private string              tableName;
        private bool                connected = false;

        // Constructors & Finalizers -------------------------------------------
        public PersistentMDB()
        {
        }
            
		protected void Dispose()
		{
			if(connected)
				dbConnection.Close();
		}

        // Properties ----------------------------------------------------------
        public string FileName
        { get { return fileName; } }

        public string TableName
        { get { return tableName; } }

        public DataSet DataSet
        { get { return dataset; } }

        // Methods -------------------------------------------------------------
        public bool Load(string fileName, string[] tableNames)
        {
			if(!File.Exists(fileName))
				CreateDatabase(fileName);

            this.fileName = fileName;
            this.tableName = tableNames[0];

            // Create a OleDbConnection.
            string cString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + fileName;
            dbConnection = new OleDbConnection(cString);

            // Create a DataAdapter.
            dbAdapter = new OleDbDataAdapter();
            dbAdapter.MissingMappingAction = MissingMappingAction.Passthrough;
            dbAdapter.MissingSchemaAction  = MissingSchemaAction.AddWithKey;

            // create a CommandBuilder
            dbCommandBuilder = new OleDbCommandBuilder(dbAdapter);

            // create a DataSet
            dataset = new DataSet();

            // open connection
            dbConnection.Open();

            // define TableMapping
            dbAdapter.TableMappings.Clear();

			foreach(string table in tableNames)
			{
				dataset.Tables.Add(table);
				dbAdapter.TableMappings.Add(table, table);  // the table name in the dataset will match the table name in the .mdb file
				bool loaded = LoadTable(table);
				if(loaded && !connected)
					connected  = true;
			}

            //Console.WriteLine("Table [{0}] is open", tableName);
			
			return connected;
        }

        public void Save()
        {
            if (connected)
            {

//                Console.WriteLine("Current DataSet [{0} tables]", dataset.Tables.Count);
//                foreach (DataTable table in dataset.Tables)
//                {
//                    Console.WriteLine("Table: {0} --------", table.TableName);
//
//                    foreach (DataRow row in table.Rows)
//                    {
//                        if (row.RowState != DataRowState.Deleted) 
//                        {
//                            foreach (DataColumn col in table.Columns)
//                                Console.Write("\t{0}", row[col]);
//                        }
//                        Console.WriteLine("\t" + row.RowState);
//                    }
//
//                }

                int rowsUpdated = dbAdapter.Update(dataset, tableName);
                Console.WriteLine("updated {0} rows in {1}", rowsUpdated, tableName);
            }
            else
            {
                throw new Exception("Cannot save what is not yet loaded.");
            }
        }

		protected double GetValue(string row, string column, string table)
		{
			Monitor.Enter(this);
			double d = 0.0;
			try
			{
				if(dataset.Tables[table].Rows.Contains(new object[] {row}))
				{
					d = Convert.ToDouble(dataset.Tables[table].Rows.Find(new object[] {row})[column]);
				}
			}
			catch (System.InvalidCastException e)
			{
				return 0.0;
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);
			}
			finally
			{
				Monitor.Exit(this);
			}

			return d;
		}
			
		protected void SetValue(double val, string column, string row, string table)
		{
			Monitor.Enter(this);
			try
			{
				// assign the value
				dataset.Tables[table].Rows.Find(new object[] {row})[column] = val;
				// create the update command
				string keyName = dataset.Tables[table].Columns[0].ColumnName;
				string assignment = string.Format("[{0}]={1}",column,val);
				string selector = string.Format("[{0}]='{1}'",keyName,row);
				string updateCmd = String.Format("UPDATE {0} SET {1} WHERE {2};",table,assignment,selector);
				dbAdapter.UpdateCommand = new OleDbCommand(updateCmd,dbConnection);
				int updates = dbAdapter.Update(dataset,table);
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);			
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected void SetValue(string val, string column, string row, string table)
		{
			Monitor.Enter(this);
			try
			{
				// assign the value
				dataset.Tables[table].Rows.Find(new object[] {row})[column] = val;
				// create the update command
				string keyName = dataset.Tables[table].Columns[0].ColumnName;
				string assignment = string.Format("[{0}]='{1}'",column,val);
				string selector = string.Format("[{0}]='{1}'",keyName,row);
				string updateCmd = String.Format("UPDATE {0} SET {1} WHERE {2};",table,assignment,selector);
				dbAdapter.UpdateCommand = new OleDbCommand(updateCmd,dbConnection);
				int updates = dbAdapter.Update(dataset,table);
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);			
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected void AddRow(string[] columns, object[] values, string table)
		{
			Monitor.Enter(this);
			try
			{
				// build up the sql command
				string cols = "(";
				string vals = "(";
				for(int i=0;i<columns.Length;i++)
				{
					cols += columns[i] + ",";
					if(values[i].GetType() == typeof(double))
					{
						vals += Convert.ToDouble(values[i]) + ",";			// += converts double to string
					}
					else if(values[i].GetType() == typeof(int))
					{
						vals += Convert.ToInt32(values[i]) + ",";			// += converts int to string
					}
					else // represent as string data
					{
						vals += "'" + values[i].ToString() + "',";
					}
				}
				cols = cols.Remove(cols.Length-1,1);	// replace the last comma with the closing parenthesis
				vals = vals.Remove(vals.Length-1,1);	// replace the last comma with the closing parenthesis
				cols += ")";
				vals += ")";

				// assign the values to the data set
				dataset.Tables[table].Rows.Add(values);

				// update the database
				string cmd = string.Format("INSERT INTO [{0}] {1} VALUES {2};",table,cols,vals);
				dbAdapter.UpdateCommand = new OleDbCommand(cmd,dbConnection);
				int updates = dbAdapter.Update(dataset,table);
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.Message);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

        // Internal methods ----------------------------------------------------
		private void CreateDatabase(string fileName)
		{
//			ADOX.CatalogClass cat = new ADOX.CatalogClass();
//
//			string s = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Jet OLEDB:Engine Type=5";
//			cat.Create(s);
//
//			Console.WriteLine("Database Created Successfully");
//
//			cat = null;
		}

		protected void VerifyPrimaryKeyColumnExists(string columnName, string type, string table)
		{
			VerifyColumnExists(columnName,type,table);
			DataColumn[] dc = dataset.Tables[table].PrimaryKey;
			if(dc.Length == 0 || (dc.Length > 0 && dc[0].Caption != columnName))
			{
				string cmd = string.Format("ALTER TABLE [{0}] ADD PRIMARY KEY ({1});",table,columnName);
				OleDbCommand addCol = new OleDbCommand(cmd,dbConnection);
				try
				{
					addCol.ExecuteNonQuery();
				}
				catch (System.Exception e)
				{
					MessageBox.Show(e.Message);
				}
				dataset.Tables[table].PrimaryKey = new DataColumn[] { dataset.Tables[table].Columns[columnName] };
				connected = LoadTable(table);
			}
		}

		// VerifyRow/ColumnExists are used to set up a static table (one where just the values change,
		//   rows aren't added for logging)
		// here, col should be the column name and rowName should be the key for that entry
		// assumption is made the data type for the key is a string ... kind of weak, but oh well- HG
		protected void VerifyColumnExists(string columnName, string type, string table)
		{
			if(!connected || !dataset.Tables[table].Columns.Contains(columnName))
			{
				string cmd = string.Format("ALTER TABLE [{0}] ADD COLUMN [{1}] {2}",table,columnName,type);
				OleDbCommand addCol = new OleDbCommand(cmd,dbConnection);
				try
				{
					addCol.ExecuteNonQuery();
				}
				catch (System.Exception e)
				{
					MessageBox.Show(e.Message);
				}
				connected = LoadTable(table);
			}
		}

		protected void VerifyRowExists(string rowName, string col, string table)
		{
			if(!dataset.Tables[table].Rows.Contains(rowName))
			{
				string cmd = string.Format("INSERT INTO [{0}] ({1}) VALUES ('{2}');",table,col,rowName);
				OleDbCommand addCol = new OleDbCommand(cmd,dbConnection);
				try
				{
					addCol.ExecuteNonQuery();
				}
				catch (System.Exception e)
				{
					MessageBox.Show(e.Message);
				}
				connected = LoadTable(table);
			}
		}

		protected bool LoadTable(string table)
		{
			// try to create the table, swallow exception if it already exists
			string s = string.Format("CREATE TABLE {0}",table);
			OleDbCommand addTable = new OleDbCommand(s,dbConnection);
			try
			{
				addTable.ExecuteNonQuery();
			}
			catch(System.Data.OleDb.OleDbException oleException)
			{
				string ioes = oleException.Message;
			}
			catch(Exception ex)
			{
				string exs = ex.Message;
			}

			// get whole table from data source ...
			OleDbCommand dbCommand = new OleDbCommand("SELECT * FROM " + table, dbConnection);
			dbCommand.CommandType = CommandType.Text;
			dbAdapter.SelectCommand = dbCommand;

			// ... and fill the dataset with it
			try
			{
				dbAdapter.Fill(dataset,table);
			}
			catch (System.Exception e)
			{
				string msg = e.Message;
				return false;
			}
			return true;
		}

		protected void GetDataSet(string sql, string table, ref DataSet ds)
		{
			Monitor.Enter(this);
			OleDbDataAdapter da = new OleDbDataAdapter();
			da.SelectCommand = new OleDbCommand(sql, dbConnection);

			// ... and fill the dataset with it
			int rows = 0;
			try
			{
				ds.Clear();
				rows = da.Fill(ds,table);
			}
			catch(System.Exception e)
			{
				MessageBox.Show(e.Message);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected DataSet GetDataSet(string sql, string table)
		{
			DataSet ds = new DataSet();
			GetDataSet(sql, table, ref ds);
			return ds;
		}
	}
}
