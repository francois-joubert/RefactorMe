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

        [TestMethod]
        public async Task GivenGenerate_WhenTypeMetadataIsSite_ShouldStartWithSiteNamingPattern()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = "Site" });

            string result = (string)entity["externalId"];
            Assert.IsTrue(result.StartsWith("ST"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenTypeMetadataIsOrder_ShouldStartWithSiteNamingPattern()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = "Order" });

            string result = (string)entity["externalId"];
            Assert.IsTrue(result.StartsWith("ORD"));
        }
    }
}