using Microsoft.AspNetCore.Mvc;
using portal_teme.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portal_teme.API {
    public static class ControllerExtensions {

        /// <summary>
        /// Creates an <see cref="UnauthorizedObjectResult"/> that produces an Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized
        /// response.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="UnauthorizedObjectResult"/> for the response.</returns>
        public static IActionResult Unauthorized(this ControllerBase controller, object value) {
            return new UnauthorizedObjectResult(value);
        }

    }
}
