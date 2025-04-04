using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace College.Client.Bot;

interface IUserController {
    public string start_message();
    public Task<string> actual_schedule(string group_number);
}

class TelegramUserController : IUserController {
    static HttpClient httpClient = new HttpClient();
    CancellationTokenSource cts_token = new CancellationTokenSource();
    Schedule.Schedule schedule = new Schedule.Schedule(); 

    public string start_message() {
        return MessageView.start_message();
    }
    
    public async Task<string> actual_schedule(string group_number) {
        return await schedule.actual_schedule(group_number);
    }
};

