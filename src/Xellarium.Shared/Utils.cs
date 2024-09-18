namespace Xellarium.Shared;

public static class Utils
{
    public static int GetCyclicIndex(int x, int size)
    {
        x %= size;
        if (x < 0)
            x = size + x;
        return x;
    }
    
    public static int[][] ZeroPadding(int[][] array, int padding)
    {
        var width = array.Length;
        var height = array[0].Length;
        var newWidth = width + 2 * padding;
        var newHeight = height + 2 * padding;
        var newArray = new int[newWidth][];
        for (var i = 0; i < newWidth; i++)
        {
            newArray[i] = new int[newHeight];
        }
        
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                newArray[i + padding][j + padding] = array[i][j];
            }
        }
        
        return newArray;
    }
}