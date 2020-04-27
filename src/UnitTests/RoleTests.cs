using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace NTMiner {
    [TestClass]
    public class RoleTests {
        [TestMethod]
        public void StringIgnoreCaseComparerTest() {
            string left = "Hello";
            string right = "hello";
            Assert.IsTrue(StringComparer.OrdinalIgnoreCase.Equals(left, right));
            left = null;
            right = "hello";
            Assert.IsFalse(StringComparer.OrdinalIgnoreCase.Equals(left, right));
            left = "Hello";
            right = null;
            Assert.IsFalse(StringComparer.OrdinalIgnoreCase.Equals(left, right));
            left = "";
            right = "hello";
            Assert.IsFalse(StringComparer.OrdinalIgnoreCase.Equals(left, right));
        }

        [TestMethod]
        public void StringContainsTest() {
            Assert.IsTrue("Admin".IgnoreCaseContains("admin"));
        }

        [TestMethod]
        public void LinqContainsTest() {
            string roles = "admin";
            Assert.IsTrue(roles.Split(',').Contains(Role.RoleEnum.Admin.ToString(), StringComparer.OrdinalIgnoreCase));
            roles = "Admin";
            Assert.IsTrue(roles.Split(',').Contains(Role.RoleEnum.Admin.ToString(), StringComparer.OrdinalIgnoreCase));
            roles = "Admin,user";
            Assert.IsTrue(roles.Split(',').Contains(Role.RoleEnum.Admin.ToString(), StringComparer.OrdinalIgnoreCase));
            roles = "user";
            Assert.IsFalse(roles.Split(',').Contains(Role.RoleEnum.Admin.ToString(), StringComparer.OrdinalIgnoreCase));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Replace的oldValue参数不能为空")]
        public void ReplaceTest() {
            string roles = "Admin,user";
            Assert.AreEqual(roles, roles.Replace(string.Empty, "hello"));
        }

        [TestMethod]
        public void IgnoreCaseReplaceTest() {
            string roles = "Admin,user";
            Assert.AreEqual("user", roles.IgnoreCaseReplace("admin,", string.Empty));
            roles = "Admin,user";
            Assert.AreEqual("admin,user", roles.IgnoreCaseReplace("admin,", "admin,"));
            roles = "Admin,user";
            Assert.AreEqual("admin,user", roles.IgnoreCaseReplace("Admin,", "admin,"));
            roles = "Admin,user";
            Assert.AreEqual(roles, roles.IgnoreCaseReplace(string.Empty, "admin,"));
        }
    }
}
