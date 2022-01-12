using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;
using RefactorMe.Types;
using System;
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
            string externalId = (string)entity["externalId"];

            Assert.IsTrue(externalId.StartsWith("PRD"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenUnknownEntityType_ShouldNotGenerateExternalId()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = "IamUnknown" });

            Assert.IsFalse(entity.ContainsKey("externalId"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenNonObjectStart_ShouldAppendNonObjectCharecter()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = EntityTypes.Order });
            string externalId = (string)entity["externalId"];

            var NonObjectCharecter = '-';
            Assert.AreEqual(NonObjectCharecter, externalId[3]);
        }

        [TestMethod]
        public async Task GivenGenerate_WhenDateNamingPattern_ShouldAppendDate()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = EntityTypes.Order });
            string externalId = (string)entity["externalId"];

            var date = DateTime.Now.ToString("ddMMyyyy");
            Assert.IsTrue(externalId.StartsWith($"ORD-{date}"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenIncrementNamingPattern_ShouldAppendIncrement()
        {
            var entity = new Dictionary<string, object>();

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = EntityTypes.Order });
            string externalId = (string)entity["externalId"];

            var date = DateTime.Now.ToString("ddMMyyyy");
            Assert.AreEqual(13, $"ORD-{date}-".Length);
            // some random number was added
            Assert.IsTrue(externalId.Length > 13);
        }

        [TestMethod]
        public async Task GivenGenerate_WhenSiteEntityTypeAndPropertyDrill_ShouldGetDfrilledDownValue()
        {
            var entity = new Dictionary<string, object>() { 
                { "id", 1 },
                {   "location", new Dictionary<string, object> {
                        {"address", new Dictionary<string, object> { { "postalOrZipCode", "0042" } } }
                    } 
                }
            };

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity },
                new TypeMetadata { Name = EntityTypes.Site });
            string externalId = (string)entity["externalId"];

            Assert.IsTrue(externalId.StartsWith($"ST-0042-"));
        }

        [TestMethod]
        public async Task GivenGenerate_WhenMultipleEntitiers_ShouldProccessAll()
        {
            var entity = new Dictionary<string, object>() {
                { "id", 1 },
                {   "location", new Dictionary<string, object> {
                        {"address", new Dictionary<string, object> { { "postalOrZipCode", "0042" } } }
                    }
                }
            };
            var entity2 = new Dictionary<string, object>() {
                { "id", 1 },
                {   "location", new Dictionary<string, object> {
                        {"address", new Dictionary<string, object> { { "postalOrZipCode", "MyValue" } } }
                    }
                }
            };

            await _classUnderTest.GenerateAsync(
                new List<Dictionary<string, object>> { entity, entity2 },
                new TypeMetadata { Name = EntityTypes.Site });
            string externalId = (string)entity["externalId"];
            string externalEntity2Id = (string)entity2["externalId"];

            Assert.IsTrue(externalId.StartsWith($"ST-0042-"));
            Assert.IsTrue(externalEntity2Id.StartsWith($"ST-MyValue-"));
        }
    }
}