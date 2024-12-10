using System.Net.Http.Json;
using JSONRising.Models;
using Microsoft.JSInterop;

namespace JSONRising.Services;

public interface ISchemaService
{
    Task<List<JsonSchema>> GetAllSchemas();
    Task<JsonSchema?> GetSchemaById(string id);
}

public class SchemaService : ISchemaService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<SchemaService> _logger;

    public SchemaService(IJSRuntime jsRuntime, ILogger<SchemaService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }
      public async Task<List<JsonSchema>> GetAllSchemas()
      {
          try
          {
              var schemaFiles = await _jsRuntime.InvokeAsync<string[]>("window.__TAURI__.core.invoke", "list_schema_files");
              _logger.LogInformation($"Found schema files: {string.Join(", ", schemaFiles)}");

              var schemas = new List<JsonSchema>();
              var options = new System.Text.Json.JsonSerializerOptions
              {
                  PropertyNameCaseInsensitive = true,
                  PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
              };

              foreach (var file in schemaFiles)
              {
                  var jsonString = await _jsRuntime.InvokeAsync<string>("window.__TAURI__.core.invoke", "load_schema_file", new { path = $"{file}" });
                  _logger.LogInformation($"Raw JSON string: {jsonString}");
                
                  var schema = System.Text.Json.JsonSerializer.Deserialize<JsonSchema>(jsonString, options);
                  _logger.LogInformation($"Deserialized schema - Name: {schema.Name}, Id: {schema.Id}");
                  schemas.Add(schema);
              }

              return schemas;
          }
          catch (Exception ex)
          {
              _logger.LogError($"Error in GetAllSchemas: {ex.Message}");
              return new List<JsonSchema>();
          }
    }

    public async Task<JsonSchema?> GetSchemaById(string id)
    {
        try
        {
            var schemas = await GetAllSchemas();
            return schemas.FirstOrDefault(s => s.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetSchemaById: {ex.Message}");
            return null;
        }
    }
}