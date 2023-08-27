using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotTelega
{
    public class TelegramResponse
    {
        public bool Ok { get; set; }
        public TelegramUpdates[]? Result { get; set; }

    }

    public class TelegramUser
    {
        public long Id { get; set; }

        public bool IsBot { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Username { get; set; }
    }

    public class TelegramUpdates
    {
        public long UpdateId { get; set; }

        public TelegramMessage? Message { get; set; }
    }

    public class TelegramMessage
    {
        public long MessageId { get; set; }

        public TelegramUser? From { get; set; }

        public TelegramChat? Chat { get; set; }

        public string Text { get; set; }

        public TelegramMessage()
        {
            Text = "";
        }
    }


    public class TelegramChat
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Type { get; set; }
    }


    class MessageRequest
    {
        public long ChatId { get; }

        public string Text { get; }

        public ReplyKeyboardMarkup ReplyMarkup { get; set; }

        public string ParseMode { get; set; }

        public MessageRequest(long chatId, string text, ReplyKeyboardMarkup replyMarkup)
        {
            ChatId = chatId;
            Text = text;
            ReplyMarkup = replyMarkup;
            ParseMode = "HTML";
        }

        
    }

    class ReplyKeyboardMarkup
    {
        public KeyboardButton[][] Keyboard { get; set; }

        public bool ResizeKeyboard { get; set; }

        public bool OneTimeKeyboard { get; set; }
        
        public ReplyKeyboardMarkup(KeyboardButton[][] keyboard, bool resizeKeyboard, bool oneTimeKeyboard)
        {
            Keyboard = keyboard;
            ResizeKeyboard = resizeKeyboard;
            OneTimeKeyboard = oneTimeKeyboard;
            
        }

        public ReplyKeyboardMarkup()
        {

        }

    }

    public class KeyboardButton
    {
        public string Text { get; set; }

        public KeyboardButton(string Text)
        {
            this.Text = Text;
        }

    }
}
