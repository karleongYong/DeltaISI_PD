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
//  [9/14/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using Seagate.AAS.Parsel;
using NUnit.Framework;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Services;

namespace Seagate.AAS.Parsel.Test
{
    /// <summary>
    /// Summary description for ServicesTestFixture.
    /// </summary>
    [TestFixture]
    public class ServicesTestFixture
    {
        [SetUp]
        public void Init()
        {
            Services.ServiceManager.Initialize();
        }

        [Test]
        public void TestAppConfigService()
        {
            //Services.ServiceManager.AppConfig;
            //Services.ServiceManager.DirectoryLocator.GetDirectory("dir");

            MessageBox.Show("sdf");

            //bool ret = ExceptionPolicy.HandleException(new ArgumentNullException("Test Message", "Test Param"), ExceptionPolicyName, new TestConfigurationContext());
            //Assert.IsFalse(ret);
        }		

        [Test]        
		public void TestDirectoryLocatorService()
		{
// 			string pathCognex = Services.ServiceManager.DirectoryLocator.GetDirectory("Cognex");
// 			string pathWC1Setup = Services.ServiceManager.DirectoryLocator.GetDirectory("Workcell1_Setup");
// 			string pathWC2Setup = Services.ServiceManager.DirectoryLocator.GetDirectory("Workcell2_Setup");
// 
// 			Assert.AreEqual(@"C:\Cognex",pathCognex);
// 			Assert.AreEqual(@"C:\Seagate\Viper\Bin\Workcell1\Setup",pathWC1Setup);			
// 			Assert.AreEqual(@"C:\Seagate\Viper\Bin\Workcell2\Setup",pathWC2Setup);
		}

        class X 
        { 
            int id;  
            public X(int x) { id = x; } 
            public void ReceiveMessage(object source, Services.Message msg) 
            { 
                //Console.WriteLine("Event received by object " + id + msg.msg); 
            } 
        }      
        
        [Test]
        public void TestMessagingService()
        {
            MessageChannel msgCh = ServiceManager.Messaging.CreateMessageChannel("testChannel");

            X o1 = new X(1); 
            X o2 = new X(2); 
            X o3 = new X(3); 
 
            msgCh.MessageSentEvent += new MessageChannel.ReceiveMessageHandler(o1.ReceiveMessage);
            msgCh.MessageSentEvent += new MessageChannel.ReceiveMessageHandler(o2.ReceiveMessage);
            msgCh.MessageSentEvent += new MessageChannel.ReceiveMessageHandler(o3.ReceiveMessage);
 
            // Fire the event. 
            msgCh.SendMessage(null, new Services.Message("here you go")); 
        }
    }
}
