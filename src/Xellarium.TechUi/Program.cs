using Microsoft.Extensions.Configuration;
using Xellarium.WebApi.V1;

namespace Xellarium.TechUi;

static class Program
{
    public class MenuAction
    {
        public required Func<Task<HttpResponseMessage>> Action { get; set; }
        public required string Description { get; set; }
    }
    
    private static bool _quitFlag;
    private static readonly List<MenuAction> MenuActions = new();
    private static readonly HttpClient Client = new HttpClient();
    
    static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        var configuration = builder.Build();

        var serverPath = configuration["ServerAddress"];
        
        foreach (var (t, path) in ApiHelper.GetControllers(typeof(UserController).Assembly))
        {
            foreach (var (mi, method, callPath) in ApiHelper.GetApiCalls(t))
            {
                var call = ApiHelper.GenApiCall(mi, method, $"{serverPath}/{path}/{callPath}");
                MenuActions.Add(new MenuAction()
                {
                    Action = () => call(Client),
                    Description = $"{method.Method} {path}/{callPath}"
                });
            }
        }
        
        while (!_quitFlag)
        {
            PrintMenu();
            var index = RequestIndex();
            if (index is null)
            {
                Console.WriteLine("Ошибка ввода");
            }
            else if (index == 0)
            {
                _quitFlag = true;
            }
            else
            {
                int realIndex = index.Value - 1;
                var action = MenuActions[realIndex];
                try
                {
                    var response = await action.Action();
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Успешно, ответ сервера: ");
                        Console.WriteLine(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка запроса: {response.StatusCode}, {response.ReasonPhrase}");
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine($"Некорректный формат введенных данных.");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Сервер Xellarium по адресу {serverPath} отверг подключение.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка: {e}");
                }
            }

            if (!_quitFlag)
            {
                Console.Write("Нажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("Доступные пункты меню:");
        for (int i = 0; i < MenuActions.Count; i++)
        {
            var action = MenuActions[i];
            Console.WriteLine($"{i + 1}. {action.Description}");
        }
        Console.WriteLine("0. Выход");
    }

    private static int? RequestIndex()
    {
        Console.WriteLine("Выберите пункт меню: ");
        var str = Console.ReadLine();
        if (str is null || !int.TryParse(str, out var ind))
        {
            return null;
        }

        return ind;
    }
}