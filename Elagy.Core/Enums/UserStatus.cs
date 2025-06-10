/*
namespace Elagy.Core.Enums
{
    public enum UserStatus
    {
        PendingEmailConfirmation = 0, 
        PendingAdminApproval = 1,      
        Active = 2, 
        Deactive = 3,
        Suspended = 4
    }
}
*/

namespace Elagy.Core.Enums
{
    public enum UserStatus
    {
        Pending = 0,         // For Service Providers awaiting SuperAdmin approval (after email confirmation)
        Active = 1,          // For Patients and Service Providers (after approval)
        Deactivated = 2,     // Account deactivated by admin
        EmailUnconfirmed = 3 // Initial state after registration, before email confirmation
    }
}