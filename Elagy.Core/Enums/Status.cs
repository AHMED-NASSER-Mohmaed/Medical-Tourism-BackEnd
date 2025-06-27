namespace Elagy.Core.Enums
{
    public enum Status
    {
        Deactivated = 0,     // Account deactivated by admin
        Active = 1,          // For Patients and Service Providers (after approval)
        Pending = 2,         // For Service Providers awaiting SuperAdmin approval (after email confirmation)
        EmailUnconfirmed = 3 // Initial state after registration, before email confirmation
    }
}