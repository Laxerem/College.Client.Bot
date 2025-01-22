using System.Text;

namespace Home.Client.Bot;

class UserController {
    static HttpClient httpClient = new HttpClient();
    
    public async Task Get_schedule() {

        string post_data = """
        {
            "d_start": "2025-01-19T19:00:00.000Z",
            "d_end": "2025-01-24T19:00:00.000Z",
            "group": "ИТ24-12"
        }
        """;

        var content = new StringContent(post_data, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://portal.it-college.ru/schedule.php");
        request.Content = content;

        using var response = await httpClient.SendAsync(request);
        string responseText = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseText);
    }
}