namespace FmeaManager.Domain.AccessControl;

[Flags]
public enum AccessPermission
{
    None = 0,
    Read = 1,
    Edit = 2,
    Approve = 4
}
