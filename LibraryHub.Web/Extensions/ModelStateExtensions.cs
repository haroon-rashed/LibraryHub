using LibraryHub.Common.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryHub.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddApiErrors<T>(this ModelStateDictionary modelState, ApiResponse<T> response)
        {
            if (response.Errors.Count == 0)
            {
                modelState.AddModelError(string.Empty, response.Message);
                return;
            }

            foreach (var error in response.Errors)
            {
                modelState.AddModelError(string.Empty, error);
            }
        }
    }
}