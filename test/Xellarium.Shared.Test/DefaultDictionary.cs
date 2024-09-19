using System.Diagnostics.CodeAnalysis;

namespace Xellarium.Shared.Test;

public class DefaultDictionaryTests
{
    // Выключено, т.к. аргумент используется для выведения generic типа
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    [Theory]
    [InlineData(default(int))]
    [InlineData(default(double))]
    [InlineData(default(string))]
    public void NoFactorySpecified_ReturnsDefault<T>(T typeValue)
    {
        var dict = new DefaultDictionary<int, T>();

        Assert.Equal(default, dict[0]);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(1.25)]
    [InlineData("some string")]
    public void DefaultValueSpecified_ReturnsIt<T>(T defaultValue)
    {
        var dict = new DefaultDictionary<int, T>(defaultValue);

        Assert.Equal(defaultValue, dict[0]);
    }

    [Theory]
    [MemberData(nameof(FactoryAndValues))]
    public void FactorySpecified_ReturnsValueFromIt<TK, TV>(Func<TK, TV> factory, IEnumerable<TK> values)
    {
        var dict = new DefaultDictionary<TK, TV>(factory);

        Assert.All(values, key => Assert.Equal(factory(key), dict[key]));
    }

    [Fact]
    public void InnerDictionaryAccess_ThrowsException()
    {
        var dict = new DefaultDictionary<int, int>();

        Assert.Throws<KeyNotFoundException>(() => dict.InnerDictionary[0]);
    }

    [Theory]
    [InlineData(5)]
    [InlineData("value")]
    [InlineData(true)]
    public void GetExistedValue_ReturnsIt<T>(T value)
    {
        var dict = new DefaultDictionary<int, T>();

        dict[0] = value;

        Assert.Equal(value, dict[0]);
    }

    public static IEnumerable<object[]> FactoryAndValues = new[]
    {
        new object[] { new Func<int, string>(i => i.ToString()), new[] { 1, 2, 3, -15, 0 } },
        new object[] { new Func<string, int>(b => b.GetHashCode()), new[] { "Hey", "Look", "At", "Me" } }
    };
}