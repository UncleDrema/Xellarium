using System.Reflection;
using System.Text;
using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NuGet.Protocol;
using Xellarium.Shared;
using Xellarium.Shared.DTO;

namespace Xellarium.TechUi;

public static class ApiHelper
{
    static ApiHelper()
    {
        RegisterInputFunction(InputInt);
        RegisterInputFunction(InputString);
        RegisterInputFunction(InputUserLoginDto);
        RegisterInputFunction(InputUserDto);
        RegisterInputFunction(InputInts);
        RegisterInputFunction(InputBool);
        RegisterInputFunction(InputUserRole);
        RegisterInputFunction(InputCollectionDto);
        RegisterInputFunction(InputRuleDto);
        RegisterInputFunction(InputPostUserDto);
        RegisterInputFunction(InputPostCollectionDto);
        RegisterInputFunction(InputPostRuleDto);
        RegisterInputFunction(InputGenericRule);
    }
    
    private static void RegisterInputFunction<T>(Func<string, T> inputFunction)
    {
        _typeInputFunctions[typeof(T)] = name => inputFunction(name)!;
    }
    
    public static IEnumerable<(Type, string)> GetControllers(Assembly assembly)
    {
        foreach(Type type in assembly.GetTypes())
        {
            var routes = type.GetCustomAttributes<RouteAttribute>(true).ToList();
            var apiVersion = type.GetCustomAttribute<ApiVersionAttribute>();
            if (routes.Count > 0)
            {
                var route = routes[0];
                var template = route!.Template;
                var noControllerName = type.Name.Replace("Controller", "");
                var realisedTemplate = template.Replace("[controller]", noControllerName);
                realisedTemplate = realisedTemplate.Replace("{version:apiVersion}", $"{apiVersion.Versions[0].MajorVersion}");
                yield return (type, realisedTemplate);
            }
        }
    }

    public static IEnumerable<(MethodInfo, HttpMethod, string)> GetApiCalls(Type controller)
    {
        foreach (MethodInfo mi in controller.GetMethods())
        {
            var methodAttributes = mi.GetCustomAttributes<HttpMethodAttribute>().ToList();
            if (methodAttributes.Count > 0)
            {
                var method = methodAttributes[0];
                var template = method.Template ?? "";
                var httpMethod = method switch
                {
                    HttpGetAttribute => HttpMethod.Get,
                    HttpPostAttribute => HttpMethod.Post,
                    HttpPutAttribute => HttpMethod.Put,
                    HttpDeleteAttribute => HttpMethod.Delete,
                    _ => null
                };
                yield return (mi, httpMethod!, template);
            }
        }
    }

    private static Dictionary<Type, Func<string, object>> _typeInputFunctions = new();

    private static GenericRule InputGenericRule(string name)
    {
        return GenericRule.GameOfLife;
    }
    
    private static int InputInt(string name)
    {
        Console.WriteLine($"Введите числовое значение {name}:");
        return int.Parse(Console.ReadLine()!);
    }

    private static List<int> InputInts(string name)
    {
        Console.WriteLine($"Введите список чисел {name} по одному, для завершения - Enter");
        var res = new List<int>();
        string? s = Console.ReadLine();
        while (s != "")
        {
            res.Add(int.Parse(s!));
            s = Console.ReadLine();
        }
        
        return res;
    }

    private static string InputString(string name)
    {
        Console.WriteLine($"Введите строковое значение {name}:");
        return Console.ReadLine()!;
    }
    
    private static T InputType<T>(string name)
    {
        return (T) InputType(name, typeof(T));
    }

    private static UserLoginDTO InputUserLoginDto(string name)
    {
        return new UserLoginDTO()
        {
            Username = InputType<string>("username"),
            Password = InputType<string>("password")
        };
    }

    private static UserDTO InputUserDto(string name)
    {
        return new UserDTO()
        {
            Id = InputType<int>("Id"),
            Name = InputType<string>("Name"),
            Role = InputType<UserRole>("UserRole"),
            WarningsCount = InputType<int>("WarningsCount"),
            IsBlocked = InputType<bool>("IsBlocked"),
            Rules = InputType<List<int>>("RuleIds").Select(i => new RuleDTO(){Id = i}).ToList(),
            Collections = InputType<List<int>>("CollectionIds").Select(i => new CollectionDTO(){Id = i}).ToList()
        };
    }
    
