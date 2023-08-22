using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotFilesManager.Command;

namespace TelegramBotFilesManager
{
    public class TelegramBot
    {
        private readonly TelegramBotClient _botClient;
        private readonly CancellationTokenSource cts;
        public TelegramBot(string botToken)
        {
            cts = new();
            _botClient = new TelegramBotClient(botToken);
        }

        public void Start()
        {
            _botClient.StartReceiving(Update, Error);
            Console.ReadLine();
        }


        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update != null && update.Message != null)
            {
                await Console.Out.WriteLineAsync($"[{DateTime.Now}] {update!.Message!.From!.FirstName} {update!.Message!.From!.LastName} {update!.Message!.From!.Username}: {update.Message.Text ?? "Фотография"}");

                CommandHandler handler = new CommandHandler(update);
                ICommand command =  await handler.GetCommandHandler(_botClient);
                command?.Execute(update);
            }
        }
        private Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
