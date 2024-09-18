using Xellarium.BusinessLogic.Models;

namespace Xellarium.BusinessLogic.Test.Models;

public class UserTests
{
    // Проверим блокировку пользователя после трёх предупреждений
    [Fact]
    public void AddWarning_ThreeWarnings_BlockUser()
    {
        var user = new User();
        
        user.AddWarning();
        user.AddWarning();
        user.AddWarning();
        
        Assert.True(user.IsBlocked);
    }
    
    // Проверим, что после снятия предупреждения пользователь разблокируется
    [Fact]
    public void RemoveWarning_ThreeWarnings_UnblockUser()
    {
        var user = new User();
        
        user.AddWarning();
        user.AddWarning();
        user.AddWarning();
        
        user.RemoveWarning();
        
        Assert.False(user.IsBlocked);
    }
}