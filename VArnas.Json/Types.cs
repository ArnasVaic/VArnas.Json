namespace VArnas.Json;

public abstract record JsonNode
{
    public record JsonNull : JsonNode;
    
    public record JsonBool(bool Value) : JsonNode;
    
    public record JsonNumber(decimal Value) : JsonNode;
    
    public record JsonString(string Value) : JsonNode;
    
    public record JsonList(JsonNode[] Values) : JsonNode;
    
    public record JsonDict(Dictionary<JsonString, JsonNode> Values) : JsonNode;
}