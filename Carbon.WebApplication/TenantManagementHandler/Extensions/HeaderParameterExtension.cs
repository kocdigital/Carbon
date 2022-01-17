using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
namespace Carbon.WebApplication.TenantManagementHandler.Extensions
{
    public class HeaderParameterExtension: IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "p360-solution-id",
                In = ParameterLocation.Header,
                Required = false
            });

        }
    }
}
