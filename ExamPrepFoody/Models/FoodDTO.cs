using System.Text.Json.Serialization;

namespace ExamPrepFoody.Models;

public class FoodDTO
{
    [JsonPropertyName ("name")]
    public string Name { get; set; }
    [JsonPropertyName ("description")]
    public string Description { get; set; }
    [JsonPropertyName ("url")]
    public string Url { get; set; }
}