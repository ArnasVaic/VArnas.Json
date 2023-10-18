using VArnas.Json;
using VArnas.ParserCombinator.Core;

namespace VArnas.UnitTests.Simple;

public class BoolTests
{
    [Fact]
    public void True()
    {
        var result = JsonParser.Parse("true");
        
        Assert.True(result.IsRight);
        result.Match(
            error => throw new Exception(error),
            r =>
            {
                Assert.Equal(new JsonNode.JsonBool(true), r.Result);
                return new Unit();
            });
    }
}