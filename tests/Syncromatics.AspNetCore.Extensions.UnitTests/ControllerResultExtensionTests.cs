using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Syncromatics.AspNetCore.Extensions.UnitTests
{
    public class ControllerResultExtensionTests
    {
        [Theory]
        [InlineData(1, 2, 5, null, 2, "?item=one&item=two", 1, 2)]
        [InlineData(2, 2, 5, 1, 3, "?item=one&item=two", 3, 4)]
        [InlineData(5, 2, 5, 4, null, "?item=one&item=two", 9, 10)]
        [InlineData(6, 2, 5, null, null, "?item=one&item=two")]
        [InlineData(1, 10, 1, null, null, "?item=one&item=two", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
        public void ShouldReturnPageOfResults(uint page, uint perPage, uint expectedLastPage, int? expectedPrevPage, int? expectedNextPage, string queryString, params int[] expectedResults)
        {
            // Arrange
            var controller = new TestController()
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };
            controller.ControllerContext.HttpContext.Request.QueryString = new QueryString(queryString);

            // Act
            var actionResult = controller.Index(page, perPage);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<int>>()
                .Which.Should().BeEquivalentTo(expectedResults);
            controller.ControllerContext.HttpContext.Response.Headers.TryGetValue("Link", out var links).Should().BeTrue();
            var expectedLinkHeaders = new List<string>
            {
                $"<{queryString}&pageNumber=1>; rel=\"first\"",
                $"<{queryString}&pageNumber={expectedLastPage}>; rel=\"last\"",
            };

            if (expectedPrevPage.HasValue)
            {
                expectedLinkHeaders.Add($"<{queryString}&pageNumber={expectedPrevPage}>; rel=\"prev\"");
            }

            if (expectedNextPage.HasValue)
            {
                expectedLinkHeaders.Add($"<{queryString}&pageNumber={expectedNextPage}>; rel=\"next\"");
            }

            links.Should().ContainSingle()
                .Which.Split(',').Should().BeEquivalentTo(expectedLinkHeaders);
        }

        private class TestController : Controller
        {
            public IActionResult Index(
                [FromQuery] uint pageNumber,
                [FromQuery] uint perPage)
            {
                var results = Enumerable.Range(1, 10).AsQueryable();
                return this.Ok(results, pageNumber, perPage, nameof(pageNumber));
            }
        }
    }
}
