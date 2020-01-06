using Carbon.Common;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;

namespace Carbon.WebApplication
{
    public abstract class CarbonController : ControllerBase
    {
        public OkObjectResult CarbonOk<T>(T entity)
        {
            if (typeof(IPageableDto).IsAssignableFrom(typeof(T)))
            {
                IPageableDto pageableDto = (IPageableDto)entity;
                Response.Headers.Add("X-Paging-PageIndex", pageableDto.PageIndex.ToString());
                Response.Headers.Add("X-Paging-PageSize", pageableDto.PageSize.ToString());
                Response.Headers.Add("X-Paging-PageCount", pageableDto.PageCount.ToString());
                Response.Headers.Add("X-Paging-TotalRecordCount", pageableDto.TotalCount.ToString());

                IOrderableDto orderableDto = null;
                if (typeof(IOrderableDto).IsAssignableFrom(typeof(T)))
                {
                    orderableDto = (IOrderableDto)entity;
                }

                if (orderableDto == null)
                {
                    if (pageableDto.PageIndex > 1)
                    {
                        AddParameter("X-Paging-Previous-Link", null, pageableDto.PageSize, pageableDto.PageIndex - 1);
                    }
                    if (pageableDto.PageIndex < pageableDto.TotalCount)
                    {
                        AddParameter("X-Paging-Next-Link", null, pageableDto.PageSize, pageableDto.PageIndex + 1);
                    }
                }
                else
                {
                    if (pageableDto.PageIndex > 1)
                    {
                        AddParameter("X-Paging-Previous-Link", orderableDto.Orderables, pageableDto.PageSize, pageableDto.PageIndex - 1);
                    }
                    if (pageableDto.PageIndex < pageableDto.TotalCount)
                    {
                        AddParameter("X-Paging-Next-Link", orderableDto.Orderables, pageableDto.PageSize, pageableDto.PageIndex + 1);
                    }

                }
            }

            return Ok(entity);
        }

        private void AddParameter(string key, IList<Orderable> ordination, int pageSize, int pageIndex)
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

    }
}
