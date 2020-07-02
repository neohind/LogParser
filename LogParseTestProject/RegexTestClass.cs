using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogParseTestProject
{
    [TestClass]
    public class RegexTestClass
    {
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        public void TestMakeRegexressStr()
        {
            string[] arySearchWords = new string[] { "(Test)", "[Test]", "\"Test\"" };

            StringBuilder sbRegex = new StringBuilder();
            bool bIsFirst = true;
            sbRegex.Append("(");
            foreach (string sWord in arySearchWords)
            {
                string sWordReplaced = Regex.Replace(sWord, @"([\?\(\)\[\]\*\$\.\-\!\'\""])", @"\$1");
                if (bIsFirst)
                    bIsFirst = false;
                else
                    sbRegex.Append("|");

                sbRegex.AppendFormat("{0}", sWordReplaced);
            }
            sbRegex.Append(")");

            TestContext.WriteLine(sbRegex.ToString());

            Regex regex = new Regex(sbRegex.ToString());

            Assert.IsTrue(regex.IsMatch("skajfa;sdjf;asdkfja;sd[Test]"));

            Assert.IsTrue(regex.IsMatch("skajfa;sdjf;asdkfja;sd\"Test\"ss"));

        }
    }
}
