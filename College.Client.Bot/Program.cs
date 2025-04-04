using Telegram.Bot;
using DotNetEnv;
using College.Client.Bot;

Env.Load();

string? token = Environment.GetEnvironmentVariable("BOT_TOKEN");

if (token != null) {
    Telegram_Bot bot = new Telegram_Bot(token);
    await bot.Start();
}