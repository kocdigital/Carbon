using Carbon.Common;
using Carbon.PagedList;
using Carbon.WebApplication.TenantManagementHandler.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Carbon.WebApplication
{
    /// <summary>
	/// A base class for an MVC controller without view support.
	/// </summary>
	/// <remarks>
	/// For standardized responses adds response types like <see cref="ResponseOk"/>, <see cref="DeletedOk"/>
	/// </remarks>
    public abstract class CarbonController : ControllerBase
    {


        public CarbonController()
        {

        }

         /// <summary>
        /// A response type that returns a result with HttpStatusCode.
        /// </summary>
        /// <typeparam name="T">Type of the response data</typeparam>
        /// <param name="value">The response data</param>
        /// <returns> HttpStatusCode result which could contain the response data from type of <typeparamref name="T"/></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected ObjectResult ResponseResult<T>(T value) where T : IApiResponse
        {
            var httpStatusCode = value.StatusCode.GetHttpStatusCode();
            return StatusCode((int)httpStatusCode, value);
        }

        /// <summary>
        /// A response type which returns a Conflict response status code.
        /// </summary>
        /// <typeparam name="T">Type of the response data</typeparam>
        /// <param name="value">The response data</param>
        /// <returns> Conflict response status result</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected ObjectResult ResponseConflict<T>(T value)
        {
            var result = new ApiResponse<T>(GetRequestIdentifier(), ApiStatusCode.Conflict);
            result.SetData(value);

            return ResponseResult(result);
        }

        /// <summary>
        /// A response type which returns a NotFound response status code.
        /// </summary>
        /// <typeparam name="T">Type of the response data</typeparam>
        /// <param name="value">The response data</param>
        /// <returns> NotFound response status result</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected ObjectResult ResponseNotFound<T>(T value)
        {
            var result = new ApiResponse<T>(GetRequestIdentifier(), ApiStatusCode.NotFound);
            result.SetData(value);

            return ResponseResult(result);
        }

        /// <summary>
        /// A response type which returns an <see cref="OkObjectResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response with data.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="OkObjectResult"/> for the response with its data</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected OkObjectResult ResponseOk<T>(T value)
        {
            var result = new ApiResponse<T>(GetRequestIdentifier(), ApiStatusCode.OK);
            result.SetData(value);

            return Ok(result);
        }

        /// <summary>
        /// A response type which returns an <see cref="OkObjectResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response without any data.
        /// </summary>
        /// <returns>The created <see cref="OkObjectResult"/> for the response.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected OkObjectResult ResponseOk()
        {
            var result = new ApiResponse<object>(GetRequestIdentifier(), ApiStatusCode.OK);
            return Ok(result);
        }

        /// <summary>
        /// An Update response type which returns an <see cref="CreatedAtActionResult"/> object that produces a <see cref="StatusCodes.Status201Created"/> response.
        /// </summary>
        /// <param name="actionName">The name of the action to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="CreatedAtActionResult"/> for the response.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected CreatedAtActionResult UpdatedOk<T>(string actionName, object routeValues, T value)
        {
            var result = new ApiResponse<T>(GetRequestIdentifier(), ApiStatusCode.OK);
            result.SetData(value);

            return CreatedAtAction(actionName, routeValues, result);
        }

        /// <summary>
        /// A Delete response type which returns an OK response status code.
        /// </summary>
        /// <returns> OK response status result</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected OkObjectResult DeletedOk()
        {
            var result = new ApiResponse<object>(GetRequestIdentifier(), ApiStatusCode.OK);
            return Ok(result);
        }

        /// <summary>
        /// A Create response type which returns an <see cref="CreatedAtActionResult"/> object that produces a <see cref="StatusCodes.Status201Created"/> response.
        /// </summary>
        /// <param name="actionName">The name of the action to use for generating the URL.</param>
        /// <param name="routeValues">The route data to use for generating the URL.</param>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="CreatedAtActionResult"/> for the response.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected CreatedAtActionResult CreatedOk<T>(string actionName, object routeValues, T value)
        {
            var result = new ApiResponse<T>(GetRequestIdentifier(), ApiStatusCode.OK);
            result.SetData(value);

            return CreatedAtAction(actionName, routeValues, result);
        }

        /// <summary>
        /// A response type which returns an <see cref="OkObjectResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response with a List of data.
        /// </summary>
        /// <typeparam name="T">Type of the response data</typeparam>
        /// <param name="entity">The response list of data</param>
        /// <returns> The created <see cref="OkObjectResult"/> for the response with a list of data</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected OkObjectResult PagedListOk<T>(IPagedList<T> entity)
        {
            IPagedList pageable = entity;
            Response.Headers.Add("X-Paging-PageIndex", pageable.PageNumber.ToString());
            Response.Headers.Add("X-Paging-PageSize", pageable.PageSize.ToString());
            Response.Headers.Add("X-Paging-PageCount", pageable.PageCount.ToString());
            Response.Headers.Add("X-Paging-TotalRecordCount", pageable.TotalItemCount.ToString());

            IOrderableDto orderableDto = null;
            if (typeof(IOrderableDto).IsAssignableFrom(typeof(T)))
            {
                orderableDto = (IOrderableDto)entity;
            }

            if (orderableDto == null)
            {
                if (pageable.PageNumber > 1)
                {
                    AddParameter("X-Paging-Previous-Link", null, pageable.PageSize, pageable.PageNumber - 1);
                }
                if (pageable.PageNumber * pageable.PageSize < pageable.TotalItemCount)
                {
                    AddParameter("X-Paging-Next-Link", null, pageable.PageSize, pageable.PageNumber + 1);
                }
            }
            else
            {
                if (pageable.PageNumber > 1)
                {
                    AddParameter("X-Paging-Previous-Link", orderableDto.Orderables, pageable.PageSize, pageable.PageNumber - 1);
                }
                if (pageable.PageNumber * pageable.PageSize < pageable.PageNumber)
                {
                    AddParameter("X-Paging-Next-Link", orderableDto.Orderables, pageable.PageSize, pageable.PageNumber + 1);
                }
            }

            var result = new ApiPagedResponse<IPagedList<T>>(GetRequestIdentifier(), ApiStatusCode.OK, entity);
            return Ok(result);
        }

        /// <summary>
        /// Adds some paging variables to the Response Headers.
        /// </summary>
        /// <param name="key">The key to the header to add</param>
        /// <param name="ordination"> A list of <code>Orderable</code> objects.</param>
        /// <param name="pageSize"> Used for indicating how many data are on the page</param>
        /// <param name="pageIndex"> Used for indicating which page it is</param>
        protected void AddParameter(string key, IList<Orderable> ordination, int pageSize, int pageIndex)
        {
            var builder = new StringBuilder();

            builder.Append("?");
            if (ordination != null)
            {
                for (int i = 0; i < ordination.Count; i++)
                {
                    if (builder.Length > 1)
                        builder.Append("&");

                    var value = ordination[i].Value;
                    var isAscending = ordination[i].IsAscending;

                    builder.Append(nameof(ordination)).Append("[").Append(i).Append("]").Append(".")
                           .Append(nameof(value)).Append("=").Append(value)
                           .Append("&")
                           .Append(nameof(ordination)).Append("[").Append(i).Append("]").Append(".")
                           .Append(nameof(isAscending)).Append("=").Append(isAscending.ToString());
                }
            }

            if (builder.Length > 1)
                builder.Append("&");

            var headerParameterLink = builder.Append(nameof(pageSize)).Append("=").Append(pageSize)
                                      .Append("&")
                                      .Append(nameof(pageIndex)).Append("=").Append(pageIndex)
                                      .ToString();

            headerParameterLink = $"{Request.Scheme}://{Request.Host}{Request.Path}{headerParameterLink}";
            Response.Headers.Add(key, headerParameterLink);
        }

        /// <summary>
        /// Gets CorrelationId from the header
        /// </summary>
        /// <returns>CorrelationId</returns>
        private string GetRequestIdentifier()
        {
            return Request.GetIdentifier();
        }




        /// <summary>
        ///  A response type which returns an <see cref="OkObjectResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response with some paged data.
        /// </summary>
        /// <typeparam name="T">Type of the response data</typeparam>
        /// <param name="TEntity">The response data</param>
        /// <param name="ordinatedPageDto">Ordination object for ordering the response data</param>
        /// <param name="totalCount">Total count of response data</param>
        //// <returns>The created <see cref="OkObjectResult"/> for the response with some paged data.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected OkObjectResult PagedOk<T>(T TEntity, IOrdinatedPageDto ordinatedPageDto, int totalCount)
        {
            if (ordinatedPageDto.PageIndex > 1)
            {
                Response.Headers.AddParameter(Request, "X-Paging-Previous-Link", ordinatedPageDto.Orderables, ordinatedPageDto.PageSize, ordinatedPageDto.PageIndex - 1);
            }
            if (ordinatedPageDto.PageIndex < totalCount)
            {
                Response.Headers.AddParameter(Request, "X-Paging-Next-Link", ordinatedPageDto.Orderables, ordinatedPageDto.PageSize, ordinatedPageDto.PageIndex + 1);
            }
            return Ok(TEntity);
        }
    }

    [Obsolete("This extension is obsolete. Usage for only apis dependant to P360Controller")]
    public static class P360ControllerExtensionMethod
    {
        /// <summary>
        /// Adds some paging variables to the Response Headers.
        /// </summary>
        /// <param name="headers">Header dictionary</param>
        /// <param name="request">Http request object</param>
        /// <param name="key">The key to the header to add</param>
        /// <param name="ordination"> A list of <code>Orderable</code> objects.</param>
        /// <param name="pageSize"> Used for indicating how many data are on the page</param>
        /// <param name="pageIndex"> Used for indicating which page it is</param>
        public static void AddParameter(this IHeaderDictionary headers, HttpRequest request, string key, IList<Orderable> ordination, int pageSize, int pageIndex)
        {
            var builder = new StringBuilder();

            builder.Append("?");
            if (ordination != null)
            {
                for (int i = 0; i < ordination.Count; i++)
                {
                    if (builder.Length > 1)
                        builder.Append("&");

                    var value = ordination[i].Value;
                    var isAscending = ordination[i].IsAscending;

                    builder.Append(nameof(ordination)).Append("[").Append(i).Append("]").Append(".")
                           .Append(nameof(value)).Append("=").Append(value)
                           .Append("&")
                           .Append(nameof(ordination)).Append("[").Append(i).Append("]").Append(".")
                           .Append(nameof(isAscending)).Append("=").Append(isAscending.ToString());
                }
            }

            if (builder.Length > 1)
                builder.Append("&");

            var headerParameterLink = builder.Append(nameof(pageSize)).Append("=").Append(pageSize)
                                      .Append("&")
                                      .Append(nameof(pageIndex)).Append("=").Append(pageIndex)
                                      .ToString();

            headerParameterLink = $"{request.Scheme}://{request.Host}{request.Path}{headerParameterLink}";
            headers.Add(key, headerParameterLink);
        }

    }

}
