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
//  [9/30/2005] Sabrina Murray
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using Seagate.AAS.Utils;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel.Services
{
    /// <summary>
    /// ConfigData
    ///		Object containing key/value pairs used by app config section
    ///		to store configurations
    ///	</summary>
    public class ConfigData
	{	
		// Member Variables --------------------------------------------------------
		[XmlElement("Name")]
		public string name;
		[XmlElement("Value")]
		public object value;
	}
	
	/// <summary>
	/// AppConfigSection
	///		Class containing section configuration information (Section Name
	///		and an array list of key/value paired configurations for that section
	///		For example: A section would be the base station and it might contain 
	///		a configuration called LightCurtainEnabled (key) is false (value)
	///	</summary>
    [XmlInclude(typeof(ConfigData))]
	public class AppConfigSection 
	{
		// Member Variables --------------------------------------------------------
		private string _name;
		private ConfigData cfgData;
		private ArrayList _configurations = new ArrayList();
		
		// Properties -------------------------------------------------
		[XmlElement("Name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		
		[XmlElement("Configuration")]
		public ArrayList Configurations
		{
			get
			{
				return _configurations;
			}
		}


		// Constructors & Finalizers ----------------------------------
		public AppConfigSection()
		{
		}


		// Methods -----------------------------------------------------
		public void EditExsistingConfig(string key, object value)
		{
			//bool configFound = false;

			try
			{
				foreach(ConfigData cd in Configurations)
				{
					if(String.Compare(cd.name,key) == 0)
					{
						//configFound = true;
						Configurations.Remove(cd);
						cd.name = key;
						cd.value = value;
						Configurations.Add(cd);
						break;
					}
				}
			}
			catch //(Exception ex)
			{

			}
		}

		public void RemoveConfig(string key)
		{
			bool configFound = false;

			try
			{
				foreach(ConfigData cd in Configurations)
				{
					if(String.Compare(cd.name,key) == 0)
					{
						configFound = true;
						Configurations.Remove(cd);
						break;
					}
				}
				if(!configFound)
				{
					MessageBox.Show("Cannot find Config");
				}
			}
			catch //(Exception ex)
			{

			}
		}

		public object GetConfig(string key)
		{
			try
			{
				foreach(ConfigData cd in Configurations)
				{
					if(String.Compare(cd.name,key) == 0)
					{
						return cd.value;
					}
				}
				return "NotFound";				
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.InnerException.Message);
				return null;
			}
		}

		public void RegisterNewConfig(string key, object value)
		{
			bool configFound = false;

			try
			{
				foreach(ConfigData cd in Configurations)
				{
					if(String.Compare(cd.name,key) == 0)
					{
						configFound = true;
						break;
					}
				}

				if(!configFound)
				{
					cfgData = new ConfigData();
					cfgData.name = key;
					cfgData.value = value;
					Configurations.Add(cfgData);
				}
				else
				{					
					MessageBox.Show("Key already Exsists");
				}
				
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.InnerException.Message);
			}
		}
	}

	/// <summary>
	/// AppConfigService
	///		Service for storing, editing and deleting application configuration settings 
	///		to an XML file. 
	///	</summary>
	[XmlInclude(typeof(AppConfigSection))]  
	[XmlRoot("AppConfig")]
    public class AppConfigService : PersistentXML,IService
    {
		// Nested Declarations -----------------------------------------------------
		// Member Variables --------------------------------------------------------
		[XmlElement("Section")]
		public ArrayList sectionTable = new ArrayList();

		// Constructors & Finalizers ----------------------------------
		public AppConfigService()
		{
			fileName =@"C:\Seagate\Viper\Bin\AppConfig.xml";
		}


		// Methods -----------------------------------------------------
		public void EditExsistingConfig(string cfgSection,string key, object value)
		{
			try
			{
				foreach(AppConfigSection acs in sectionTable)
				{
					if(String.Compare(acs.Name,cfgSection) == 0)
					{
						acs.EditExsistingConfig(key,value);
						Save();
						break;
					}
				}	
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public void RemoveConfig(string cfgSection,string key)
		{
			try
			{
				foreach(AppConfigSection acs in sectionTable)
				{
					if(String.Compare(acs.Name,cfgSection) == 0)
					{
						acs.RemoveConfig(key);
						Save();
						break;
					}
				}	
			}			
			catch //(Exception ex)
			{

			}
		}

		public object GetConfig(string cfgSection, string key)
		{
			try
			{
				foreach(AppConfigSection acs in sectionTable)
				{
					if(String.Compare(acs.Name,cfgSection) == 0)
					{
						return acs.GetConfig(key);
						//break;
					}
				}				
				return "Not Found";
			}			
			catch(Exception ex)
			{
				MessageBox.Show(ex.InnerException.Message);
				return null;
			}
		}

		public object GetConfig(string cfgSection, string key, object defaultValue)
		{
			object obj = GetConfig(cfgSection,key);
            string objString = obj as string;
			if(objString == "NotFound")
			{
				RegisterNewConfig(cfgSection,key,defaultValue);
				Save();
				return GetConfig(cfgSection,key);
			}
			else
				return obj;
		}

		public void RegisterNewConfig(string cfgSection, string key, object value)
		{
			bool foundSection = false;

			try
			{				
				foreach(AppConfigSection acs in sectionTable)
				{
					if(String.Compare(acs.Name,cfgSection) == 0)
					{
						acs.RegisterNewConfig(key,value);
						foundSection = true;
						Save();
						break;
					}
				}				
				if(!foundSection)
				{
					AppConfigSection newSection = new AppConfigSection();
					newSection.Name = cfgSection;
					newSection.RegisterNewConfig(key,value);
					sectionTable.Add(newSection);
				}				
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.InnerException.Message);
			}
		}
	
		#region IService Members

		public void InitializeService()
		{
			// TODO:  Add DirectoryLocator.InitializeService implementation
		}

		public void UnloadService()
		{
			// TODO:  Add DirectoryLocator.UnloadService implementation
		}

		#endregion

		// Internal methods ----------------------------------------------------
		protected override void UpdateMemberVariables(PersistentXML xml)
		{
			this.sectionTable    = null; 
			this.sectionTable    = ((AppConfigService) xml).sectionTable;
		}

    }		
}
