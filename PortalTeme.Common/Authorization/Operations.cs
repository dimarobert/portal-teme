using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortalTeme.Common.Authorization {
    public static class Operations {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = nameof(Create) };
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = nameof(Read) };
        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = nameof(Update) };
        public static OperationAuthorizationRequirement CourseEditAssistents =
            new OperationAuthorizationRequirement { Name = nameof(CourseEditAssistents) };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = nameof(Delete) };

        public static OperationAuthorizationRequirement ViewGroups =
            new OperationAuthorizationRequirement { Name = nameof(ViewGroups) };
        public static OperationAuthorizationRequirement EditGroups =
            new OperationAuthorizationRequirement { Name = nameof(EditGroups) };

        public static OperationAuthorizationRequirement ViewDomains =
            new OperationAuthorizationRequirement { Name = nameof(ViewDomains) };
        public static OperationAuthorizationRequirement EditDomains =
            new OperationAuthorizationRequirement { Name = nameof(EditDomains) };

        public static OperationAuthorizationRequirement ViewAssignmentEntries =
            new OperationAuthorizationRequirement { Name = nameof(ViewAssignmentEntries) };
        public static OperationAuthorizationRequirement EditAssignmentEntries =
            new OperationAuthorizationRequirement { Name = nameof(EditAssignmentEntries) };
    };
}