    private static bool InputBool(string name)
    {
        Console.WriteLine($"Введите булево значение {name} (true/false):");
        return bool.Parse(Console.ReadLine()!);
    }
    
    private static UserRole InputUserRole(string name)
    {
        Console.WriteLine($"Введите роль {name} (Admin/User):");
        return Enum.Parse<UserRole>(Console.ReadLine()!);
    }
    
    private static CollectionDTO InputCollectionDto(string name)
    {
        return new CollectionDTO()
        {
            Id = InputType<int>("Id"),
            Name = InputType<string>("Name"),
            IsPrivate = InputType<bool>("IsPrivate"),
            OwnerId = InputType<int>("OwnerId"),
            RuleReferences = InputType<List<int>>("RuleIds").Select(i => new RuleReferenceDTO(){Id = i}).ToList()
        };
    }
    
    private static RuleDTO InputRuleDto(string name)
    {
        return new RuleDTO()
        {
            Id = InputType<int>("Id"),
            Name = InputType<string>("Name"),
            GenericRule = InputType<GenericRule>("GenericRule"),
            OwnerId = InputType<int>("OwnerId"),
            CollectionReferences = InputType<List<int>>("CollectionIds").Select(i => new CollectionReferenceDTO(){Id = i}).ToList()
        };
    }
    
    private static PostUserDTO InputPostUserDto(string name)
    {
        return new PostUserDTO()
        {
            Name = InputType<string>("Name"),
            Password = InputType<string>("Password"),
            Role = InputType<UserRole>("Role")
        };
    }
    
    private static PostCollectionDTO InputPostCollectionDto(string name)
    {
        return new PostCollectionDTO()
        {
            Name = InputType<string>("Name"),
            IsPrivate = InputType<bool>("IsPrivate"),
        };
    }
    
    private static PostRuleDTO InputPostRuleDto(string name)
    {
        return new PostRuleDTO()
        {
            Name = InputType<string>("Name"),
            GenericRule = InputType<GenericRule>("GenericRule"),
            NeighborhoodId = InputType<int>("NeighborhoodId")
        };
    }

    private static object InputType(string name, Type type)
    {
        return _typeInputFunctions[type](name);
    }

    public static Func<HttpClient, Task<HttpResponseMessage>> GenApiCall(MethodInfo mi, HttpMethod httpMethod, string path)
    {
        return async (client) =>
        {
            var realPath = path;
            var pars = mi.GetParameters();
            Type? BodyParameter = null;
            Dictionary<string, Type> PathParameters = new Dictionary<string, Type>();
            foreach (var param in pars)
            {
                var fromBody = param.GetCustomAttribute<FromBodyAttribute>();
                if (fromBody != null)
                {
                    if (BodyParameter is not null)
                        throw new ArgumentException("There can be only one BodyParameter");
                    BodyParameter = param.ParameterType;
                }
                else
                {
                    PathParameters[param.Name!] = param.ParameterType;
                }
            }

            Dictionary<string, object> PathValues = new Dictionary<string, object>();
            foreach (var (name, type) in PathParameters)
            {
                var value = InputType(name, type);
                PathValues[name] = value;
            }
            
            HttpContent? content = null;
            if (BodyParameter is not null)
            {
                var body = InputType("Тело запроса", BodyParameter);
                content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            }

            foreach (var (name, value) in PathValues)
            {
                realPath = realPath.Replace($"{{{name}}}", value.ToString());
            }
            
            var req = new HttpRequestMessage(httpMethod, realPath)
            {
                Content = content,
            };

            var json = "";
            if (content is not null)
            {
                json = await content.ReadAsStringAsync();
            }
            Console.WriteLine($"Отправляем запрос: {json} по пути {realPath}");
            
            return await client.SendAsync(req);
        };
    }
}