using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PortalTeme.Data.Application {
    public class ApplicationSetting {

        [Key, Required]
        public string Name { get; set; }

        public string Value { get; set; }

    }
}
