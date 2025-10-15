using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sieve.Services;

namespace SieveQueryBuilder;

/// <summary>
/// Information about a property configured in a SieveProcessor
/// </summary>
public class SievePropertyInfo
{
    public string PropertyName { get; set; } = string.Empty;
    public Type PropertyType { get; set; } = typeof(object);
    public bool CanFilter { get; set; }
    public bool CanSort { get; set; }
}

/// <summary>
/// Generates query model information from SieveProcessor configuration using reflection
/// </summary>
public static class SieveQueryModelGenerator
{
    /// <summary>
    /// Discovers all configured properties for an entity from a SieveProcessor
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="processor">The configured SieveProcessor instance</param>
    /// <returns>List of property information</returns>
    /// <remarks>
    /// This method uses reflection to access Sieve's internal property mapper.
    /// It may not work with all versions of Sieve due to internal API changes.
    /// Consider using manual property configuration instead.
    /// </remarks>
    public static List<SievePropertyInfo> DiscoverProperties<TEntity>(SieveProcessor processor)
        where TEntity : class
    {
        var entityType = typeof(TEntity);
        var properties = new List<SievePropertyInfo>();

        try
        {
            // Try to access the internal property mapper
            var processorType = processor.GetType();

            // Look for _mapper or _options fields
            FieldInfo? mapperField = null;
            var currentType = processorType;

            while (currentType != null && mapperField == null)
            {
                mapperField = currentType.GetField("_mapper", BindingFlags.NonPublic | BindingFlags.Instance);
                currentType = currentType.BaseType;
            }

            if (mapperField == null)
            {
                // Cannot access internal mapper - return empty list
                // Users will need to manually define query models
                return properties;
            }

            var mapper = mapperField.GetValue(processor);
            if (mapper == null)
            {
                return properties;
            }

            // Try different possible property names for the configurations dictionary
            var mapperType = mapper.GetType();
            PropertyInfo? configurationsProperty = null;

            foreach (var propName in new[] { "PropertyConfigurations", "Configurations", "_configurations" })
            {
                configurationsProperty = mapperType.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (configurationsProperty != null) break;
            }

            if (configurationsProperty == null)
            {
                return properties;
            }

            var configurations = configurationsProperty.GetValue(mapper) as System.Collections.IDictionary;
            if (configurations == null)
            {
                return properties;
            }

            // Iterate through configurations
            foreach (var key in configurations.Keys)
            {
                try
                {
                    var keyType = key.GetType();

                    // Check if this configuration is for our entity type
                    Type? keyEntityType = null;
                    var entityTypeProperty = keyType.GetProperty("EntityType", BindingFlags.Public | BindingFlags.Instance);
                    if (entityTypeProperty != null)
                    {
                        keyEntityType = entityTypeProperty.GetValue(key) as Type;
                    }

                    if (keyEntityType != entityType)
                    {
                        continue;
                    }

                    var config = configurations[key];
                    if (config == null)
                    {
                        continue;
                    }

                    var configType = config.GetType();

                    // Get property name
                    var nameProperty = configType.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                    var propertyName = nameProperty?.GetValue(config) as string;
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        continue;
                    }

                    // Get filter/sort capabilities
                    var canFilterProperty = configType.GetProperty("CanFilter", BindingFlags.Public | BindingFlags.Instance);
                    var canSortProperty = configType.GetProperty("CanSort", BindingFlags.Public | BindingFlags.Instance);

                    var canFilter = canFilterProperty?.GetValue(config) as bool? ?? false;
                    var canSort = canSortProperty?.GetValue(config) as bool? ?? false;

                    // Try to determine property type from the entity
                    Type propertyType = typeof(object);
                    var entityProperty = entityType.GetProperty(propertyName);
                    if (entityProperty != null)
                    {
                        propertyType = entityProperty.PropertyType;
                    }

                    properties.Add(new SievePropertyInfo
                    {
                        PropertyName = propertyName,
                        PropertyType = propertyType,
                        CanFilter = canFilter,
                        CanSort = canSort
                    });
                }
                catch
                {
                    // Skip properties that fail to parse
                    continue;
                }
            }
        }
        catch
        {
            // If reflection fails, return empty list
            // Users should manually create query models
        }

        return properties;
    }

    /// <summary>
    /// Generates C# code for a query model class based on SieveProcessor configuration
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <param name="processor">The configured SieveProcessor instance</param>
    /// <param name="queryModelName">Name for the generated query model class</param>
    /// <returns>C# code as a string</returns>
    public static string GenerateQueryModelCode<TEntity>(SieveProcessor processor, string? queryModelName = null)
        where TEntity : class
    {
        var entityType = typeof(TEntity);
        var modelName = queryModelName ?? $"{entityType.Name}QueryModel";
        var properties = DiscoverProperties<TEntity>(processor);

        var code = new System.Text.StringBuilder();
        code.AppendLine("using SieveQueryBuilder;");
        code.AppendLine();
        code.AppendLine($"/// <summary>");
        code.AppendLine($"/// Auto-generated query model for {entityType.Name}");
        code.AppendLine($"/// </summary>");
        code.AppendLine($"public class {modelName} : ISieveQueryModel");
        code.AppendLine("{");

        foreach (var prop in properties.OrderBy(p => p.PropertyName))
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

    private static string GetCSharpTypeName(Type type)
    {
        // Handle nullable types
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
