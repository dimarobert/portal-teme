using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Routing {
    public static class SpaRoutingExtensions {
        public static void MapSpaWithWdsRoute(this IRouteBuilder routeBuilder, string name, object defaults, object constraints = null, object dataTokens = null) {
            var env = routeBuilder.ServiceProvider.GetRequiredService<IHostingEnvironment>();
            MapSpaWithWdsRoute(routeBuilder, name, env.IsDevelopment(), defaults, constraints, dataTokens);
        }

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
