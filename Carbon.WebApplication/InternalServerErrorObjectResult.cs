using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Carbon.WebApplication
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        /// <summary>
        /// Sets ObjectResults HTTP status code to 500.
        /// </summary>
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
