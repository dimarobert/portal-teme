using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portal_teme.ActionResults {
    public class UnauthorizedObjectResult : ObjectResult {

        public UnauthorizedObjectResult(object value) : base(value) {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
