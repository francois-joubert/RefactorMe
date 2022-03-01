using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefactorMe.Tests
{
    public class ExternalIdServiceTests
    {
        private ExternalIdService _externalIdService;
        private Mock<ILogger<ExternalIdService>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<ExternalIdService>>();
            _externalIdService = new ExternalIdService(_mockLogger.Object);
        }

        [Test]
        public async Task Can_Generate_Site_Id()
        {
            // Arrange
            var postalCodeValue = "0042";
            var entity = new Dictionary<string, object> 
            { 
                { "id", 1 },       
                { "location", new Dictionary<string, object> {
                    {"address", new Dictionary<string, object> { 
                        { "postalOrZipCode", $"{postalCodeValue}" } 
                    } }
                }                 }
            };

            var entities = new List<Dictionary<string, object>> { entity };
            var typeMetaData = new TypeMetadata { Name = "Site" };

            // Act
            var result = await _externalIdService.GenerateAsync(entities, typeMetaData);
            var externalId = entity["externalId"];
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(externalId);
            Assert.IsTrue(externalId.ToString().Contains(postalCodeValue));
            Assert.IsTrue(externalId.ToString().StartsWith("ST-"));
        }

        [Test]
        public async Task Can_Generate_Order_Id()
        {
            // Arrange
            var entity = new Dictionary<string, object>();

            var entities = new List<Dictionary<string, object>> { entity };
            var typeMetaData = new TypeMetadata { Name = "Order" };

            // Act
            var result = await _externalIdService.GenerateAsync(entities, typeMetaData);
            var externalId = entity["externalId"];
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(externalId);
            Assert.IsTrue(externalId.ToString().StartsWith("ORD-"));
        }

        [Test]
        public async Task Can_Generate_Product_Id()
        {
            // Arrange
            var entity = new Dictionary<string, object>();

            var entities = new List<Dictionary<string, object>> { entity };
            var typeMetaData = new TypeMetadata { Name = "Product" };

            // Act
            var result = await _externalIdService.GenerateAsync(entities, typeMetaData);
            var externalId = entity["externalId"];
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(externalId);
            Assert.IsTrue(externalId.ToString().StartsWith("PRD-"));
        }

        [Test]
        public async Task Can_Return_Error_For_Empty_Naming_Pattern()
        {
            // Arrange
            var entity = new Dictionary<string, object>();
            var entities = new List<Dictionary<string, object>> { entity };
            var typeMetaData = new TypeMetadata { Name = "" };

            // Act
            var result = await _externalIdService.GenerateAsync(entities, typeMetaData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Errors.Contains("Cannot not get TypeMetaData Naming Pattern as Name is blank!"));
        }

        [Test]
        public async Task Can_Return_Error_For_Null_TypeMetaData()
        {
            // Arrange
            var entity = new Dictionary<string, object>();
            var entities = new List<Dictionary<string, object>> { entity };

            // Act
            var result = await _externalIdService.GenerateAsync(entities, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Errors.Contains("Cannot not get TypeMetaData Naming Pattern as Name is blank!"));
        }

        [Test]
        public async Task Can_Return_Error_For_Null_Entities()
        {
            // Arrange
            var entity = new Dictionary<string, object>();
            var typeMetaData = new TypeMetadata { Name = "Site" };

            // Act
            var result = await _externalIdService.GenerateAsync(null, typeMetaData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Errors.Contains("Entities cannot be null. Please provide entities."));
        }
    }
}
