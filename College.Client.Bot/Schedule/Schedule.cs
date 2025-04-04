using System.Text;
using System.Text.Json;
using College.Client.Bot;

namespace College.Client.Bot.Schedule;


public class Schedule {
    private MessageView message_controller = new MessageView();

    public async Task<string> actual_schedule(string group_number) {
        Time current_day = TimeController.Get_day();

        try {
            var dataController = new DataController(current_day.Get_day_start(), current_day.Get_day_end(), "24-13");

            var cts_token = new CancellationTokenSource();

            var ApiResponse = await dataController.SendRequestAsync(cts_token); 
            return message_controller.Day_data(ApiResponse);
        }
        catch(Exception ex) when (ex.Message == "RequestTimeOut") {
            Console.WriteLine(ex);
            return MessageView.RequestTimeOut();
        }
        catch(Exception ex) {
            Console.WriteLine(ex);
            return "О нет, это ошибка, почините меня";
        }
    }
}
