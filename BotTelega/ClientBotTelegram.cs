using System.Text;
using System.Text.Json;

namespace BotTelega;

public class ClientBotTelegram
{
    private static HttpClient _httpClient = new();
    private const string Token = "5807464884:AAFvH0gpGPZmGNmcUtp2fzL9-lfM0iV__5E"; // токен вашего бота
    private const string SendMessage = $"https://api.telegram.org/bot{Token}/sendMessage";
    private const string GetUpdates = $"https://api.telegram.org/bot{Token}/getUpdates?offset=";
    private long _offset = -1;

    private static ReplyKeyboardMarkup? _replyKeyboardMarkup;

    public static TelegramResponse? _telegramResponse;

    public ClientBotTelegram()
    {
        KeyboardButton[][] keyboardButtons = new[]
        {
            new KeyboardButton[] { 
                new KeyboardButton("как делить?"), new KeyboardButton("как соединять?")
            },
            new KeyboardButton[] {
                new KeyboardButton("как делить по кол-ву интерфейсов?")
            },
            new KeyboardButton[] {
                new KeyboardButton("основная информация о сети")
            },
            new KeyboardButton[]
            {
                new KeyboardButton("/help"),
            }
        };
        _replyKeyboardMarkup = new(keyboardButtons, true, true);
    }


   

    // функция для получения сообщения, отправленного пользователем
    public async Task GetAsync()
    {

        using HttpResponseMessage response = await _httpClient.GetAsync(GetUpdates + _offset.ToString());

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine(jsonResponse);

        _telegramResponse = JsonSerializer.Deserialize<TelegramResponse>(jsonResponse, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = new ToSnakeCaseNamingPolicy()
        });


        if (_telegramResponse != null && _telegramResponse.Result != null && _telegramResponse.Result.Length != 0)
        {
            _offset = _telegramResponse.Result[0].UpdateId + 1;

            long id = _telegramResponse.Result[0].Message.Chat.Id;
            string textMessage = _telegramResponse.Result[0].Message.Text;

            // TODO: спец символ для деления
            new Thread(async () => {
                ResponseMessage message = new();
                var answer = message.GetAnswer(textMessage).Split("||");
                for (int i = 0; i < answer.Length; i++)
                    await PostAsync(id, answer[i]);
            }).Start();   
        }
    }


    public async Task PostAsync(long chat_id, string text)
    {
        var message = new MessageRequest(chat_id, text, _replyKeyboardMarkup);
        var messageDataJson = JsonSerializer.Serialize
            (
                message,
                new JsonSerializerOptions() { PropertyNamingPolicy = new ToSnakeCaseNamingPolicy() }
            );

        var content = new StringContent(messageDataJson, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(SendMessage, content);
    }


}
