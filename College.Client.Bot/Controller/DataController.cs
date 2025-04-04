namespace College.Client.Bot;

using System.Globalization;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.Registry;
using Polly.Retry;

//-------------Ответ от сервера--------------
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
//---------------------------------------------

//-----------------Тело Запроса----------------
class PostData {
    public required string d_start {get; set;}
    public required string d_end {get; set;}
    public required string group {get; set;}
};
//---------------------------------------------

class DataController {
    private TimeController time = new TimeController();
    private HttpClient httpClient = new HttpClient(new HttpClientHandler{ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true});
    private static readonly ResiliencePipeline _pipelineProvider = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(), // Повтор при любых исключениях
                Delay = TimeSpan.FromSeconds(2),        // Задержка 2 секунды между попытками
                MaxRetryAttempts = 2,                   // 2 повторные попытки (итого 3 запроса)
                BackoffType = DelayBackoffType.Constant // Постоянная задержка
            })
            .Build();


    private PostData postData;

    public DataController(string day_start, string day_end, string group_number) {
        DateTime date_start = DateTime.ParseExact(day_start, "yyyy-MM-ddT19:00:00.000Z", CultureInfo.InvariantCulture);
        DateTime date_end = DateTime.ParseExact(day_end, "yyyy-MM-ddT19:00:00.000Z", CultureInfo.InvariantCulture);

        Time time_interval = time.Get_time_interval(DateOnly.FromDateTime(date_start), DateOnly.FromDateTime(date_end));
        this.postData = new PostData {
            d_start = time_interval.Get_day_start(),
            d_end = time_interval.Get_day_end(),
            group = $@"ИТ{group_number}"
        };
    }
    public async Task<List<IApiResponse>> SendRequestAsync(CancellationTokenSource ctsToken) {
        ctsToken.CancelAfter(TimeSpan.FromSeconds(5));

        try {
            string postDataJson = JsonSerializer.Serialize(this.postData);
            var content = new StringContent(postDataJson, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://portal.students.it-college.ru/schedule.php");
            request.Content = content;

            var response = await _pipelineProvider.ExecuteAsync(async cancellationToken =>
            {
                HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode(); // Бросает исключение, если статус не успешен
                return await response.Content.ReadAsStringAsync();
            });

            List<IApiResponse> ?response_data = JsonSerializer.Deserialize<List<IApiResponse>>(response);
            return response_data;
        }
        catch (TaskCanceledException ex) {
            throw new Exception(ex.Message);
        }
        catch (HttpRequestException ex) when (ex.InnerException is AuthenticationException) {
            Console.WriteLine($"Ошибка SSL: {ex.Message}");
            throw;
        }
    }
}