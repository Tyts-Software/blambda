using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace BLambda.HolaMundo.Helper
{
    public class UpperCaseAttribute : ModelBinderAttribute
    {
        public UpperCaseAttribute() : base(typeof(UpperCaseBinder))
        {
        }
    }

    public class UpperCaseBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (bindingContext.ModelType == typeof(string))
            {
                var val = bindingContext.ValueProvider.GetValue(bindingContext.OriginalModelName);                
                bindingContext.Result = ModelBindingResult.Success(val.FirstValue.ToUpper(val.Culture));                
            }

            return Task.CompletedTask;
        }
    }
}
