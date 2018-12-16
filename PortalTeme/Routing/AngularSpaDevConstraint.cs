using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Routing {
    public class WebpackDevServerSocketConstraint : IRouteConstraint, IParameterPolicy {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection) {
            return !httpContext.Request.Path.StartsWithSegments("/sockjs-node");
        }
    }
}
