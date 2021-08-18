using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLambda.HolaMundo.Helper
{
    public class StatModelBinder : IModelBinder
    {
        private static IList<IInputFormatter>? formatters;
        private static BodyModelBinder? binder;

        public StatModelBinder(IHttpRequestStreamReaderFactory readerFactory)
        {
            formatters ??= new List<IInputFormatter>
            {
                new StatInputFormatter()
            };
            
            binder ??= new BodyModelBinder(formatters, readerFactory);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (binder != null)
            {
                return binder.BindModelAsync(bindingContext);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            //if (bindingContext.Result.IsModelSet && bindingContext.Result.Model is IStat)
            //{
            //    //var inputModel = (ResetPasswordModel)bindingContext.Result.Model;

            //    // all of your property updates with be listed here
            //    //
            //    //
            //    //

            //    //bindingContext.Result = ModelBindingResult.Success(inputModel);
            //}
        }
    }
}
