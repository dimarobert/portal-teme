using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.API.Models {
    public struct UploadedTempFile {
        public string OriginalName { get; set; }

        public string TempFileName { get; set; }
    }
}
