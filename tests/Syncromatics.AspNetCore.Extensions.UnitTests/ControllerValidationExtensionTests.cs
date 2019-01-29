using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Syncromatics.AspNetCore.Extensions.UnitTests
{
    public class ControllerValidationExtensionTests
    {
        [Fact]
        public void ShouldValidateAndThrow()
        {
            // Arrange
            var controller = new TestController();
            controller.ModelState.AddModelError("message", "invalid");

            // Act
            Action act = () => controller.SimpleEcho("message");

            // Assert
            var validationException = act.Should().Throw<ValidationException>().And;
            validationException.Message.Should().Be("Invalid message");
            validationException.Errors.Should().BeEquivalentTo(new[]
            {
                new ValidationFailure("message", "invalid"),
            });
        }

        [Fact]
        public void ShouldValidateAndThrowComplex()
        {
            // Arrange
            var controller = new TestController();
            controller.ModelState.AddModelError("message", "invalid");

            // Act
            Action act = () => controller.ComplexEcho("message");

            // Assert
            var validationException = act.Should().Throw<ValidationException>().And;
            validationException.Message.Should().Be("Invalid message");
            validationException.Errors.Should().BeEquivalentTo(new[]
            {
                new ValidationFailure("message", "invalid"),
                new ValidationFailure("message", "Message is not the one valid message"),
            });
        }

        [Fact]
        public void ShouldValidateAndNotThrow()
        {
            // Arrange
            var controller = new TestController();

            // Act
            Action act = () => controller.SimpleEcho("message");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void ShouldValidateAndNotThrowComplex()
        {
            // Arrange
            var controller = new TestController();

            // Act
            Action act = () => controller.ComplexEcho("only valid message");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public async Task ShouldValidateAndThrowAsync()
        {
            // Arrange
            var controller = new TestController();
            controller.ModelState.AddModelError("message", "invalid");

            // Act
            Func<Task<IActionResult>> act = async () => await controller.SimpleEchoAsync("message");

            // Assert
            var validationException = (await act.Should().ThrowAsync<ValidationException>()).And;
            validationException.Message.Should().Be("Invalid message");
            validationException.Errors.Should().BeEquivalentTo(new[]
            {
                new ValidationFailure("message", "invalid"),
            });
        }

        [Fact]
        public async Task ShouldValidateAndThrowComplexAsync()
        {
            // Arrange
            var controller = new TestController();
            controller.ModelState.AddModelError("message", "invalid");

            // Act
            Func<Task<IActionResult>> act = async () => await controller.ComplexEchoAsync("message");

            // Assert
            var validationException = (await act.Should().ThrowAsync<ValidationException>()).And;
            validationException.Message.Should().Be("Invalid message");
            validationException.Errors.Should().BeEquivalentTo(new[]
            {
                new ValidationFailure("message", "invalid"),
                new ValidationFailure("message", "Message is not the one valid message"),
            });
        }

        [Fact]
        public async Task ShouldValidateAndNotThrowAsync()
        {
            // Arrange
            var controller = new TestController();

            // Act
            Func<Task<IActionResult>> act = async () => await controller.SimpleEchoAsync("message");

            // Assert
            await act.Should().NotThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task ShouldValidateAndNotThrowComplexAsync()
        {
            // Arrange
            var controller = new TestController();

            // Act
            Func<Task<IActionResult>> act = async () => await controller.ComplexEchoAsync("only valid message");

            // Assert
            await act.Should().NotThrowAsync<ValidationException>();
        }

        private class TestController : Controller
        {
            public IActionResult SimpleEcho(string message)
            {
                this.ValidateAndThrowIfInvalid("Invalid message");

                return Ok(message);
            }

            public async Task<IActionResult> SimpleEchoAsync(string message)
            {
                await this.ValidateAndThrowIfInvalidAsync("Invalid message");

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

            public async Task<IActionResult> ComplexEchoAsync(string message)
            {
                await this.ValidateAndThrowIfInvalidAsync("Invalid message", async ms =>
                {
                    if (message != "only valid message")
                    {
                        ms.AddModelError(nameof(message), "Message is not the one valid message");
                    }

                    await Task.CompletedTask; // This is just here to await something
                });

                return Ok(message);
            }
        }
    }
}
