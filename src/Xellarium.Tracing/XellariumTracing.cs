using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xellarium.Tracing;

public static class XellariumTracing
{
    private static readonly Dictionary<string, ActivitySource> _sourceCache = new();

    internal static ActivitySource GetSourceCached(string name)
    {
        if (_sourceCache.TryGetValue(name, out var source))
        {
            return source;
        }
        
        source = new ActivitySource(name);
        _sourceCache.Add(name, source);
        return source;
    }
    
    private static ActivitySource GetSourceInner(string? filePath)
    {
        if (filePath == null)
        {
            return new ActivitySource("Xellarium");
        }
        
        if (_sourceCache.TryGetValue(filePath, out var source))
        {
            return source;
        }
        
        var className = Path.GetFileNameWithoutExtension(filePath);
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == className);
        var fullClassName = type?.FullName ?? className;
        source = new ActivitySource($"{fullClassName}");
        _sourceCache.Add(filePath, source);
        return source;
    }
    
    public static ActivitySource GetSource([CallerFilePath] string? filePath = null)
    {
        return GetSourceInner(filePath);
    }
    
    public static Activity? StartActivity([CallerFilePath] string? filePath = null, [CallerMemberName] string? methodName = null)
    {
        var source = GetSourceInner(filePath);
        
        if (methodName == null)
        {
            return source.StartActivity();
        }
        
        return source.StartActivity(methodName);
    }
}