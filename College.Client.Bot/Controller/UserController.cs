using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Home.Client.Bot;

interface IUserController {
    public Task<string> actual_schedule(string group_number);
}

public class ISubGroup {
    public required string SClID {get; set;}
    public required string SGrID {get; set;}
    public required string SGCaID {get; set;}
    public required string STopic {get; set;}
    public required string STitle {get; set;}
}

public class IApiResponse {
    public required string ClID {get; set;}
    public required string Day {get; set;}
    public required string group {get; set;}
    public required string topic {get; set;}
    public required string start {get; set;}
    public required string end {get; set;}
    public required string room {get; set;}
    public required string color {get; set;}
    public required string title {get; set;}
    public List<ISubGroup> SubGroup {get; set;}
}

class PostData {
    public required string d_start {get; set;}
    public required string d_end {get; set;}
    public required string group {get; set;}
}

class TelegramUserController : IUserController {
    static HttpClient httpClient = new HttpClient();
    
    public async Task<string> actual_schedule(string group_number) {
        Time current_day = DataController.Get_day();

        PostData postData = new PostData {
            d_start = current_day.Start,
            d_end = current_day.End,
            group = $@"ИТ{group_number}"
        };

        string PostData_json = JsonSerializer.Serialize(postData);

        var content = new StringContent(PostData_json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://portal.it-college.ru/schedule.php");
        request.Content = content;

        Console.WriteLine("Отправляю запрос...");
        using var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode && response.Content != null) {
            Console.WriteLine("Ответ получен");
            Console.WriteLine("Читаю данные...");
            string res = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Данные прочтены");
            Console.WriteLine("Десериализация данных...");

            try {
                List<IApiResponse> response_data = JsonSerializer.Deserialize<List<IApiResponse>>(res);
                Console.WriteLine("Данные десериализованы");

                if (!response_data.Any()) {
                    return "Сегодня пар нет :)";
                }

                MessageView message = new MessageView();
                return message.Day_data(response_data);
            }
            catch (JsonException ex) {
                Console.WriteLine($"Ошибка десериализации: {ex.Message}");
                return "Ошибка десериализации данных";
            }
        }

        else {
            return "Не удаётся отправить запрос";
        }
    }
}