using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XyratexOSC;

namespace DesktopTester.Settings
{
    [Serializable]
    [TypeConverter(typeof(NamedConverter<User>))]
    public class User : INamed
    {
        private string _name = "User 1";
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

        [PasswordPropertyText(true)]
        [Editor(typeof(UserPasswordCOMPortSettingsEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(UserPasswordConverter))]
        public string Password
        {
            get;
            set;
        }

        [Editor(typeof(UserLevelCOMPortSettingsEditor), typeof(System.Drawing.Design.UITypeEditor))]
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

        public User()
        {
            Password = "";
            Level = UserLevel.Monitor;
        }

        internal User(string name, string password, UserLevel level)
        {
            _name = name;
            _level = level;
            Password = password;
        }

        public void SwitchLevel(UserLevel level)
        {
            _level = level;
        }

        public void ChangeName(string name)
        {
            //TODO: How do we determine if the name is being updated by the settings refresh or a person
            /*
            if (ApplicationSettings.Instance.UserAccess.Users.Contains(name))
                throw new Exception("A user already exists with this name");

            if (!String.IsNullOrEmpty(Password))
            {
                if (ApplicationSettings.Instance.UserAccess.CurrentUser != this)
                    throw new Exception("Must be logged in as this user to change name");
            }
            */

            _name = name;
        }
    }

    public enum UserLevel
    {
        Monitor,
        Operator,
        Engineer,
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
