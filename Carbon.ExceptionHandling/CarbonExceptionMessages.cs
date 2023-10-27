using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.ExceptionHandling.Abstractions
{
    public static class CarbonExceptionMessages
    {
        public static readonly string OperationForbidden = "This Operation is Forbidden!";
        public static readonly string OperationUnauthorized = "This Operation is Unauthorized!";
        public static readonly string OnlyGodUserOperation = "Only godusers are allowed for this operation";
        public static readonly string SolutionHeaderMustBeSet = "Solution Header [p360-solution-id] must be set!";
        public static readonly string OnlyAdminUserOperation = "Only admin users are allowed for this operation";
        public static readonly string OnlySuperAdminUserOperation = "Only superadmin users are allowed for this operation";
    }
}
