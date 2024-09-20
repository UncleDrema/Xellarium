using System.Diagnostics.CodeAnalysis;
using Allure.Xunit.Attributes;

namespace Xellarium.Shared.Test;

[AllureParentSuite("Shared Logic")]
[AllureSuite("Utils")]
[AllureSubSuite("DefaultDictionary")]
public class DefaultDictionaryTests
{
    // Выключено, т.к. аргумент используется для выведения generic типа
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters")]
    [Theory(DisplayName = "No factory specified makes dictionary return default value")]
    [InlineData(default(int))]
    [InlineData(default(double))]
    [InlineData(default(string))]
    public void NoFactorySpecified_ReturnsDefault<T>(T typeValue)
    {
        // Arrange
        var dict = ObjectMother.CreateDefaultDictionary<int, T>();

        // Act
        var element = dict[0];
        
        // Assert
        Assert.Equal(default, element);
    }

    [Theory(DisplayName = "Default value specified makes dictionary return it")]
    [InlineData(3)]
    [InlineData(1.25)]
    [InlineData("some string")]
    public void DefaultValueSpecified_ReturnsIt<T>(T defaultValue)
    {
        // Arrange
        var dict = ObjectMother.CreateDefaultDictionary<int, T>(defaultValue);

        // Act
        var element = dict[0];
        
        // Assert
        Assert.Equal(defaultValue, element);
    }

    [Theory(DisplayName = "Factory specified makes dictionary return value from it")]
    [MemberData(nameof(FactoryAndValues))]
    public void FactorySpecified_ReturnsValueFromIt<TK, TV>(Func<TK, TV> factory, IEnumerable<TK> values)
    {
        // Arrange
        var dict = ObjectMother.CreateDefaultDictionary(factory);

        // Act
        Action<TK> action = (key) => Assert.Equal(factory(key), dict[key]);
        
        // Assert
        Assert.All(values, action);
    }

    [Fact(DisplayName = "Inner dictionary access throws exception")]
    public void InnerDictionaryAccess_ThrowsException()
    {
        // Arrange
        var dict = ObjectMother.CreateDefaultDictionary<int, int>();

        // Act
        Func<object?> action = () => dict.InnerDictionary[0];
        
        // Assert
        Assert.Throws<KeyNotFoundException>(action);
    }

    [Theory(DisplayName = "Get existing value returns it")]
    [InlineData(5)]
    [InlineData("value")]
    [InlineData(true)]
    public void GetExistedValue_ReturnsIt<T>(T value)
    {
        // Arrange
        var dict = ObjectMother.CreateDefaultDictionary<int, T>();

        // Act
        dict[0] = value;

        // Assert
        Assert.Equal(value, dict[0]);
    }

    public static IEnumerable<object[]> FactoryAndValues = new[]
    {
        new object[] { new Func<int, string>(i => i.ToString()), new[] { 1, 2, 3, -15, 0 } },
        new object[] { new Func<string, int>(b => b.GetHashCode()), new[] { "Hey", "Look", "At", "Me" } }
    };
}