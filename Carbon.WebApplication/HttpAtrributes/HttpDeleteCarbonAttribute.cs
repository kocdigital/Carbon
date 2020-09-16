using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Carbon.WebApplication.HttpAtrributes
{
    /// <summary>
    ///     An attribute that inherits from HttpDeleteAttribute
    /// </summary>
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class HttpDeleteCarbonAttribute : HttpDeleteAttribute
    {
    }

}
