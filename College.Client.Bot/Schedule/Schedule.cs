using System.Text;
using System.Text.Json;

namespace Home.Client.Bot.Schedule;

interface ISchedule_week {

}

public class Schedule {
    private static HttpClient httpClient = new HttpClient();

    private async Task<List<IApiResponse>> Get_schedule_data (PostData postData) {
        CancellationTokenSource cts_token = new CancellationTokenSource();
        cts_token.CancelAfter(TimeSpan.FromSeconds(5));

        try {
            string PostData_json = JsonSerializer.Serialize(postData);
            var content = new StringContent(PostData_json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://portal.it-college.ru/schedule.php");
            request.Content = content;

            using var response = await httpClient.SendAsync(request, cts_token.Token);

            if (response.IsSuccessStatusCode && response.Content != null) {
                string res = await response.Content.ReadAsStringAsync();

                try {
                    List<IApiResponse> ?response_data = JsonSerializer.Deserialize<List<IApiResponse>>(res);

                    if (response_data.Count != 0) {
                        return response_data;
                    }
                    else {
                        throw new Exception("DataEmpty");
                    }
                }

                catch (JsonException ex) {
                    throw new Exception("DeserializeFailed", ex);
                }
            }

            else {
                throw new Exception("RequestFailed");
            }
        }
        catch(OperationCanceledException ex) {
            throw new Exception("RequestTimeOut", ex);
        }
    }

    public async Task<string> actual_schedule(string group_number) {
        Time current_day = TimeController.Get_day();
        var message = new MessageView();

        PostData postData = new PostData {
            d_start = current_day.Start,
            d_end = current_day.End,
            group = $@"ИТ{group_number}"
        };

        try {
            List<IApiResponse> ApiResponse = await Get_schedule_data(postData);
            return message.Day_data(ApiResponse);
        }
        catch(Exception ex) when (ex.Message == "RequestTimeOut") {
            return MessageView.RequestTimeOut();
        }
    }
}
