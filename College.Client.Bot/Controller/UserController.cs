using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Home.Client.Bot;

interface IUserController {
    public string start_message();
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
    CancellationTokenSource cts_token = new CancellationTokenSource();
    Schedule.Schedule schedule = new Schedule.Schedule(); 

    public string start_message() {
        return MessageView.start_message();
    }
    
    public async Task<string> actual_schedule(string group_number) {
        return await schedule.actual_schedule(group_number);
    }
};

