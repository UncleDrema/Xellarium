using Reqnroll;

namespace Xellarium.EndToEnd;

[Binding]
public class Transform
{
    [StepArgumentTransformation]
    public string? NullableStringTransform(string str)
    {
        return str == "null" ? null : str;
    }
}