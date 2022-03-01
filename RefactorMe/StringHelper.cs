using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace RefactorMe
{
    internal static class StringHelper
    {
        public static string GetNamingPattern(this string name)
        {
            // Example Templates
            return name switch
            {
                "Order" => @"ORD-{date:ddMMyyyy}-{increment:order}",// ORD-12122022-01
                "Site" => @"ST-{entity:location.address.postalOrZipCode}-{increment:site}",// ST-0042-01
                "Product" => @"PRD-{increment:product}",// PRD-01
                _ => "",
            };
        }

        public static string GetDate(this string format)
        {
            return DateTime.Now.ToString(format);
        }

        // TODO: Implement redis server that can be consumed by the application
        public static string GetIncrement(this string type)
        {
            // Need to get this increment from Redis
            return new Random().Next(100)
                        .ToString();
        }

        public static string GetValueFromEntityObject(this string attribute, object entity)
        {
            var jsonObject = JObject.Parse(JsonConvert.SerializeObject(entity));
            var entityValue = jsonObject.SelectToken(attribute);

            return entityValue?.ToString();
        }
    }
}
