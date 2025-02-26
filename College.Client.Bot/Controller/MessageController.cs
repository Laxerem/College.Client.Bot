using Home.Client.Bot;
using Microsoft.AspNetCore.Http.Features;

class MessageData {
    private string message = "";

    public void add_string(string data) {
        if (data != null)
            this.message += $"{data}\n";
    }

    public void enter() {
        this.message += '\n';
    }

    public string Get_data() {
        return this.message;
    }
}

public class MessageView {
    public static string start_message() {

        return @"Привет, я *Diskay* - бот, который разрабатывается для студентов колледжа *цифровых технологий*. 👤

Что я пока что могу? Присылать тебе текущее расписание пар.

На данный момент, это может показаться бесполезным, но будущие обновления, возможно, сделают меня хорошим, или может даже для кого-то незаменимым помошником ⚡.

Из идей:
*1.* Поиск свободного (незанятого) кабинета.
*2.* Сохранение данных о пользоветеле, (например, группа `24-13`, подгруппа 1), для получения актуальных данных.
*3.* Отправка уведомлений о парах, настройка персональных уведомлений. Например: отправить сегодняшнее расписание в `8:30`.

А пока, я нахожусь на самой начальной стадии разработки, и продвигаюсь по мере возможности и желания.

Какие команды есть?

[/schedule](https://t.me/Diskay_bot?start=/schedule) - посмотреть текущее расписание";
    }

    public static string RequestTimeOut() {
        return "⏳ `IT COLLEGE OFFLINE` ⏳\n Запрос улетает в никуда...";
    }

    public string Day_data(List<IApiResponse> data) {
        Console.WriteLine("Структурирую данные");
        MessageData message = new MessageData();
    
        message.add_string($"*{data[0].group}*");
        message.add_string($"Расписание на сегодня: *{TimeController.Get_time()}*\n");
        message.add_string($"*- - - >* *Начало пар:* `{data[0].start.Substring(11, 5)}` 😩");
        message.enter();

        string item_start = "";
        string item_end = "";

        foreach(IApiResponse item in data) {
            item_start = item.start.Substring(11, 5);
            item_end = item.end.Substring(11, 5);

            message.add_string($"! - - - - `{item_start}` : `{item_end}` - - - - !");

            if (item.SubGroup == null) {
                message.add_string($"Предмет: *{item.title}*");
                message.add_string($"Кабинет: *{item.room}*");
            }
            else {
                message.add_string($"Предмет: *{item.title}*");
                foreach (ISubGroup sub in item.SubGroup) {
                    message.add_string(sub.STopic);
                    message.add_string(sub.STitle);
                    message.enter();
                    message.add_string($"Подгруппа: *{sub.SGrID}*");
                    message.add_string($"Кабинет: *{sub.SGCaID}*");
                }
            }
            message.add_string("");
        }
        message.add_string($"*Конец пар:* `{item_end}` 😓 *< - - -*");
        Console.WriteLine("Данные готовы к отправке");

        return message.Get_data();
    }
}