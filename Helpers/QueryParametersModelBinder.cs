using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace finsyncapi.Helpers
{
    public class QueryParametersModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Use our reliable parser to create the object.
            var queryParams = QueryParser.Parse(bindingContext.HttpContext.Request.Query);

            // Pass the successfully parsed object to the framework.
            bindingContext.Result = ModelBindingResult.Success(queryParams);

            return Task.CompletedTask;
        }
    }
}
