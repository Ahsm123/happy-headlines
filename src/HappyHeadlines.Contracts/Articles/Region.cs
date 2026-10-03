using System.Text.Json.Serialization;

namespace HappyHeadlines.Contracts.Articles;

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