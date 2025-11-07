namespace Bcp.Exchange.FunctionApp.Authorization;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RequireRoleAttribute : Attribute
{
    public string Role { get; }

    public RequireRoleAttribute(string role)
    {
        Role = role;
    }
}

public class RequireAdminAttribute : RequireRoleAttribute
{
    public RequireAdminAttribute()
        : base("Admin") { }
}

public class RequireViewerAttribute : RequireRoleAttribute
{
    public RequireViewerAttribute()
        : base("Viewer") { }
}
