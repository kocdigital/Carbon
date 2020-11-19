using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Moq;

using Swashbuckle.AspNetCore.SwaggerGen;

using Xunit;

namespace Carbon.WebApplication.UnitTests
{
	public class HybridOperationFilterTest
	{
        private readonly Mock<OpenApiOperation> mockOpenApiOperation;
        private readonly OperationFilterContext mockOperationFilterContext;

        public HybridOperationFilterTest()
        {
            mockOpenApiOperation = new Mock<OpenApiOperation>();
            mockOperationFilterContext = new OperationFilterContext(new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription(), new Mock<ISchemaGenerator>().Object, new SchemaRepository(), new Mock<MethodInfo>().Object);
        }
        [Fact]
        public void Apply_HTTPGET_ExpectedBehavior()
        {
            var hybridOperationFilter = new HybridOperationFilter();
            mockOperationFilterContext.ApiDescription.HttpMethod = "GET";
            mockOperationFilterContext.ApiDescription.ParameterDescriptions.Add(new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription
            {
                Source = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource("Hybrid", "Hybrid",false,true)
            });

            mockOpenApiOperation.Object.Parameters.Add(new OpenApiParameter
            {
                Name = "Hybrid"
            });

            hybridOperationFilter.Apply(mockOpenApiOperation.Object, mockOperationFilterContext);
            Assert.Equal(1, mockOpenApiOperation.Object.Parameters.Count);
        }
        [Fact]
        public void Apply_HTTPDELETE_ExpectedBehavior()
        {
            var hybridOperationFilter = new HybridOperationFilter();
            mockOperationFilterContext.ApiDescription.HttpMethod = "DELETE";
            mockOperationFilterContext.ApiDescription.ParameterDescriptions.Add(new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription
            {
                Source = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource("Hybrid", "Hybrid", false, true)
            });

            mockOpenApiOperation.Object.Parameters.Add(new OpenApiParameter
            {
                Name = "Hybrid"
            });

            hybridOperationFilter.Apply(mockOpenApiOperation.Object, mockOperationFilterContext);
            Assert.Equal(1, mockOpenApiOperation.Object.Parameters.Count);
        }
        [Fact]
        public void Apply_ExpectedBehavior()
        {
            var hybridOperationFilter = new HybridOperationFilter();
            mockOperationFilterContext.ApiDescription.HttpMethod = "POST";
            mockOperationFilterContext.ApiDescription.ParameterDescriptions.Add(new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription
            {
                Source = new Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource("Hybrid", "Hybrid", false, true),
                Name = "Hybrid"
            });

            mockOpenApiOperation.Object.Parameters.Add(new OpenApiParameter
            {
                Name = "Hybrid"
            });

            hybridOperationFilter.Apply(mockOpenApiOperation.Object, mockOperationFilterContext);
            Assert.Equal(0, mockOpenApiOperation.Object.Parameters.Count);
        }
    }
}
