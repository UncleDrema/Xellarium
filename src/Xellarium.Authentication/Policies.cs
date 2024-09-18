namespace Xellarium.Authentication;

public static class Policies
{
    public const string Admin = "Admin";
    public const string AdminOrOwner = "AdminOrOwner";
    public const string CanAccessCollection = "CanAccessCollection";
}