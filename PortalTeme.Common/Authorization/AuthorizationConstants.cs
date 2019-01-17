using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace PortalTeme.Common.Authorization {
    public static class AuthorizationConstants {

        public const string AdministratorPolicy = "AdministratorRights";

        public const string CanViewCoursePolicy = "CanViewCourse";
        public const string CanCreateCoursePolicy = "CanCreateCourse";
        public const string CanUpdateCoursePolicy = "CanUpdateCourse";
        public const string CanEditCourseAssistantsPolicy = "CanEditCourseAssistants";
        public const string CanDeleteCoursePolicy = "CanDeleteCourse";

        public const string CanViewAssignmentEntriesPolicy = "CanViewAssignmentEntries";
        public const string CanEditAssignmentEntriesPolicy = "CanEditAssignmentEntries";


        public const string CanViewGroupsPolicy = "CanViewGroups";
        public const string CanEditGroupsPolicy = "CanEditGroups";

        public const string CanViewStudyDomainsPolicy = "CanViewStudyDomains";
        public const string CanEditStudyDomainsPolicy = "CanEditStudyDomains";


        public const string AdministratorRoleName = "Admin";
        public const string ProfessorRoleName = "Professor";
        public const string AssistantRoleName = "Assistant";
        public const string StudentRoleName = "Student";

    }
}
