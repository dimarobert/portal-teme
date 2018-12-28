using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Services {
    public class AppInitialization {

        private readonly IServiceProvider services;
        private bool? isInitialized = null;

        public AppInitialization(IServiceProvider services) {
            this.services = services;
        }

        public void Reset() {
            isInitialized = null;
        }

        public async Task<bool> IsInitialized() {
            if (isInitialized is null)
                await GetFromDatabase();

            return isInitialized.Value;
        }

        private async Task GetFromDatabase() {
            using (var scope = services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
                isInitialized = await context.Users.AnyAsync();
            }
        }

    }
}
