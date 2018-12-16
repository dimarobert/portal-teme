using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Routing {
    public static class SpaRoutingExtensions {
        public static void MapSpaWithWdsRoute(this IRouteBuilder routeBuilder, string name, bool isDev, object defaults, object constraints = null, object dataTokens = null) {
            var constraintsDict = ObjectToDictionary(constraints);
            if (isDev)
                constraintsDict[name] = new WebpackDevServerSocketConstraint();

            routeBuilder.MapSpaFallbackRoute(name, defaults, constraintsDict, dataTokens);
        }

        private static IDictionary<string, object> ObjectToDictionary(object value) {
            return value as IDictionary<string, object> ?? new RouteValueDictionary(value);
        }
    }
}
