using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PortalTeme.Auth.Areas.Identity.Stores;
using PortalTeme.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Managers {
    public class ApplicationUserManager : UserManager<User> {
        private readonly IIdentityLocalizer localizer;

        public ApplicationUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            IIdentityLocalizer localizer)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
            this.localizer = localizer;
        }

        public virtual async Task<string> GetFirstNameAsync(User user) {
            ThrowIfDisposed();
            var store = GetUserProfileStore();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            return await store.GetFirstNameAsync(user, CancellationToken);
        }

        public virtual async Task<IdentityResult> SetFirstNameAsync(User user, string firstName) {
            ThrowIfDisposed();
            var store = GetUserProfileStore();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            await store.SetFirstNameAsync(user, firstName, CancellationToken);
            return await UpdateUserAsync(user);
        }


        public async Task<string> GetLastNameAsync(User user) {
            ThrowIfDisposed();
            var store = GetUserProfileStore();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            return await store.GetLastNameAsync(user, CancellationToken);
        }

        public virtual async Task<IdentityResult> SetLastNameAsync(User user, string lastName) {
            ThrowIfDisposed();
            var store = GetUserProfileStore();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            await store.SetLastNameAsync(user, lastName, CancellationToken);
            return await UpdateUserAsync(user);
        }


        private IUserProfileStore<User> GetUserProfileStore(bool throwOnFail = false) {
            var cast = Store as IUserProfileStore<User>;
            if (throwOnFail && cast == null) {
                throw new NotSupportedException(localizer.StoreNotIUserProfileStore);
            }
            return cast;
        }
    }
}
