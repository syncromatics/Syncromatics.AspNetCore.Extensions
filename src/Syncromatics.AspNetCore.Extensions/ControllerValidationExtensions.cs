using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Syncromatics.AspNetCore.Extensions
{
    public static class ControllerValidationExtensions
    {
        public static void ValidateAndThrowIfInvalid(this Controller controller, string message, Action<ModelStateDictionary> additionalValidation = null)
        {
            additionalValidation?.Invoke(controller.ModelState);
            ThrowIfInvalid(controller, message);
        }

        public static async Task ValidateAndThrowIfInvalidAsync(this Controller controller, string message, Func<ModelStateDictionary, Task> additionalValidation = null)
        {
            if (additionalValidation != null)
                await additionalValidation(controller.ModelState);

            ThrowIfInvalid(controller, message);
        }

        private static void ThrowIfInvalid(Controller controller, string message)
        {
            if (controller.ModelState.IsValid) return;

            var validationFailures = controller.ModelState
                .SelectMany(x => x.Value.Errors
                    .Select(e => new ValidationFailure(x.Key, e.ErrorMessage)));

            throw new ValidationException(message, validationFailures);
        }
    }
}
