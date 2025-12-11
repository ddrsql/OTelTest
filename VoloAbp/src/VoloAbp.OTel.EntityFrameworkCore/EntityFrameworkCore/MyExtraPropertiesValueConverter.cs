using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.ObjectExtending;

namespace VoloAbp.OTel.EntityFrameworkCore;

public class MyExtraPropertiesValueConverter<TEntityType> : ValueConverter<ExtraPropertyDictionary, string>
{
    public MyExtraPropertiesValueConverter()
        : base(
            d => SerializeObject(d),
            s => DeserializeObject(s))
    {

    }

    public readonly static JsonSerializerOptions SerializeOptions = new JsonSerializerOptions()
    {
        // 关键：允许不安全的字符（包括中文）不被转义
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // 保持 ABP 默认的驼峰命名（可选，为了保持一致性）
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // 如果你想省库空间，设为 true 跳过 null；否则保持默认
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private static string SerializeObject(ExtraPropertyDictionary extraProperties)
    {
        var copyDictionary = new Dictionary<string, object?>(extraProperties);

        var entityType = typeof(TEntityType);
        if (entityType != null)
        {
            var objectExtension = ObjectExtensionManager.Instance.GetOrNull(entityType);
            if (objectExtension != null)
            {
                foreach (var property in objectExtension.GetProperties())
                {
                    if (property.IsMappedToFieldForEfCore())
                    {
                        copyDictionary.Remove(property.Name);
                    }
                }
            }
        }

        return JsonSerializer.Serialize(copyDictionary, SerializeOptions);
    }

    public readonly static JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions()
    {
        Converters =
        {
            new ObjectToInferredTypesConverter()
        }
    };

    private static ExtraPropertyDictionary DeserializeObject(string extraPropertiesAsJson)
    {
        if (extraPropertiesAsJson.IsNullOrEmpty() || extraPropertiesAsJson == "{}")
        {
            return new ExtraPropertyDictionary();
        }

        var dictionary = JsonSerializer.Deserialize<ExtraPropertyDictionary>(extraPropertiesAsJson, DeserializeOptions) ??
                            new ExtraPropertyDictionary();

        var entityType = typeof(TEntityType);
        if (entityType != null)
        {
            var objectExtension = ObjectExtensionManager.Instance.GetOrNull(entityType);
            if (objectExtension != null)
            {
                foreach (var property in objectExtension.GetProperties())
                {
                    dictionary[property.Name] = GetNormalizedValue(dictionary!, property);
                }
            }
        }

        return dictionary;
    }

    private static object? GetNormalizedValue(
        Dictionary<string, object> dictionary,
        ObjectExtensionPropertyInfo property)
    {
        var value = dictionary.GetOrDefault(property.Name);
        if (value == null)
        {
            return property.GetDefaultValue();
        }

        try
        {
            if (property.Type.IsEnum)
            {
                return Enum.Parse(property.Type, value.ToString()!, true);
            }

            //return Convert.ChangeType(value, property.Type);
            return value;
        }
        catch
        {
            return value;
        }
    }
}