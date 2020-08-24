using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WindowLayouterer.Configuration;
using WindowLayouterer.Domain;

namespace WindowLayouterer.Tests.Configuraton
{
    [TestClass]
    public class KeyParserTests
    {
        private KeyParser KeyParser;

        [TestInitialize]
        public void Setup()
        {
            KeyParser = new KeyParser();
        }

        [TestMethod]
        [DataRow("A", Keys.A)]
        [DataRow("a", Keys.A)]
        [DataRow("B", Keys.B)]
        [DataRow("Plus", Keys.Add)]
        [DataRow("6", Keys.D6)]
        [DataRow("F6", Keys.F6)]
        [DataRow("f6", Keys.F6)]
        [DataRow("Space", Keys.Space)]
        public void Can_ConvertKey(string key, Keys expected)
        {
            var result = KeyParser.Parse(key);
            Assert.AreEqual(expected, result.Key);
            Assert.AreEqual(KeyModifiers.None, result.Modifiers);
        }

        [TestMethod]
        [DataRow("A", KeyModifiers.None)]
        [DataRow("Ctrl+A", KeyModifiers.Control)]
        [DataRow("Ctrl A", KeyModifiers.Control)]
        [DataRow("Ctrl-A", KeyModifiers.Control)]
        [DataRow("Ctrl+Alt+A", KeyModifiers.Control | KeyModifiers.Alt)]
        [DataRow("Ctrl+Shift+A", KeyModifiers.Control | KeyModifiers.Shift)]
        public void Can_ConvertModifiersy(string key, KeyModifiers expected)
        {
            var result = KeyParser.Parse(key);
            Assert.AreEqual(Keys.A, result.Key);
            Assert.AreEqual(expected, result.Modifiers);
        }
    }
}


