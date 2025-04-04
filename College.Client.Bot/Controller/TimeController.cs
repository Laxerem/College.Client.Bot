using Microsoft.AspNetCore.SignalR;

namespace College.Client.Bot;

public class Time {
    private string Start {get; set;}
    private string End {get; set;}

    public Time(string start, string end) {
        this.Start = start;
        this.End = end;
    }

    public string Get_day_start() {
        return this.Start;
    }

    public string Get_day_end() {
        return this.End;
    }
} 

public class TimeController {
    public static string Get_time() {
        var current_datetime = DateTime.Now;
        string result = current_datetime.ToString("dd.MM");

        return result;
    }

    public static Time Get_week() {
        var current_datetime = DateTime.Now;
        int days_of_week = (int)current_datetime.DayOfWeek;
        
        DateTime week_start = current_datetime.AddDays(-days_of_week);
        DateTime week_end = week_start.AddDays(+7);

        string start = week_start.ToString("yyyy-MM-ddT19:00:00.000Z");
        string end = week_end.ToString("yyyy-MM-ddT19:00:00.000Z");

        var week = new Time(start, end);
        return week;
    }

    public static Time Get_day() {
        var current_datetime = DateTime.Now;
        
        string day_start = current_datetime.AddDays(-1).ToString("yyyy-MM-ddT19:00:00.000Z");
        string day_end = current_datetime.ToString("yyyy-MM-ddT19:00:00.000Z");

        var day = new Time(day_start, day_end);
        return day;
    }

    public Time Get_time_interval(DateOnly date_start, DateOnly date_end) {
        string start = date_start.ToString("yyyy-MM-dd");
        string end = date_end.ToString("yyyy-MM-dd");

        Time time_interval = new Time(start, end);

        return new Time(start, end);
    }
}