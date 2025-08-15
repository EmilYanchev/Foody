using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ExamPrepFoody.Models;

internal class ApiResponseDTO
{
    [JsonPropertyName ("msg")]
    public string Msg { get; set; }
    [JsonPropertyName ("foodid")]
    public string FoodId { get; set; }
}