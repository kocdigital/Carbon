using System.Collections.Generic;

namespace Carbon.WebApplication
{
    public class SwaggerSettings
    {
        /// <summary>
        /// The Endpoint Url of the Api
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        /// The Endpoint Path of the Api
        /// </summary>
        public string EndpointPath { get; set; }

        /// <summary>
        /// The Endpoint Name of the Api
        /// </summary>
        public string EndpointName { get; set; }

        /// <summary>
        /// Flag to indicate if controller XML comments.
        /// </summary>
        public bool EnableXml { get; set; }

        /// <summary>
        /// List of documents that defines or describes an API. An OpenAPI definition uses and conforms to the OpenAPI Specification.
        /// </summary>
        public IList<SwaggerDocument> Documents { get; set; }

        /// <summary>
        /// The route prefix of the Api
        /// </summary>
        public string RoutePrefix { get; internal set; }

        /// <summary>
        /// A document (or set of documents) that defines or describes an API. An OpenAPI definition uses and conforms to the OpenAPI Specification.
        /// </summary>
        public class SwaggerDocument
        {
            /// <summary>
            /// Document name
            /// </summary>
            public string DocumentName { get; set; }

            /// <summary>
            /// Information of the Api. (Title, version, description) <see cref="OpenApiInformation"/>
            /// </summary>
            public OpenApiInformation OpenApiInfo { get; set; }

            /// <summary>
            /// Security parameters of the Api. <see cref="OpenApiSecurity"/>
            /// </summary>
            public OpenApiSecurity Security { get; set; }
        }
        public class OpenApiSecurity
        {
            /// <summary>
            /// The authorization URL to be used for this flow. This MUST be in the form of a URL.
            /// </summary>
            public string AuthorizationUrl { get; set; }
            /// <summary>
            ///  The available scopes for the OAuth2 security scheme. A map between the scope name and a short description for it.
            /// </summary>
            public IList<OpenApiScope> Scopes { get; set; }
        }

        public class OpenApiScope
        {
            /// <summary>
            /// 	OpenApiScope Key.
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// 	OpenApiScope Value of the Key Property.
            /// </summary>
            public string Description { get; set; }
        }

        public class OpenApiInformation
        {
            /// <summary>
            /// The title of the API.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// The version of the OpenAPI document (which is distinct from the OpenAPI Specification version or the API implementation version).
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            ///  A short description of the OpenAPI. CommonMark syntax MAY be used for rich text representation.
            /// </summary>
            public string Description { get; set; }
        }
    }
}
