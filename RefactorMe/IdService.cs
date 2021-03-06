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

namespace Opsi.Cloud.Core
{
  internal class ExternalIdService : IExternalIdGeneratorService
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

      NamingPattern = getNamingPattern(typeMetadata.Name);

      if (NamingPattern == string.Empty)
      {
        return result;
      }

      var split = Regex.Split(NamingPattern, regExMatch).Where(s => s != string.Empty).ToArray();

      foreach (var entity in entities)
      {
        var sb = new StringBuilder();

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

              case "entity":
                sb.Append(GetEntity(args[1], entity));
                break;

              case "reference":
                sb.Append(GetReference(args[1], entity));
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

      return result;
    }

    private string getNamingPattern(string name)
    {
      // Example Templates
      switch (name)
      {
        case "Order":
          return @"ORD-{date:ddMMyyyy}-{increment:order}"; // ORD-12122022-01

        case "Site":
          return @"ST-{entity:location.address.postalOrZipCode}-{increment:site}"; // ST-0042-01

        case "Product":
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

    private string GetEntity(string attribute, object entity)
    {
      var splitPath = Regex.Split(attribute, @"\.")
        .Where(s => s != String.Empty).ToArray();
      var resultObject = entity;

      foreach (var attr in splitPath)
      {
        try
        {
          resultObject = 
            resultObject.GetType().GetProperty(attr).GetValue(resultObject, null);
        }
        catch { }
      }

      return resultObject.ToString();

    }

    private string GetReference(string attribute, object entity)
    {
      return entity.GetType().GetProperty(attribute).GetValue(entity, null).ToString();
    }
  }

}




