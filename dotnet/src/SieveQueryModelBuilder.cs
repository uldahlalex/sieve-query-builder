using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SieveQueryBuilder;

/// <summary>
/// Builder for creating query model configuration manually
/// Provides a single source of truth for both SieveProcessor configuration and Query Model generation
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public class SieveQueryModelBuilder<TEntity> where TEntity : class
{
    private readonly List<SievePropertyInfo> _properties = new();
    private readonly string _entityName;

    public SieveQueryModelBuilder()
    {
        _entityName = typeof(TEntity).Name;
    }

    /// <summary>
    /// Add a property with both filter and sort capabilities
    /// </summary>
    public SieveQueryModelBuilder<TEntity> AddProperty<TProp>(
        string propertyName,
        Type? propertyType = null,
        bool canFilter = true,
        bool canSort = true)
    {
        _properties.Add(new SievePropertyInfo
        {
            PropertyName = propertyName,
            PropertyType = propertyType ?? typeof(TProp),
            CanFilter = canFilter,
            CanSort = canSort
        });
        return this;
    }

    /// <summary>
    /// Get all configured properties
    /// </summary>
    public List<SievePropertyInfo> GetProperties() => _properties;

    /// <summary>
    /// Generate C# code for the query model class
    /// </summary>
    public string GenerateQueryModelCode(string? customName = null)
    {
        var modelName = customName ?? $"{_entityName}QueryModel";
        var code = new StringBuilder();

        code.AppendLine("using SieveQueryBuilder;");
        code.AppendLine();
        code.AppendLine($"/// <summary>");
        code.AppendLine($"/// Query model for {_entityName} with configured filterable and sortable properties");
        code.AppendLine($"/// </summary>");
        code.AppendLine($"public class {modelName} : ISieveQueryModel");
        code.AppendLine("{");

        foreach (var prop in _properties.OrderBy(p => p.PropertyName))
        {
            var typeName = GetCSharpTypeName(prop.PropertyType);
            var capabilities = new List<string>();
            if (prop.CanFilter) capabilities.Add("Filter");
            if (prop.CanSort) capabilities.Add("Sort");

            code.AppendLine($"    /// <summary>");
            code.AppendLine($"    /// Can {string.Join(", ", capabilities)}");
            code.AppendLine($"    /// </summary>");
            code.AppendLine($"    public {typeName}? {prop.PropertyName} {{ get; set; }}");
            code.AppendLine();
        }

        code.AppendLine("}");

        return code.ToString();
    }

    /// <summary>
    /// Generate SieveProcessor MapProperties method code
    /// </summary>
    public string GenerateSieveProcessorCode()
    {
        var code = new StringBuilder();

        code.AppendLine($"// Configure {_entityName} entity");
        foreach (var prop in _properties.OrderBy(p => p.PropertyName))
        {
            code.AppendLine($"mapper.Property<{_entityName}>(e => e.{prop.PropertyName})");
            if (prop.CanFilter)
            {
                code.AppendLine("    .CanFilter()");
            }
            if (prop.CanSort)
            {
                code.AppendLine("    .CanSort()");
            }
            code.AppendLine("    ;");
            code.AppendLine();
        }

        return code.ToString();
    }

    private static string GetCSharpTypeName(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        // Map common types to C# keywords
        if (underlyingType == typeof(string)) return "string";
        if (underlyingType == typeof(int)) return "int";
        if (underlyingType == typeof(long)) return "long";
        if (underlyingType == typeof(bool)) return "bool";
        if (underlyingType == typeof(DateTime)) return "DateTime";
        if (underlyingType == typeof(decimal)) return "decimal";
        if (underlyingType == typeof(double)) return "double";
        if (underlyingType == typeof(float)) return "float";
        if (underlyingType == typeof(Guid)) return "Guid";

        return underlyingType.Name;
    }
}
