using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Syncromatics.AspNetCore.Extensions
{
    public static class ControllerResultExtensions
    {
        public static IActionResult Ok<TEntity>(
            this Controller controller,
            IQueryable<TEntity> items,
            uint page,
            uint perPage,
            string pageQueryStringParameter = "page")
        {
            var count = items.Count();
            var totalPages = (int)Math.Ceiling(count / (double)perPage);

            var links = new Dictionary<string, IEnumerable<string>>();
            var baseQueryString = controller.Request.Query
                .Where(x => x.Key != pageQueryStringParameter)
                .SelectMany(x => x.Value
                    .Select(v => $"{x.Key}={v}"))
                .ToList();

            links.Add($"first", baseQueryString.Concat(new[] { $"{pageQueryStringParameter}=1" }));
            links.Add($"last", baseQueryString.Concat(new[] { $"{pageQueryStringParameter}={totalPages}" }));
            if (1 < page && page <= totalPages)
            {
                links.Add($"prev", baseQueryString.Concat(new[] { $"{pageQueryStringParameter}={page - 1}" }));
            }

            if (1 <= page && page < totalPages)
            {
                links.Add($"next", baseQueryString.Concat(new[] { $"{pageQueryStringParameter}={page + 1}" }));
            }

            var headerValues = links.Select(x => $"<{controller.Request.Path}?{string.Join("&", x.Value)}>; rel=\"{x.Key}\"");
            controller.Response.Headers.Add("Link", string.Join(",", headerValues));

            return controller.Ok(items
                .Skip((int)((page - 1) * perPage))
                .Take((int)perPage)
                .ToList());
        }
    }
}
