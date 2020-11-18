using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XyratexOSC.Settings;

namespace XyratexOSC.Tests.Settings
{
    [TestClass]
    public class SettingsNodeTests
    {
        [TestMethod]
        public void GetLevel_TopLevel_ReturnsZero()
        {
            SettingsNode node = new SettingsNode("Test");

            int level = node.Level;

            Assert.AreEqual(0, level);
        }

        [TestMethod]
        public void GetLevel_ChildReturnsOneMoreThanParent()
        {
            SettingsNode node = new SettingsNode("Test");
            SettingsNode childNode = new SettingsNode("Child");
            node.AddChild(childNode);

            int parentLevel = node.Level;
            int childLevel = childNode.Level;

            Assert.AreEqual(parentLevel + 1, childLevel);
        }

        [TestMethod]
        public void GetNode_PathExists_ReturnsNode()
        {
            SettingsNode node = new SettingsNode("Test");
            node.AddChild("Test Child").AddChild("Child of Child");

            SettingsNode foundNode = node.GetNode("Test Child | Child of Child");

            Assert.IsNotNull(foundNode);
        }

        [TestMethod]
        public void GetNode_PathDiffCasing_ReturnsNode()
        {
            SettingsNode node = new SettingsNode("Test");
            node.AddChild("Test Child").AddChild("Child of Child");

            SettingsNode foundNode = node.GetNode("TEST CHILD | ChILd oF chIld");

            Assert.IsNotNull(foundNode);
        }
    }
}
