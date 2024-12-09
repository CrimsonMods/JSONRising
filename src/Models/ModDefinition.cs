namespace JSONRising.Models;

public class ModDefinition
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? IconUrl { get; set; }
    public string? ThunderstoreUrl { get; set; }
    public bool IsDynamicMod { get; set; } = false;
}
