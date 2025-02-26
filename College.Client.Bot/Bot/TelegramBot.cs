using Home.Client.Bot.Schedule;
using Microsoft.AspNetCore.Components;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Home.Client.Bot;

interface IBot {
    public Task Start();
    public Task Stop();
}

abstract class Bot : IBot{

    protected CancellationTokenSource cts_token { get; set; }

    public virtual async Task Start() {
        try {
            await OnStart();
        }
        catch (Exception ex) {
            await Console.Out.WriteLineAsync("Не удалось запустить бота");
            await Console.Out.WriteLineAsync(ex.Message);
        }
    }

    public virtual async Task Stop() {
        try {
            cts_token.Cancel();
            await OnStop();
        }
        catch {
            await Console.Out.WriteLineAsync("Бота уже не остановить...");
        }
    }

    protected abstract Task OnStart();
    protected abstract Task OnStop();
}

class Telegram_Bot : Bot {
    private TelegramBotClient bot;
    private new CancellationTokenSource cts_token;
    private IUserController data;

    // Конструктор класса
    public Telegram_Bot(string TOKEN) {
        cts_token = new CancellationTokenSource();
        data = new TelegramUserController();
        bot = new TelegramBotClient(TOKEN, null, cts_token.Token);
        bot.OnMessage += OnMessage;
        bot.OnUpdate += OnUpdate;
    }

    protected async Task OnMessage(Message msg, UpdateType type) {
        switch (msg.Text) {
            case "/start":
                await bot.SendMessage(msg.Chat, data.start_message(), ParseMode.Markdown);
                break;
            case "/schedule":
                await bot.SendMessage(msg.Chat, await data.actual_schedule("24-13"), ParseMode.Markdown);
                break;

                // TimeSpan timeout = TimeSpan.FromSeconds(5); // Устанавливаем тайм-аут

                // Task<string> scheduleTask = data.actual_schedule("24-13");
                // Task delayTask = Task.Delay(timeout);

                // if (await Task.WhenAny(scheduleTask, delayTask) == scheduleTask)
                // {
                //     // Если данные пришли вовремя
                //     try
                //     {
                //         string schedule = await scheduleTask; // Дожидаемся завершения
                //         await bot.SendMessage(msg.Chat, schedule, ParseMode.Markdown);
                //         Console.WriteLine("Данные отправлены");
                //     }
                //     catch (Telegram.Bot.Exceptions.ApiRequestException error)
                //     {
                //         Console.WriteLine($"ОШИБКА: {error.Message}");
                //     }
                // }
                // else
                // {
                //     // Если таймер истёк
                //     await bot.SendMessage(msg.Chat, "⏳ `IT COLLEGE OFFLINE` ⏳\n Запрос улетает в никуда...", ParseMode.Markdown);
                //     Console.WriteLine("ОШИБКА: Превышено время ожидания.");
                // }
                // break;
        }
    }

    protected async Task OnUpdate(Update update) {
        if (update is { CallbackQuery: { } query }) {
            await bot.SendMessage(query.Message!.Chat, $"{query.From} ну и всё тогда");
        }
    }

    protected override async Task OnStart() {
        var bot_info = await bot.GetMe();
        Console.WriteLine($"@{bot_info.Username} начал воркать, для завершения нажмите enter");
        Console.ReadLine();
    }

    protected override async Task OnStop() {
        cts_token.Cancel();
        await bot.Close();
    }
};