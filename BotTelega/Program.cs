namespace BotTelega;

internal class Program
{
    static async Task Main(string[] args)
    {

        ClientBotTelegram bot = new ClientBotTelegram();
        Console.WriteLine("Сервер запущен...");
        await bot.GetAsync();
        while (true)
        {
            await bot.GetAsync();
        }

    }

}




