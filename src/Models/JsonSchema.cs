namespace JSONRising.Models;

public class JsonSchema 
{
    // Mod Metadata
    public string Id { get; set; }
    public string Name { get; set; }
    public string? SubName { get; set; }
    public string? Category { get; set; }
    public string IconUrl { get; set; }
    public string? DownloadUrl { get; set; }
    
    // Schema Definition
    public List<SchemaField> Fields { get; set; }
}

public class SchemaField
{
    public string Name { get; set; }
    public string Path { get; set; }
    public string ComponentType { get; set; }
    public Dictionary<string, object> ComponentProps { get; set; }
}