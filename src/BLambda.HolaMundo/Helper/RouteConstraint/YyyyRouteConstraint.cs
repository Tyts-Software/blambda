using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BLambda.HolaMundo.Helper
{
    public class YyyyRouteConstraint : IRouteConstraint
    {
        private Regex _regex;

        public YyyyRouteConstraint()
        {
            _regex = new Regex(@"^\d{4}$",
                                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(100));
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.TryGetValue(routeKey, out object value))
            {
                var parameterValueString = Convert.ToString(value, CultureInfo.InvariantCulture);
                if (parameterValueString == null)
                {
                    return false;
                }

                return _regex.IsMatch(parameterValueString);
            }

            return false;
        }
    }
}
