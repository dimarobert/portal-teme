using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Helpers {
    public static class FilesHelpers {

        public static (string fileName, string extension) GetFileNameAndExtension(string fileNameWithExt) {
            if (string.IsNullOrWhiteSpace(fileNameWithExt))
                throw new ArgumentException("Invalid file name provided.", nameof(fileNameWithExt));

            var extSeparatorPosition = fileNameWithExt.LastIndexOf('.');
            if (extSeparatorPosition == -1)
                extSeparatorPosition = fileNameWithExt.Length;

            return (
                fileNameWithExt.Substring(0, extSeparatorPosition),
                fileNameWithExt.Substring(extSeparatorPosition + 1)
            );
        }

    }
}
