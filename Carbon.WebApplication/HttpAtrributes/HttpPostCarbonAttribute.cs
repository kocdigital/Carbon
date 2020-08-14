using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Carbon.WebApplication.HttpAtrributes
{
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public class HttpPostCarbonAttribute : HttpPostAttribute
    {

    }

}
