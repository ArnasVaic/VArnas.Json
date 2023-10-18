using VArnas.ParserCombinator.Core;
using VArnas.ParserCombinator.Interfaces;
using static VArnas.ParserCombinator.CharacterParsers;
using static VArnas.ParserCombinator.CommonParsers;

namespace VArnas.Json;

public static class JsonParser
{
    private static readonly IParser<char, JsonNode> Bool = Or(
        String("true").Map(_ => new JsonNode.JsonBool(true)),
        String("false").Map(_ => new JsonNode.JsonBool(false)));

    //private static readonly IParser<char, int> Integer
    
     private static readonly IParser<char, JsonNode> Node = Bool;

     public static IEither<string, IParseResult<char, JsonNode>> Parse(string input) =>
         Bool.Parse(new ParserInput<char>(input.ToArray(), 0));
}