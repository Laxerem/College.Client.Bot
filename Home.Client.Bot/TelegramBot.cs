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
        catch {
            await Console.Out.WriteLineAsync("Не удалось запустить бота");
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

    // Конструктор класса
    public Telegram_Bot(string TOKEN) {
        cts_token = new CancellationTokenSource();
        bot = new TelegramBotClient(TOKEN, null, cts_token.Token);
        bot.OnMessage += OnMessage;
        bot.OnUpdate += OnUpdate;
    }

    protected async Task OnMessage(Message msg, UpdateType type) {
        switch (msg.Text) {
            case "/start":
                await bot.SendMessage(msg.Chat, "Привет! На каком курсе ты учишься?",
                replyMarkup: new InlineKeyboardMarkup().AddButtons("1", "2", "3", "4"));
                break;
            case "/schedule":
                await bot.SendMessage(msg.Chat, "Ща погодь");
                var user = new UserController();
                await user.Get_schedule();
                break;
        }
    }

    protected async Task OnUpdate(Update update) {
        if (update is { CallbackQuery: { } query }) {
            await bot.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
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