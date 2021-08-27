using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;
using System.Threading.Tasks;


namespace BLambda.HolaMundo.Helper
{
    /// <summary>
    /// That's just not really вдалий example of ValueProvider implementation
    /// </summary>

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UpperCaseAttribute : Attribute, IBindingSourceMetadata//, IModelNameProvider//, IFromRouteMetadata
    {
        public BindingSource BindingSource => BindingSource.Query; //nor Path neither ModelBinding doesn't work ???

        //public string? Name { get; set; }
    }

    public class UpperCaseValueProvider : RouteValueProvider
    {
        public UpperCaseValueProvider(
            BindingSource bindingSource,
            RouteValueDictionary values)
            : base(bindingSource, values, CultureInfo.InvariantCulture)
        {
        }

        public override ValueProviderResult GetValue(string key)
        {
            var val = base.GetValue(key);
            if (val != ValueProviderResult.None)
            {
                return new ValueProviderResult(val.FirstValue.ToUpper(Culture), Culture);
            }

            return val;
        }
    }

    public class UpperCaseValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var valueProvider = new UpperCaseValueProvider(
                BindingSource.Query, //nor Path neither ModelBinding doesn't work ???
                context.ActionContext.RouteData.Values);

            context.ValueProviders.Add(valueProvider);

            return Task.CompletedTask;
        }
    }
}
