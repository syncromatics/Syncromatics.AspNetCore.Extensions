# Syncromatics.AspNetCore.Extensions

This is a collection of handy extensions for working with ASP.NET Core controllers.

## Quickstart

`ValidateAndThrowIfInvalid`: Validates the current request (using `ModelState`) and throws a `FluentValidation.ValidationException` if not valid.

```csharp
private class TestController : Controller
{
    public IActionResult SimpleEcho(string message)
    {
        this.ValidateAndThrowIfInvalid("Invalid message");

        return Ok(message);
    }

    public IActionResult ComplexEcho(string message)
    {
        this.ValidateAndThrowIfInvalid("Invalid message", ms =>
        {
            if (message != "only valid message")
            {
                ms.AddModelError(nameof(message), "Message is not the one valid message");
            }
        });

        return Ok(message);
    }
}
```

Paged results: Returns a page of results from an `IQueryable<T>` and sets the `Link` header appropriately.

```csharp
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
```

## Building

[![Travis](https://img.shields.io/travis/syncromatics/Syncromatics.AspNetCore.Extensions.svg)](https://travis-ci.org/syncromatics/Syncromatics.AspNetCore.Extensions)
[![NuGet](https://img.shields.io/nuget/v/Syncromatics.AspNetCore.Extensions.svg)](https://www.nuget.org/packages/Syncromatics.AspNetCore.Extensions/)

Build using .NET Core:

```bash
dotnet build
```

Run the unit tests:

```bash
dotnet test
```

## Code of Conduct

We are committed to fostering an open and welcoming environment. Please read our [code of conduct](CODE_OF_CONDUCT.md) before participating in or contributing to this project.

## Contributing

We welcome contributions and collaboration on this project. Please read our [contributor's guide](CONTRIBUTING.md) to understand how best to work with us.

## License and Authors

[![GMV Syncromatics Engineering logo](https://secure.gravatar.com/avatar/645145afc5c0bc24ba24c3d86228ad39?size=16) GMV Syncromatics Engineering](https://github.com/syncromatics)

[![license](https://img.shields.io/github/license/syncromatics/Syncromatics.AspNetCore.Extensions.svg)](https://github.com/syncromatics/Syncromatics.AspNetCore.Extensions/blob/master/LICENSE)
[![GitHub contributors](https://img.shields.io/github/contributors/syncromatics/Syncromatics.AspNetCore.Extensions.svg)](https://github.com/syncromatics/Syncromatics.AspNetCore.Extensions/graphs/contributors)

This software is made available by GMV Syncromatics Engineering under the MIT license.