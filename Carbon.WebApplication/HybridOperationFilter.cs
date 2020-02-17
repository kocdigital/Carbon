using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.WebApplication
{


    public class HybridOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hybridParameters = context.ApiDescription.ParameterDescriptions
                .Where(x => x.Source.Id == "Hybrid")
                .Select(x => new { name = x.Name }).ToList();

            for (var i = 0; i < operation.Parameters.Count; i++)
            {
                for (var j = 0; j < hybridParameters.Count; j++)
                {
                    if (hybridParameters[j].name == operation.Parameters[i].Name)
                    {
                        var name = operation.Parameters[i].Name;
                        var isRequired = operation.Parameters[i].Required;
                        var hybridMediaType = new OpenApiMediaType { Schema = operation.Parameters[i].Schema };

                        operation.Parameters.RemoveAt(i);

                        operation.RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                        {
                            { "application/json", hybridMediaType }
                        },
                            Required = isRequired
                        };
                    }
                }
            }
        }

    }
}
