using System.Text.Json.Serialization;

namespace ServiceDefaults.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter<Region>))]
public enum Region
{
    Africa,
    Antarctica,
    Asia,
    Europe,
    Global,
    NorthAmerica,
    Oceania,
    SouthAmerica
}