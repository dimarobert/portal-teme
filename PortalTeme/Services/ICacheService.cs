using Microsoft.Extensions.Caching.Distributed;
using PortalTeme.API.Controllers;
using PortalTeme.API.Models;
using PortalTeme.API.Models.Courses;
using PortalTeme.Common.Caching;
using PortalTeme.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTeme.Services {
    public interface ICacheService {

        Task<List<CourseEditDTO>> GetCoursesRefAsync();
        Task SetCoursesRefAsync(List<CourseEditDTO> courses);

        Task<CourseViewDTO> GetCourseBySlugAsync(string slug);
        Task SetCourseBySlugAsync(CourseViewDTO course);

        Task<List<UserDTO>> GetCourseMembersAsync(Guid courseId);
        Task SetCourseMembersAsync(Guid courseId, List<UserDTO> members);

        Task ClearCoursesRefCacheAsync();
        Task ClearCourseDefinitionCacheAsync(Guid courseDefId);
        Task ClearCourseCacheAsync(Guid courseId, string slug);
    }

    public class DistributedCacheService : ICacheService {
        private readonly IExtendedDistributedCache cache;
        private readonly IJsonSerializer jsonSerializer;

        private const string coursesMasterCacheKey = nameof(Course);

        public DistributedCacheService(IExtendedDistributedCache cache, IJsonSerializer jsonSerializer) {
            this.cache = cache;
            this.jsonSerializer = jsonSerializer;
        }

        public async Task<List<CourseEditDTO>> GetCoursesRefAsync() {
            var cacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCoursesRef)}";
            var cachedCourses = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedCourses)) {
                return jsonSerializer.Deserialize<List<CourseEditDTO>>(cachedCourses);
            }

            return null;
        }

        public async Task SetCoursesRefAsync(List<CourseEditDTO> courses) {
            var cacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCoursesRef)}";
            await EnsureCoursesMasterCacheKey();
            await cache.SetStringAsync(cacheKey, jsonSerializer.Serialize(courses), ExtendedDistributedCacheEntryOptions.New.AddDependencyKey(CacheKeyDependency.WithKey(coursesMasterCacheKey)));
        }


        public async Task<CourseViewDTO> GetCourseBySlugAsync(string slug) {
            var cacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCourseBySlug)}_{slug}";
            var cachedCourse = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedCourse)) {
                return jsonSerializer.Deserialize<CourseViewDTO>(cachedCourse);
            }

            return null;
        }

        public async Task SetCourseBySlugAsync(CourseViewDTO course) {
            var idCacheKey = await EnsureCourseCacheKey(course.Id.Value);

            var cacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCourseBySlug)}_{course.CourseDef.Slug}";

            await cache.SetStringAsync(
                cacheKey,
                jsonSerializer.Serialize(course),
                ExtendedDistributedCacheEntryOptions.New.AddDependencyKey(CacheKeyDependency.WithKey(idCacheKey))
            );
        }


        public async Task<List<UserDTO>> GetCourseMembersAsync(Guid courseId) {
            var cacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCourseMembers)}_{courseId}";
            var cachedMembers = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedMembers)) {
                return jsonSerializer.Deserialize<List<UserDTO>>(cachedMembers);
            }

            return null;
        }

        public async Task SetCourseMembersAsync(Guid courseId, List<UserDTO> members) {
            var idCacheKey = await EnsureCourseCacheKey(courseId);

            var cacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCourseMembers)}_{courseId}";
            await cache.SetStringAsync(cacheKey, jsonSerializer.Serialize(members),
                ExtendedDistributedCacheEntryOptions.New.AddDependencyKey(CacheKeyDependency.WithKey(idCacheKey))
            );
        }


        public async Task ClearCourseDefinitionCacheAsync(Guid courseDefId) {
            var courseDefCacheKey = $"{nameof(CourseDefinitionsController)}_{nameof(CourseDefinitionsController.GetCourseDefinition)}_{courseDefId}";

            await cache.RemoveAsync(courseDefCacheKey);
            await ClearCoursesRefCacheAsync();
        }

        public async Task ClearCourseCacheAsync(Guid courseId, string slug) {
            var idCacheKey = await EnsureCourseCacheKey(courseId);
            await cache.RemoveAsync(idCacheKey);

            await ClearCoursesRefCacheAsync();
        }

        public async Task ClearCoursesRefCacheAsync() {
            var coursesRefCacheKey = $"{nameof(CoursesController)}_{nameof(CoursesController.GetCoursesRef)}";
            await cache.RemoveAsync(coursesRefCacheKey);
        }

        private async Task EnsureCoursesMasterCacheKey() {
            var data = await cache.GetAsync(coursesMasterCacheKey);
            if (data is null)
                await cache.SetAsync(coursesMasterCacheKey, new byte[0]);
        }

        private async Task<string> EnsureCourseCacheKey(Guid courseId) {
            await EnsureCoursesMasterCacheKey();

            var idCacheKey = $"{nameof(Course)}_{courseId}";
            var data = await cache.GetAsync(idCacheKey);
            if (data is null)
                await cache.SetAsync(
                    idCacheKey,
                    new byte[0],
                    ExtendedDistributedCacheEntryOptions.New
                        .AddDependencyKey(CacheKeyDependency.WithKey(coursesMasterCacheKey))
                );

            return idCacheKey;
        }
    }
}
