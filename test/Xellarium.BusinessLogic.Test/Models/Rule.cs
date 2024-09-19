using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test.Models;

public class RuleTests(GameOfLife gameOfLife) : IClassFixture<GameOfLife>
{
    [Theory]
    [MemberData(nameof(GameOfLife.StaticCombinationsData), MemberType = typeof(GameOfLife))]
    public void Rule_GameOfLife_Static_Combinations(int [][] cells, int steps)
    {
        // Arrange
        cells = Utils.ZeroPadding(cells, 1);
        var world = ObjectMother.WorldOfCells(cells);
        
        // Act
        world = gameOfLife.Rule.NextState(world, Neighborhood.MooreOffsets, steps);
        
        // Assert
        Assert.Equal(cells, world.Cells);
    }
    
    [Theory]
    [MemberData(nameof(GameOfLife.DyingCombinationsData), MemberType = typeof(GameOfLife))]
    public void Rule_GameOfLife_Dying_Combinations(int[][] cells, int steps)
    {
        // Arrange
        cells = Utils.ZeroPadding(cells, 1);
        var world = ObjectMother.WorldOfCells(cells);
        
        // Act
        world = gameOfLife.Rule.NextState(world, Neighborhood.MooreOffsets, steps);
        
        // Assert
        for (var i = 0; i < world.Width; i++)
        {
            for (var j = 0; j < world.Height; j++)
            {
                Assert.Equal(0, world[i, j]);
            }
        }
    }

    [Fact]
    public void Rule_ThrowsIfOffsets_AreNull()
    {
        // Arrange
        var rule = gameOfLife.Rule;
        var world = ObjectMother.SimpleWorld();
        
        // Act
        Action action = () => rule.NextState(world, null);
        
        // Assert
        Assert.Throws<ArgumentNullException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Rule_ThrowsIfTimes_IsLessThanOne(int times)
    {
        // Arrange
        var rule = gameOfLife.Rule;
        var world = ObjectMother.SimpleWorld();
        
        // Act
        Action action = () => rule.NextState(world, Neighborhood.MooreOffsets, times);
        
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}

public class GameOfLife
{
    public Rule Rule { get; } = new Rule
    {
        GenericRule = GenericRule.GameOfLife,
        Name = "Game of Life",
        Owner = new User { Name = "John Doe" }
    };
    
    public static IEnumerable<object[]> DyingCombinationsData { get; } =
        new List<object[]>()
        {
            new object[]
            {
                new[]
                {
                    new[] {1}
                },
                10
            },
            new object[]
            {
                new[]
                {
                    new[] {0, 1, 0},
                    new[] {1, 0, 1}
                },
                10
            }
        };
    
    public static IEnumerable<object[]> StaticCombinationsData { get; } =
        new List<object[]>
        {
            new object[]
            {
                new[]
                {
                    new[] {1, 1},
                    new[] {1, 1},
                }, 
                10
            }, // Блок
            new object[]
            {
                new[]
                {
                    new[] {0, 1, 0},
                    new[] {1, 0, 1},
                    new[] {0, 1, 0},
                }, 
                10
            }, // Хлеб
            new object[]
            {
                new[]
                {
                    new[] {0, 1, 1, 0},
                    new[] {1, 0, 0, 1},
                    new[] {0, 1, 1, 0}
                }, 
                10
            } // Блин
        };
}