using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefactorMe.Test
{
    [TestClass]
    public class ExternalIdServiceTest
    {
        public ExternalIdService _classUnderTest { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            // ToDo: gets to get ExternalIdService from DI
            _classUnderTest = new ExternalIdService(null);
        }

        
    }
}