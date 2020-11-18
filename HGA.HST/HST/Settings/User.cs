using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
//using System.Reflection;
using System.Text;
using XyratexOSC;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Settings
{
    [Serializable]
    [TypeConverter(typeof(NamedConverter<User>))]
//    [Browsable(true)]
    public class User : INamed
    {
        private string _name = "User 1";
        private string _empname = "";
        private UserLevel _level;

        [DisplayName("(Name)")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                ChangeName(value);
            }
        }

        [DisplayName("(EmpName)")]
        public string EmpName
        {
            get
            {
                return _empname;
            }
            set
            {
                ChangeEmpName(value);
            }
        }

        [PasswordPropertyText(true)]
        [Editor(typeof(UserPasswordEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(UserPasswordConverter))]
        public string Password
        {
            get;
            set;
        }

        [Editor(typeof(UserLevelEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public UserLevel Level
        {
            get
            {
                return _level;
            }
            set
            {
                SwitchLevel(value);
            }
        }

        [TypeConverter(typeof(YesNoConverter))]
        [Editor(typeof(MaintenanceAccessEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public bool MaintenanceAccess
        {
            get;
            set;
        }        

        public User()
        {
            Password = "";
            Level = UserLevel.Monitor;
            MaintenanceAccess = false;
        }

        internal User(string name, string password, UserLevel level, bool maintenanceAccess)
        {
            _name = name;
            _level = level;
            Password = password;
            MaintenanceAccess = maintenanceAccess;
        }

        public void SwitchLevel(UserLevel level)
        {
            _level = level;

            // Lai: not useful for our case as it effects all instances.
            // http://stackoverflow.com/questions/13673101/how-do-you-control-what-is-visible-in-a-property-grid-at-runtime

            if (level == UserLevel.Administrator)
                MaintenanceAccess = true;
            else if (level <= UserLevel.Operator)
                MaintenanceAccess = false;
        }

        public void ChangeName(string name)
        {
            //TODO: How do we determine if the name is being updated by the settings refresh or a person
            _name = name;
        }

        public void ChangeEmpName(string empname)
        {
            _empname = empname;
        }
    }

    public enum UserLevel
    {
        Monitor,
        Operator,
        Engineer,
        Technician,
        Administrator
    }

    public class UserPasswordConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
