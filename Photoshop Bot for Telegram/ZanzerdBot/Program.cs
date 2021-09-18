using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace ZanzerdBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramBotClient botClient = new TelegramBotClient("1904232218:AAG5-qMbFrcDj-iVHhIi5Qdc2dCMMhAK0J4");
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(me.Username);
            Console.WriteLine($"Привет. Я - бот {me.FirstName}, а мой айдишник - {me.Id}");

            var cts = new CancellationTokenSource();
            botClient.StartReceiving(new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync), cts.Token);

            Console.WriteLine($"Начинаю слушать @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

    }
}
