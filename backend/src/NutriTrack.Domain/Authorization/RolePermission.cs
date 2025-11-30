namespace NutriTrack.Domain.Authorization;

public class RolePermission
{
    public RoleId RoleId { get; set; } = default!;
    public PermissionId PermissionId { get; set; } = default!;

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}