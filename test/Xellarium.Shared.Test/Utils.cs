using Allure.Xunit.Attributes;

namespace Xellarium.Shared.Test;

[AllureParentSuite("Shared Logic")]
[AllureSuite("Utils")]
[AllureSubSuite("Utils methods")]
public class UtilsTests
{
    [Theory(DisplayName = "GetCyclicIndex returns correct value")]
    [InlineData(0, 5, 0)]
    [InlineData(1, 5, 1)]
    [InlineData(5, 5, 0)]
    [InlineData(-1, 5, 4)]
    [InlineData(-5, 5, 0)]
    [InlineData(-6, 5, 4)]
    public void GetCyclicIndex_ShouldReturnCorrectValue(int x, int size, int expected)
    {
        // Arrange
        // Act
        var result = Utils.GetCyclicIndex(x, size);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    // test padding array
    public static IEnumerable<object[]> ZeroPaddingData =>
        new List<object[]>
        {
            new object[]
            {
                new[]
                {
                    new [] {1}
                },
                1,
                new[]
                {
                    new[] {0, 0, 0},
                    new[] {0, 1, 0},
                    new[] {0, 0, 0}
                }
            },
            new object[]
            {
                new[]
                {
                    new[] { 0, 0, 0, 0 },
                    new[] { 0, 1, 1, 0 },
                    new[] { 0, 1, 1, 0 },
                    new[] { 0, 0, 0, 0 }
                },
                1,
                new[]
                {
                    new[] { 0, 0, 0, 0, 0, 0 },
                    new[] { 0, 0, 0, 0, 0, 0 },
                    new[] { 0, 0, 1, 1, 0, 0 },
                    new[] { 0, 0, 1, 1, 0, 0 },
                    new[] { 0, 0, 0, 0, 0, 0 },
                    new[] { 0, 0, 0, 0, 0, 0 }
                }
            },
            new object[]
            {
                new[]
                {
                    new[] { 0, 1, 0 },
                    new[] { 1, 0, 1 },
                    new[] { 0, 1, 0 },
                },
                2,
                new[]
                {
                    new[] { 0, 0, 0, 0, 0, 0, 0 },
                    new[] { 0, 0, 0, 0, 0, 0, 0 },
                    new[] { 0, 0, 0, 1, 0, 0, 0 },
                    new[] { 0, 0, 1, 0, 1, 0, 0 },
                    new[] { 0, 0, 0, 1, 0, 0, 0 },
                    new[] { 0, 0, 0, 0, 0, 0, 0 },
                    new[] { 0, 0, 0, 0, 0, 0, 0 }
                }
            },
        };
    
    [Theory(DisplayName = "ZeroPadding returns correct zero-padded array")]
    [MemberData(nameof(ZeroPaddingData))]
    public void ZeroPadding_ShouldReturnCorrectValue(int[][] array, int padding, int[][] expected)
    {
        // Arrange
        // Act
        var result = Utils.ZeroPadding(array, padding);
        
        // Assert
        Assert.Equal(expected, result);
    }
}