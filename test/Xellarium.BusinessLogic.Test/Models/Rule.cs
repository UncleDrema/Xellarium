using Xellarium.BusinessLogic.Models;
using Xellarium.Shared;

namespace Xellarium.BusinessLogic.Test.Models;

public class RuleTests
{
    private static Rule GameOfLifeRule => new Rule
    {
        GenericRule = GenericRule.GameOfLife,
        Name = "Game of Life",
        Owner = new User { Name = "John Doe" }
    };

    [Fact]
    private void Test()
    {
        try
        {
            var rule = new Rule
            {
                GenericRule = GenericRule.GameOfLife,
                Name = "Game of Life",
                Owner = new User { Name = "John Doe" }
            };
        }
        catch (Exception ex)
        {
            throw ex.InnerException ?? ex;
        }
    }
    
    [Theory]
    [MemberData(nameof(StaticCombinationsData))]
    public void Rule_GameOfLife_Static_Combinations(int [][] cells, int steps)
    {
        cells = Utils.ZeroPadding(cells, 1);
        var world = new World(cells);
        
        for (var i = 0; i < steps; i++)
        {
            world = GameOfLifeRule.NextState(world, Neighborhood.MooreOffsets);
        }
        
        Assert.Equal(cells, world.Cells);
    }
    
    [Theory]
    [MemberData(nameof(DyingCombinationsData))]
    public void Rule_GameOfLife_Dying_Combinations(int[][] cells, int steps)
    {
        cells = Utils.ZeroPadding(cells, 1);
        var world = new World(cells);
        
        for (var i = 0; i < steps; i++)
        {
            world = GameOfLifeRule.NextState(world, Neighborhood.MooreOffsets);
        }
        
        Assert_AllDead(world);
    }
    
    private static void Assert_AllDead(World world)
    {
        for (var i = 0; i < world.Width; i++)
        {
            for (var j = 0; j < world.Height; j++)
            {
                Assert.Equal(0, world[i, j]);
            }
        }
    }
    
    public static IEnumerable<object[]> DyingCombinationsData => 
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
    
    public static IEnumerable<object[]> StaticCombinationsData =>
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