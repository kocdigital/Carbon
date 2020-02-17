using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Carbon.Demo.WebApplication
{
    public class ComposerHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-tenantId",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = true // set to false if this is optional
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Token",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = true // set to false if this is optional
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-transactionId",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema() { Type = "string" },
                Required = true // set to false if this is optional
            });
        }
    }
}
