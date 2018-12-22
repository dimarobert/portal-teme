using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalTeme.Data.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortalTeme.Auth.Areas.Identity.Stores {

    public interface IUserProfileStore<TUser> where TUser : class {
        Task<string> GetFirstNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetLastNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        Task SetFirstNameAsync(TUser user, string firstName, CancellationToken cancellationToken = default(CancellationToken));
        Task SetLastNameAsync(TUser user, string lastName, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ApplicationUserStore : UserStore<User>, IUserProfileStore<User> {
        public ApplicationUserStore(IdentityContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }

        public Task<string> GetFirstNameAsync(User user, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.FirstName);
        }

        public Task<string> GetLastNameAsync(User user, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.LastName);
        }

        public Task SetFirstNameAsync(User user, string firstName, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            user.FirstName = firstName;
            return Task.CompletedTask;
        }

        public Task SetLastNameAsync(User user, string lastName, CancellationToken cancellationToken = default(CancellationToken)) {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) {
                throw new ArgumentNullException(nameof(user));
            }
            user.LastName = lastName;
            return Task.CompletedTask;
        }
    }
}
