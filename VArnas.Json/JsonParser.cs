using System.Text;
using VArnas.ParserCombinator.Core;
using VArnas.ParserCombinator.Interfaces;
using static VArnas.ParserCombinator.CharacterParsers;
using static VArnas.ParserCombinator.CommonParsers;
using static VArnas.ParserCombinator.Core.Parser;

namespace VArnas.Json;

public static class JsonParser
{
    private static readonly Unit Unit = new();

    private static IParser<byte, IEnumerable<byte>> ByteString(ReadOnlySpan<byte> bytes) => 
        Sequence(bytes.ToArray().Select(Symbol).ToArray());

    private static Parser<byte, byte> OneOfU8(ReadOnlySpan<byte> bytes) => 
        OneOf(bytes.ToArray().Select(Symbol).ToArray());
    
    private static readonly IParser<byte, JsonNode> Bool = Or(
        ByteString("true"u8).Map(_ => new JsonNode.JsonBool(true)),
        ByteString("false"u8).Map(_ => new JsonNode.JsonBool(false)));
    
    private static readonly IParser<byte, Unit> Ws = 
        Some(OneOf(new[]
        {
            Symbol((byte)' '),
            Symbol((byte)'\n'),
            Symbol((byte)'\r'),
            Symbol((byte)'\t')
        })).Map(_ => Unit);

    private static readonly Parser<byte, bool> Sign = OneOf(new [] {
        Symbol((byte)'+').Map(_ => false),
        Symbol((byte)'-').Map(_ => true),
        Pure<byte, bool>(false)
    });

    private static readonly IParser<byte, byte> OneNine = 
        OneOf("123456789"u8.ToArray().Select(Symbol).ToArray());
    
    private static readonly IParser<byte, byte> Digit = Or(
        Symbol((byte)'0'), 
        OneNine);

    private static readonly IParser<byte, IEnumerable<byte>> Digits = Many(Digit);

    private static readonly IParser<byte, IEnumerable<byte>> Exponent = Or( 
        OneOf("Ee") .Bind(_ => 
        Sign        .Bind(sgn => 
        Digits      .Map<IEnumerable<byte>>(digits =>
        {
            var result = new List<byte>();
            if (sgn) result.Add((byte)'-');
            result.AddRange(digits);
            return result;
        }))), 
        Pure<char, IEnumerable<char>>(Array.Empty<char>()));

    private static readonly IParser<char, IEnumerable<char>> Fraction = Or(
        Symbol('.').Bind(_ => Digits), 
        Pure<char, IEnumerable<char>>(Array.Empty<char>()));

    private static readonly Parser<char, IEnumerable<char>> Integer = OneOf(new[]
    {
        Digit       .Map(d => new List<char>{ d }),
        
        OneNine     .Bind(fst => 
        Digits      .Map(rest => new List<char> { fst }.Concat(rest))),
        
        Symbol('-') .Bind(_ => 
        Digit       .Map(digit => new List<char>{ '-', digit })),
        
        Symbol('-') .Bind(_ => 
        OneNine     .Bind(fst => 
        Digits      .Map(rest => new List<char> { '-', fst }.Concat(rest))))
        
    });

    private static readonly IParser<char, JsonNode.JsonNumber> Number =
        Integer  .Bind(integer =>
        Fraction .Bind(fraction =>
        Exponent .Map(exponent =>
        {
            var concat = integer.Concat(fraction).Concat(exponent).ToArray();
            var value = decimal.Parse(concat);
            return new JsonNode.JsonNumber(value);
        })));

    private static readonly IParser<byte, byte> Hex = 
        Satisfy<byte>(b => char.IsAsciiHexDigit((char)b));

    private static List<T> Wrap<T>(T any) => new() { any };
    
    private static readonly IParser<byte, char> Escape = Or(
        OneOfU8("\"\\bfnrtu"u8).Map(Convert.ToChar),
        
        Symbol((byte)'u')   .Bind(_ => 
        Hex                 .Bind(_1 =>
        Hex                 .Bind(_2 =>
        Hex                 .Bind(_3 => 
        Hex                 .Map (_4 => Convert.ToChar($"\\u{_1}{_2}{_3}{_4}"))))))
    );

    private static readonly IParser<byte, byte> Character = OneOf(new [] {
        Satisfy<char>(c => (int)c is >= 0x0020 and <= 0x10FFFF),
        Symbol('\"'),
        Symbol('\\'),
        Symbol('\\').Bind(_ => Escape)
    });

    //private static readonly IParser<char, decimal> Number =

    //private static readonly IParser<char, JsonNode> Node = OneOf(Bool, Number);

    //public static IEither<string, IParseResult<char, JsonNode>> Parse(string input) =>
    //Node.Parse(new ParserInput<char>(input.ToArray(), 0));
}