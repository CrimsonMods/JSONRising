namespace JSONRising.Models;

public class ModDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
public class JsonTemplateInfo
{
    public required string Name { get; set; }
    public required string Schema { get; set; }
    public required List<string> Examples { get; set; }
}
