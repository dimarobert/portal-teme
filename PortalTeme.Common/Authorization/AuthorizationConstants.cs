using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Common.Authorization {
    public static class AuthorizationConstants {

        public const string AdministratorPolicy = "AdministratorRights";
        public const string AdministratorRoleName = "Admin";

        public const string CanViewCoursesPolicy = "CanViewCourses";
        public const string CanCreateCoursePolicy = "CanCreateCourse";
        public const string CanUpdateCoursePolicy = "CanUpdateCourse";
        public const string CanDeleteCoursePolicy = "CanDeleteCourse";


        public const string ProfessorRoleName = "Professor";
        public const string AssistantRoleName = "Assistant";
        public const string StudentRoleName = "Student";
    }
}
