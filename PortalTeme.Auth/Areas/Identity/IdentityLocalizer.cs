using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity {

    public interface IIdentityLocalizer {
        string StoreNotIUserProfileStore { get; }
    }

    public class IdentityLocalizer : IIdentityLocalizer {
        private readonly IStringLocalizer<Resources> localizer;

        public IdentityLocalizer(IStringLocalizer<Resources> localizer) {
            this.localizer = localizer;
        }

        public string StoreNotIUserProfileStore => GetString(nameof(StoreNotIUserProfileStore));

        private string GetString(string name) {
            return localizer[name];
        }
    }
}
