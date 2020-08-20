using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using WindowLayouterer.Platform;

namespace WindowLayouterer.Tests.Platform
{
    [TestClass]
    public class PlatformManagementTests
    {
        private PlatformInterface PlatformInterface;
        private PlatformManagement PlatformManagement;

        [TestInitialize]
        public void Setup()
        {
            PlatformInterface = Substitute.For<PlatformInterface>();
            PlatformManagement = new PlatformManagement(PlatformInterface);
        }

        [TestMethod]
        public void CanGetAllVisibleWindows()
        {

        }
    }
}
