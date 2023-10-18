namespace VArnas.Json;

public abstract record JsonNode
{
    public record JsonNumber(decimal Value) : JsonNode;
    public record JsonBool(bool Value) : JsonNode;
    public record JsonString(string Value) : JsonNode;
    
    public record JsonList(JsonNode[] Values) : JsonNode;
    
    public record JsonDict(Dictionary<JsonString, JsonNode> Values) : JsonNode;
}
//
// public abstract class JsonNode;
//
// public class JsonBool(bool value) : JsonNode
// {
//     public bool Value { get; } = value;
// }