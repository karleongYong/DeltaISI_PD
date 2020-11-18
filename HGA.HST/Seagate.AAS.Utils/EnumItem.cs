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
//  [2006, Seagate HGA Automation]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Utils
{
    [Serializable()]
    public abstract class EnumItem
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        static private Dictionary<Type, int> _maxItemCount = new Dictionary<Type, int>();
        private int _enumValue;
        private string _enumName;

        // Constructors & Finalizers -------------------------------------------

        public EnumItem(string name)
        {
            _enumName = name;

            Type enumItemType = GetType();
            if (!_maxItemCount.ContainsKey(enumItemType))
            {
                _maxItemCount.Add(enumItemType, 1);
                _enumValue = 1;
            }
            else
            {
                _enumValue = ++_maxItemCount[enumItemType];
                _maxItemCount[enumItemType] = _enumValue;
            }
        }

        // Properties ----------------------------------------------------------

        public int Count
        {
            get
            {
                int maxItemCount = 0;
                if (_maxItemCount.ContainsKey(GetType()))
                    maxItemCount = _maxItemCount[GetType()];

                return maxItemCount;
            }
        }

        // Methods -------------------------------------------------------------                

        public static implicit operator int(EnumItem enumItem)
        {
            return enumItem._enumValue;
        }

        public static bool operator ==(EnumItem lhs, EnumItem rhs)
        {
            return (lhs._enumValue == rhs._enumValue);
        }

        public static bool operator !=(EnumItem lhs, EnumItem rhs)
        {
            return !(lhs._enumValue == rhs._enumValue);
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if (   obj == null
                || GetType() != obj.GetType())
                return false;

            EnumItem val = (EnumItem)obj;
            return (_enumValue == val._enumValue);
        }

        public override int GetHashCode()
        {
            return _enumValue;
        }

        public override string ToString()
        {
            return _enumName;
        }

        // Internal methods ----------------------------------------------------

    }
}
