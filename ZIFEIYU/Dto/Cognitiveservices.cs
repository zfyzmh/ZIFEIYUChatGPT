namespace ZIFEIYU.Dto;

using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class Cognitiveservices
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("DisplayName")]
    public string DisplayName { get; set; }

    [JsonProperty("LocalName")]
    public string LocalName { get; set; }

    [JsonProperty("ShortName")]
    public string ShortName { get; set; }

    [JsonProperty("Gender")]
    public Gender Gender { get; set; }

    [JsonProperty("Locale")]
    public string Locale { get; set; }

    [JsonProperty("LocaleName")]
    public string LocaleName { get; set; }

    [JsonProperty("SampleRateHertz")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long SampleRateHertz { get; set; }

    [JsonProperty("VoiceType")]
    public VoiceType VoiceType { get; set; }

    [JsonProperty("Status")]
    public Status Status { get; set; }

    [JsonProperty("WordsPerMinute", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(ParseStringConverter))]
    public long? WordsPerMinute { get; set; }

    [JsonProperty("StyleList", NullValueHandling = NullValueHandling.Ignore)]
    public string[] StyleList { get; set; }

    [JsonProperty("SecondaryLocaleList", NullValueHandling = NullValueHandling.Ignore)]
    public string[] SecondaryLocaleList { get; set; }

    [JsonProperty("RolePlayList", NullValueHandling = NullValueHandling.Ignore)]
    public string[] RolePlayList { get; set; }
}

public enum Gender
{ Female, Male };

public enum Status
{ Deprecated, Ga, Preview };

public enum VoiceType
{ Neural };

public partial class Cognitiveservices
{
    public static Cognitiveservices[] FromJson(string json) => JsonConvert.DeserializeObject<Cognitiveservices[]>(json, Converter.Settings);
}

public static class Serialize
{
    public static string ToJson(this Cognitiveservices[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
            {
                GenderConverter.Singleton,
                StatusConverter.Singleton,
                VoiceTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
}

internal class GenderConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(Gender) || t == typeof(Gender?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "Female":
                return Gender.Female;

            case "Male":
                return Gender.Male;
        }
        throw new Exception("Cannot unmarshal type Gender");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (Gender)untypedValue;
        switch (value)
        {
            case Gender.Female:
                serializer.Serialize(writer, "Female");
                return;

            case Gender.Male:
                serializer.Serialize(writer, "Male");
                return;
        }
        throw new Exception("Cannot marshal type Gender");
    }

    public static readonly GenderConverter Singleton = new GenderConverter();
}

internal class ParseStringConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        long l;
        if (Int64.TryParse(value, out l))
        {
            return l;
        }
        throw new Exception("Cannot unmarshal type long");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (long)untypedValue;
        serializer.Serialize(writer, value.ToString());
        return;
    }

    public static readonly ParseStringConverter Singleton = new ParseStringConverter();
}

internal class StatusConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "Deprecated":
                return Status.Deprecated;

            case "GA":
                return Status.Ga;

            case "Preview":
                return Status.Preview;
        }
        throw new Exception("Cannot unmarshal type Status");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (Status)untypedValue;
        switch (value)
        {
            case Status.Deprecated:
                serializer.Serialize(writer, "Deprecated");
                return;

            case Status.Ga:
                serializer.Serialize(writer, "GA");
                return;

            case Status.Preview:
                serializer.Serialize(writer, "Preview");
                return;
        }
        throw new Exception("Cannot marshal type Status");
    }

    public static readonly StatusConverter Singleton = new StatusConverter();
}

internal class VoiceTypeConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(VoiceType) || t == typeof(VoiceType?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        if (value == "Neural")
        {
            return VoiceType.Neural;
        }
        throw new Exception("Cannot unmarshal type VoiceType");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        var value = (VoiceType)untypedValue;
        if (value == VoiceType.Neural)
        {
            serializer.Serialize(writer, "Neural");
            return;
        }
        throw new Exception("Cannot marshal type VoiceType");
    }

    public static readonly VoiceTypeConverter Singleton = new VoiceTypeConverter();
}