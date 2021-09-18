using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using Telegram.Bot.Types.InlineQueryResults;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace ZanzerdBot
{
    public class Handlers 
    {
        const double RWeightGS = 0.2989;
        const double GWeightGS = 0.5870;
        const double BWeightGS = 0.1140;

        const string lighteningName = "Осветление/затемнение";
        const string grayscaleName = "Оттенки серого";
        const string sepiaName = "Сепия";
        const string negativeName = "Негатив";
        const string cancelName = "Отмена";
        const string redoName = "Вернуть";

        static int cancelCount = 0;

        static FiltersEnum filterState = FiltersEnum.None;
        static States statesOfImage;

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                => $"Ошибка апишки! \n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        async public static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Type == UpdateType.Message)
                {
                    await BotOnMessageReceived(botClient, update.Message);
                    var chatId = update.Message.Chat.Id;
                    Console.WriteLine($"Получил '{update.Message.Text}' в чате {chatId}.");
                }
                else if (update.Type == UpdateType.CallbackQuery)
                    await BotOnCallbackQueryReceived(botClient, update.CallbackQuery);
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        async private static Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Тип полученного сообщения: {message.Type}");
            if (message.Type != MessageType.Text && message.Type != MessageType.Photo)
                return;

            Task<Message> action = null;
            switch (message.Type)
            {
                case MessageType.Text:
                    MemoryStream ms;
                    switch (message.Text)
                    {
                        case grayscaleName:
                            filterState = FiltersEnum.Grayscale;
                            break;
                        case sepiaName:
                            filterState = FiltersEnum.Sepia;
                            break;
                        case lighteningName:
                            filterState = FiltersEnum.LighteningStageOne;
                            break;
                        case negativeName:
                            filterState = FiltersEnum.Negative;
                            break;
                        default:
                            break;
                    }
                    string fail = "Ты ввёл что-то не то.";
                    if (filterState != FiltersEnum.None)
                    {
                        var original = statesOfImage.Peek();
                        Image result;
                        PixelFilter<EmptyParameters> filter = null;
                        switch (filterState)
                        {
                            case FiltersEnum.LighteningStageOne:
                                string lighteningInfo = "Выбери коэффициент осветления (не меньше 0), где 1 - яркость оригинальной картинки.";
                                filterState = FiltersEnum.LighteningStageTwo;
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: lighteningInfo);
                                break;
                            case FiltersEnum.LighteningStageTwo:
                                double coeff;
                                if (!double.TryParse(message.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out _) && 
                                        !double.TryParse(message.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                                {
                                    filterState = FiltersEnum.LighteningStageOne;
                                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: fail);
                                }
                                else
                                {
                                    coeff = double.Parse(message.Text, NumberStyles.Any, CultureInfo.InvariantCulture); //вроде решил проблему?
                                    var lightParams = new LighteningParameters();
                                    lightParams.Parse(new[] { coeff });
                                    var specialFilter = new PixelFilter<LighteningParameters>(lighteningName,
                                        (pixel, parameters) => Pixel.Trim(pixel * parameters.Coefficient));
                                    result = specialFilter.Process(Convertors.Bitmap2Photo(original), lightParams);
                                    await ProcessImage(botClient, message, result);
                                }
                                break;

                            case FiltersEnum.Grayscale:
                                filter = new PixelFilter<EmptyParameters>(grayscaleName,
                                    (pixel, parameters) =>
                                    {
                                        var resultPixel = new Pixel();
                                        resultPixel.R = pixel.R * RWeightGS + pixel.G * GWeightGS + pixel.B * BWeightGS;
                                        resultPixel.G = pixel.R * RWeightGS + pixel.G * GWeightGS + pixel.B * BWeightGS;
                                        resultPixel.B = pixel.R * RWeightGS + pixel.G * GWeightGS + pixel.B * BWeightGS;
                                        return resultPixel;
                                    });
                                result = filter.Process(Convertors.Bitmap2Photo(original), new EmptyParameters());
                                await ProcessImage(botClient, message, result);
                                break;

                            case FiltersEnum.Sepia:
                                filter = new PixelFilter<EmptyParameters>(sepiaName,
                                    (pixel, parameters) =>
                                    {
                                        var resultPixel = new Pixel();
                                        resultPixel.R = (pixel.R * 0.393) + (pixel.G * 0.769) + (pixel.B * 0.189);
                                        resultPixel.G = (pixel.R * 0.349) + (pixel.G * 0.686) + (pixel.B * 0.168);
                                        resultPixel.B = (pixel.R * 0.272) + (pixel.G * 0.534) + (pixel.B * 0.131);
                                        return Pixel.Trim(resultPixel);
                                    });
                                result = filter.Process(Convertors.Bitmap2Photo(original), new EmptyParameters());
                                await ProcessImage(botClient, message, result);
                                break;

                            case FiltersEnum.Negative:
                                filter = new PixelFilter<EmptyParameters>(negativeName,
                                    (pixel, parameters) =>
                                    {
                                        var resultPixel = new Pixel();
                                        resultPixel.R = 1 - pixel.R;
                                        resultPixel.B = 1 - pixel.B;
                                        resultPixel.G = 1 - pixel.G;
                                        return Pixel.Trim(resultPixel);
                                    });
                                result = filter.Process(Convertors.Bitmap2Photo(original), new EmptyParameters());
                                await ProcessImage(botClient, message, result);
                                break;

                            default:
                                Console.WriteLine("Выбран непонятный вариант?");
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Этот вариант пока не работает");
                                await ShowKeyboard(botClient, message);
                                await ShowCallbackQueryTable(botClient, message);
                                break;

                        }
                    }
                    else
                    {
                        action = (message.Text.Split(' ').First()) switch
                        {
                            "/фильтр" => RequestForImage(botClient, message),
                            _ => Usage(botClient, message)
                        };
                    }
                    break;
                case MessageType.Photo:
                    action = BotOnImageReceived(botClient, message);
                    break;
                default:
                    break;
            }
            var sentMessage = await action;
            Console.WriteLine($"ID полученного сообщения: {sentMessage.MessageId}");
        }

        static async Task ProcessImage(ITelegramBotClient botClient, Message message, Image result)
        {
            var resultAsBmp = Convertors.Photo2Bitmap(result);
            var ms = new MemoryStream();
            resultAsBmp.Save(ms, ImageFormat.Jpeg);
            statesOfImage.Add(resultAsBmp);
            ms.Position = 0;
            filterState = FiltersEnum.None;
            await botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: ms, caption: "Обработал!");
            await ShowKeyboard(botClient, message);
            await ShowCallbackQueryTable(botClient, message);
        }
        static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            const string usage = "Я - бот-фотошоп. Загрузи фотографию для обработки.";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: usage, replyMarkup: new ReplyKeyboardRemove());
        }

        static async Task<Message> RequestForImage(ITelegramBotClient botClient, Message message)
        {
            const string request = "Загрузи фотографию.";
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: request);
        }

        static async Task<Message> BotOnImageReceived(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            var imageFile = await botClient.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
            //взял последнее представление так как оно сжато слабее остальных

            var stream = new MemoryStream();
            await botClient.DownloadFileAsync(imageFile.FilePath, stream);
            stream.Seek(0, SeekOrigin.Begin);
            Bitmap initialPicture = new Bitmap(stream);
            statesOfImage = new States(initialPicture);
            return await ShowKeyboard(botClient, message);
        }

        static async Task<Message> ShowKeyboard(ITelegramBotClient botClient, Message message)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { grayscaleName, sepiaName },
                        new KeyboardButton[] { lighteningName, negativeName },
                    })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Выбери фильтр", replyMarkup: replyKeyboardMarkup);
        }

        private static async Task<Message> ShowCallbackQueryTable(ITelegramBotClient botClient, Message message)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    // первый ряд
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(cancelName, cancelName),
                        InlineKeyboardButton.WithCallbackData(redoName, redoName)
                    },
            });
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Отменить/вернуть:", replyMarkup: inlineKeyboard); //??
        }

        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            Console.WriteLine(callbackQuery.Data);
            MemoryStream ms;
            switch (callbackQuery.Data)
            {
                case cancelName:
                    filterState = FiltersEnum.None;
                    if (statesOfImage.CanCancel())
                    {
                        await DoCancel(botClient, callbackQuery);
                    }
                    else
                        await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Нечего отменять");
                    await ShowCallbackQueryTable(botClient, callbackQuery.Message);
                    break;
                case redoName:
                    filterState = FiltersEnum.None;
                    if (cancelCount > 0)
                    {
                        await DoRedo(botClient, callbackQuery);
                    }
                    else
                        await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Нечего возвращать");
                    await ShowCallbackQueryTable(botClient, callbackQuery.Message);
                    break;
                default:
                    Console.WriteLine("Выбран непонятный вариант?");
                    await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"{callbackQuery.Data}");
                    await botClient.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Этот вариант пока не работает");
                    await ShowCallbackQueryTable(botClient, callbackQuery.Message);
                    break;
            }
        }
        static async Task DoCancel(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            statesOfImage.Cancel();
            cancelCount++;
            var ms = new MemoryStream();
            statesOfImage.Peek().Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            await botClient.SendPhotoAsync(chatId: callbackQuery.Message.Chat.Id, photo: ms, caption: "Отменил");
        }

        static async Task DoRedo(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            statesOfImage.Redo();
            cancelCount--;
            var ms = new MemoryStream();
            statesOfImage.Peek().Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            await botClient.SendPhotoAsync(chatId: callbackQuery.Message.Chat.Id, photo: ms, caption: "Вернул");
        }
    }
}
