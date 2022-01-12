using System;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Opsi.Architecture;
using Opsi.Cloud.Core.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opsi.Cloud.Core.Model;
using RefactorMe.Types;

namespace Opsi.Cloud.Core
{
    public class ExternalIdService : IExternalIdGeneratorService
    {
        private readonly ILogger<ExternalIdService> _logger;

        // Temporary Hard-Coded Configuration
        private string NamingPattern = @"";

        private readonly string regExMatch = @"({[a-z]*:[^:]*})";

        public ExternalIdService(
          ILogger<ExternalIdService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceActionResult> GenerateAsync(List<Dictionary<string, object>> entities, TypeMetadata typeMetadata)
        {
            var result = new ServiceActionResult();

            NamingPattern = GetNamingPattern(typeMetadata.Name);

            if (NamingPattern == string.Empty
                && TempExclusionHack(typeMetadata))
            {
                return result;
            }

            var split = Regex.Split(NamingPattern, regExMatch).Where(s => s != string.Empty).ToArray();

            foreach (var entity in entities)
            {
                var sb = new StringBuilder();

                switch (typeMetadata.Name)
                {
                    case EntityTypes.Site:
                        {
                            SetSiteExternalId(entity);
                            break;
                        }
                    case EntityTypes.Order:
                        {
                            SetOrderExternalId(entity);
                            break;
                        }
                    default:
                        break;
                }

                if (TempExclusionHack(typeMetadata))
                {
                    foreach (var section in split)
                    {
                        if (section.StartsWith('{'))
                        {
                            var args = Regex.Split(section, @":|{|}").Where(s => s != string.Empty).ToArray();
                            switch (args[0])
                            {
                                case "date":
                                    sb.Append(GetDate(args[1]));
                                    break;

                                case "increment":
                                    sb.Append(GetIncrement(args[1]));
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            sb.Append(section);
                        }
                    }

                    entity["externalId"] = sb.ToString();
                }
            }

            return result;
        }

        private void SetOrderExternalId(Dictionary<string, object> entity)
        {
            // ORD-{date:ddMMyyyy}-{increment:order}"; // ORD-12122022-01
            entity["externalId"] = $"ORD-" +
                $"{GetDate("ddMMyyyy")}-" +
                $"{GetIncrement(EntityTypes.Order.ToLower())}";
        }

        private static bool TempExclusionHack(TypeMetadata typeMetadata)
        {
            return typeMetadata.Name != EntityTypes.Site && typeMetadata.Name != EntityTypes.Order;
        }

        #region private
        private void SetSiteExternalId(Dictionary<string, object> entity)
        {
            // ST-{entity:location.address.postalOrZipCode}-{increment:site} // ST-0042-01

            entity["externalId"] = $"ST-" +
                $"{GetEntity("location.address.postalOrZipCode", entity)}-" +
                $"{GetIncrement(EntityTypes.Site.ToLower())}";
        }
        private string GetNamingPattern(string name)
        {
            // Example Templates
            switch (name)
            {
                case EntityTypes.Order:
                    return @"ORD-{date:ddMMyyyy}-{increment:order}"; // ORD-12122022-01

                case EntityTypes.Product:
                    return @"PRD-{increment:product}"; // PRD-01

                default:
                    return "";
            }
        }

        private string GetDate(string format)
        {
            return DateTime.Now.ToString(format);
        }



        private string GetIncrement(string type)
        {
            // Need to get this increment from Redis
            return new Random().Next(100)
                        .ToString();
        }

        private string GetEntity(string attribute, Dictionary<string, object> entity)
        {
            var splitPath = Regex.Split(attribute, @"\.")
              .Where(s => s != String.Empty).ToArray();
            var resultObject = entity;
            var result = "";

            foreach (var attr in splitPath)
            {
                try
                {
                    if (resultObject.ContainsKey(attr))
                    {
                        var value = resultObject[attr];

                        if (value != null && value is Dictionary<string, object>)
                        {
                            resultObject = (Dictionary<string, object>)value;
                        }
                        else
                        {
                            result = (string)value;
                        }
                    }
                }
                catch { }
            }

            return result;
        }
        #endregion
    }

}




