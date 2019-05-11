using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortalTeme.Common.Caching {

    public interface IExtendedDistributedCache : IDistributedCache {
        void Set(string key, byte[] value, ExtendedDistributedCacheEntryOptions options);
        Task SetAsync(string key, byte[] value, ExtendedDistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken));
    }

    public class CacheKeyDependency {
        public string Key { get; set; }

        public RegistrationBehavior RegisterBehavior { get; set; }

        public static CacheKeyDependency WithKey(string key) => new CacheKeyDependency { Key = key };

        public CacheKeyDependency WithAlwaysBehavior() {
            RegisterBehavior = RegistrationBehavior.Always;
            return this;
        }
    }

    public enum RegistrationBehavior {
        ThrowNotFound = 0,

        /// <summary>
        /// This will always register the dependency, even if it is not present yet.
        /// WARNING: The cache may never be removed if the dependency key is never added to cache.
        /// </summary>
        Always
    }

    public class ExtendedDistributedCacheEntryOptions : DistributedCacheEntryOptions {

        public IList<CacheKeyDependency> CacheKeyDependecies { get; private set; }

        public ExtendedDistributedCacheEntryOptions() {
            CacheKeyDependecies = new List<CacheKeyDependency>();
        }

        public ExtendedDistributedCacheEntryOptions AddDependencyKey(string key) {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("The dependent cache key must be non whitespace.", nameof(key));

            return AddDependencyKey(new CacheKeyDependency {
                Key = key
            });
        }

        public ExtendedDistributedCacheEntryOptions AddDependencyKey(CacheKeyDependency dependency) {
            if (dependency is null)
                throw new ArgumentNullException(nameof(dependency));

            CacheKeyDependecies.Add(dependency);

            return this;
        }

        public ExtendedDistributedCacheEntryOptions AddDependencyKeys(IEnumerable<string> keys) {
            return AddDependencyKeys(keys.Select(key => new CacheKeyDependency { Key = key }));
        }

        public ExtendedDistributedCacheEntryOptions AddDependencyKeys(IEnumerable<CacheKeyDependency> dependencies) {
            foreach (var dependency in dependencies)
                AddDependencyKey(dependency);

            return this;
        }

        public ExtendedDistributedCacheEntryOptions AddDependencyKeys(params string[] keys) {
            return AddDependencyKeys(keys as IEnumerable<string>);
        }

        public ExtendedDistributedCacheEntryOptions AddDependencyKeys(params CacheKeyDependency[] dependencies) {
            return AddDependencyKeys(dependencies as IEnumerable<CacheKeyDependency>);
        }

        public static ExtendedDistributedCacheEntryOptions New { get { return new ExtendedDistributedCacheEntryOptions(); } }

    }

    public class DependencyToken : IDisposable {
        private CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();

        public bool IsPreRegistered { get; set; }

        internal void Register(Action<object> onDependencyRemoved, string key) {
            TokenSource.Token.Register(onDependencyRemoved, key);
        }

        internal void Cancel() {
            TokenSource.Cancel();
        }

        public void Dispose() {
            TokenSource.Dispose();
        }
    }

    public class ExtendedMemoryDistributedCache : IExtendedDistributedCache {

        private readonly IMemoryCache _memCache;
        private readonly ConcurrentDictionary<string, DependencyToken> _dependencyTokens;


        public ExtendedMemoryDistributedCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor) {
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _memCache = new MemoryCache(optionsAccessor.Value);
            _dependencyTokens = new ConcurrentDictionary<string, DependencyToken>();
        }

        public byte[] Get(string key) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return (byte[])_memCache.Get(key);
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = default) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return Task.FromResult(Get(key));
        }

        public void Set(string key, byte[] value, ExtendedDistributedCacheEntryOptions options) {
            Set(key, value, options as DistributedCacheEntryOptions);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration,
                Size = value.Length
            };

            if (options is ExtendedDistributedCacheEntryOptions extendedOptions) {
                foreach (var depKey in extendedOptions.CacheKeyDependecies) {
                    var depToken = _dependencyTokens.GetOrAdd(depKey.Key, (_) => {
                        if (depKey.RegisterBehavior == RegistrationBehavior.ThrowNotFound)
                            throw new Exception($"One of the provided cache dependecy keys is not present in the cache. Key: {depKey}.");

                        return new DependencyToken() { IsPreRegistered = true };
                    });

                    depToken.Register(OnDependencyRemoved, key);
                }
            }

            var token = _dependencyTokens.AddOrUpdate(key, new DependencyToken(), (_, oldToken) => {
                oldToken.Cancel();
                return new DependencyToken();
            });

            memoryCacheEntryOptions.RegisterPostEvictionCallback(OnPostEviction, token);

            _memCache.Set(key, value, memoryCacheEntryOptions);
        }

        public async Task SetAsync(string key, byte[] value, ExtendedDistributedCacheEntryOptions options, CancellationToken token = default) {
            await SetAsync(key, value, options as DistributedCacheEntryOptions, token);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Refresh(string key) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _memCache.TryGetValue(key, out _);
        }

        public Task RefreshAsync(string key, CancellationToken token = default) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Refresh(key);
            return Task.CompletedTask;
        }

        public void Remove(string key) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _memCache.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Remove(key);
            return Task.CompletedTask;
        }


        private void OnDependencyRemoved(object dependentKey) {
            _memCache.Remove(dependentKey);
        }

        private void OnPostEviction(object key, object value, EvictionReason reason, object state) {
            var strKey = key as string ?? throw new ArgumentException("Invalid key received. The key must be a string.", nameof(key));
            if (string.IsNullOrWhiteSpace(strKey))
                throw new ArgumentException("Invalid key received. The key must be a non whitespace string.", nameof(key));

            var token = state as DependencyToken ?? throw new ArgumentException("Invalid state object received. The state must be of type DependencyToken.", nameof(state));
            token.Cancel();
            token.Dispose();
            _dependencyTokens.TryRemove(strKey, out var _);
        }
    }

    public static class ExtendedMemoryDistributedCacheExtensions {
        public static async Task SetStringAsync(this IExtendedDistributedCache cache, string key, string value, ExtendedDistributedCacheEntryOptions options, CancellationToken token = default) {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            await cache.SetAsync(key, Encoding.UTF8.GetBytes(value), options, token);
        }

        public static void SetString(this IExtendedDistributedCache cache, string key, string value, ExtendedDistributedCacheEntryOptions options) {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            cache.Set(key, Encoding.UTF8.GetBytes(value), options);
        }
    }
}
