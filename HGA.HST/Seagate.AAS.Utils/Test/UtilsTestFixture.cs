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
//  [9/15/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Seagate.AAS.Utils.Test
{
    [TestFixture]
    public class UtilsTestFixture
    {
        [SetUp]
        public void Init()
        {
        }

        public class X : EnumItem
        {
            public X(string name)
                : base(name)
            { 
            }
        }

        public class Y : EnumItem
        {
            public Y(string name)
                : base(name)
            {
            }
        }

        public class XColl
        {
            public static readonly X Item1 = new X("Item1");
            public static readonly X Item2 = new X("Item2");
            public static readonly X Item3 = new X("Item3");
        };

        public class YColl
        {
            public static readonly Y Item1 = new Y("Item1");
            public static readonly Y Item2 = new Y("Item2");
            public static readonly Y Item3 = new Y("Item3");
        };

        [Test]
        public void TestEnum()
        {
            Console.WriteLine("value: {0}", (int)XColl.Item1);
            Console.WriteLine("value: {0}", (int)XColl.Item2);
            Console.WriteLine("value: {0}", (int)XColl.Item3);
            Console.WriteLine("value: {0}", (int)YColl.Item1);
            Console.WriteLine("value: {0}", (int)YColl.Item2);
            Console.WriteLine("value: {0}", (int)YColl.Item3);

            if (XColl.Item1 == YColl.Item1)
                System.Diagnostics.Trace.Write("Hello");

            //Assert.
            //Services.ServiceManager.AppConfig;
            //Services.ServiceManager.DirectoryLocator.GetDirectory("dir");

            //MessageBox.Show("sdf");

            //bool ret = ExceptionPolicy.HandleException(new ArgumentNullException("Test Message", "Test Param"), ExceptionPolicyName, new TestConfigurationContext());
            //Assert.IsFalse(ret);
        }
    }
}
