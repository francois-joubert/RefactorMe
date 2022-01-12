using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;
using RefactorMe.Types;
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
                new TypeMetadata { Name = EntityTypes.Site });
            string result = (string)entity["externalId"];

            Assert.IsTrue(result.StartsWith("ST"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenTypeMetadataIsOrder_ShouldStartWithOrderNamingPattern()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = EntityTypes.Order });
            string result = (string)entity["externalId"];

            Assert.IsTrue(result.StartsWith("ORD"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenTypeMetadataIsProduct_ShouldStartWithProductNamingPattern()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = EntityTypes.Product });
            string result = (string)entity["externalId"];

            Assert.IsTrue(result.StartsWith("PRD"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenUnknownEntityType_ShouldNotGenerateextErnalId()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = "IamUnknown" });

            Assert.IsFalse(entity.ContainsKey("externalId"));
        }
    }
}