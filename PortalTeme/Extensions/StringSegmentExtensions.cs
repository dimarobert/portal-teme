using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Extensions {
    public static class StringSegmentExtensions {

        public static bool IsNullOrWhiteSpace(this StringSegment value) {
            return string.IsNullOrWhiteSpace(value.Value);
        }
    }
}
