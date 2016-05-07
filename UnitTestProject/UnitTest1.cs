using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HouseNumberValidator;
using System.Collections.Generic;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ValidatorHouseNumb v = new ValidatorHouseNumb();
            Dictionary<string, string> tests = new Dictionary<string, string>{
                { "12а К1", "12А к1" },
                { "12К1", "12 к1" },
                { "12аК1c3", "12А к1 с3" },
                { "12К.1", "12 к1" },
                { "12аК1", "12А к1" },
                { "12а сооруж.1", "12А соор1" },
                { "12а, к.1", "12А к1" },
                { "д. 12а, к.1", "12А к1" }
            };

            foreach ( var test in tests )
                Assert.AreEqual( test.Value, v.CorrectHouseNumb( test.Key ) );
        }
    }
}
