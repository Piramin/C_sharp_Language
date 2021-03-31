using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICQBot
{
    public class Bot
    {
        public string token;
        public string api_base_url;
        public int lastEventId;
        WebClient webclient;

        public List<List<Button>> Buttons;
        public int ButtonsNumber;

        public struct Button
        {
            public string text;
            public string callbackData;
            public string style;

            public Button(string Text, int Count, string style)
            {

                this.text = Text;
                this.callbackData = "id" + Convert.ToString(Count);
                this.style = style;
            }

            public string ButtonInfo()
            {
                string Info;
                Info = "text" + ": " + this.text + " callbackData" + ": " + this.callbackData;

                return Info;
            }
        }
        public Bot(string token, string api_base_url, int lastEventId)
        {
            this.token = token;
            this.api_base_url = api_base_url;
            this.lastEventId = lastEventId;
            this.webclient = new WebClient { Encoding = Encoding.UTF8 };

            this.Buttons = new List<List<Button>>(0);
            this.ButtonsNumber = 0;
        }
        public Bot(string token, string api_base_url)
        {
            this.token = token;
            this.api_base_url = api_base_url;
            this.lastEventId = 0;
            this.webclient = new WebClient { Encoding = Encoding.UTF8 };

            this.Buttons = new List<List<Button>>(0);
            this.ButtonsNumber = 0;
        }
        public Bot(string token)
        {
            this.token = token;
            this.api_base_url = $@"https://api.icq.net/bot/v1/";
            this.lastEventId = 0;
            this.webclient = new WebClient { Encoding = Encoding.UTF8 };

            this.Buttons = new List<List<Button>>(0);
            this.ButtonsNumber = 0;
        }

        //Собственные методы: (+)
        /// <summary>
        /// Метод ,возвращающий информацию о боте.
        /// </summary>
        /// <returns></returns>
        public string GetBotInfo()
        {
            string operation = "/self/get?";

            string Info = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}");
            return Info;
        }

        //Методы событий: (+)

        /// <summary>
        /// Метод, возвращающий данные,полученные ботом.
        /// </summary>
        /// <param name="lastEventId">Номер последнего известного события.</param>
        /// <param name="pollTime">Время удержания соединения с сервером.</param>
        /// <returns></returns>
        public string GetBotDataReport(int lastEventId, int pollTime)
        {
            string operation = "/events/get?";
            string Data;

            Data = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&lastEventId={lastEventId}" + $"&pollTime={pollTime}");
            if (JObject.Parse(Data)["events"].ToArray().Length != 0) { this.lastEventId++; }
            return Data;
        }

        /// <summary>
        /// Метод, возвращающий данные,полученные ботом.
        /// </summary>
        /// <param name="pollTime">Время удержания соединения с сервером.</param>
        /// <returns></returns>
        public string GetBotDataReport(int pollTime)
        {
            string operation = "/events/get?";
            string Data;

            Data = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&lastEventId={this.lastEventId}" + $"&pollTime={pollTime}");
            if (JObject.Parse(Data)["events"].ToArray().Length != 0) { this.lastEventId++; }
            return Data;
        }

        /// <summary>
        /// Метод очистки очереди событий бота.
        /// </summary>
        /// <returns></returns>
        public void GetBotClearAllEventsQueue()
        {
            string operation = "/events/get?";
            string Data = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&lastEventId={1_000_000}" + $"&pollTime={0}");
        }

        /// <summary>
        /// Метод очистки очереди событий бота.
        /// </summary>
        /// <param name="lastEventId">Номер последнего события.</param>
        public void GetBotClearAllEventsQueue(int lastEventId)
        {
            string operation = "/events/get?";
            string Data = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&lastEventId={Convert.ToString(lastEventId)}" + $"&pollTime={0}");
        }
        //Методы работы с файлами: (+)
        /// <summary>
        /// Метод,позволяющий получить информацию о загруженном ранее файле.
        /// </summary>
        /// <param name="FileId">Номер искомого файла.</param>
        /// <returns></returns>
        public string GetBotFileInfo(string FileId)
        {
            string operation = "/files/getInfo?";

            string Info = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&fileId={FileId}");
            return Info;
        }

        /// <summary>
        /// Метод,позволяющий боту выгружать файл пользователю.
        /// </summary>
        /// <param name="ChatId">Номер чата с пользователем.</param>
        /// <param name="FileId">Номер выгружаемого файла.</param>
        public void GetBotSendFile(string ChatId, string FileId)
        {
            string operation = "/messages/sendFile?";
            this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&chatId={ChatId}" + $"&fileId={FileId}");
        }

        /// <summary>
        /// Метод,позволяющий боту выгружать аудио-файл(aac/oog/m4a) пользователю.
        /// </summary>
        /// <param name="ChatId">Номер чата с пользователем.</param>
        /// <param name="FileId">Номер выгружаемого айдио-файла.</param>
        public void GetBotSendVoice(string ChatId, string FileId)
        {
            string operation = "/messages/sendVoice?";
            this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&chatId={ChatId}" + $"&fileId={FileId}");
        }

        //Методы работы с сообщениями: (+)
        /// <summary>
        /// Метод,позволяющий боту отвечать пользователю на сообщения.
        /// </summary>
        /// <param name="ChatId">Номер чата с пользователем.</param>
        /// <param name="Text">Текст ответа.</param>
        public void GetBotSendText(string ChatId, string Text)
        {
            string operation = "/messages/sendText?";
            this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&chatId={ChatId}" + $"&text={Text}");
        }

        /// <summary>
        /// Метод,позволяющий боту редактировать текст сообщения.
        /// </summary>
        /// <param name="ChatId">Номер чата с пользователем.</param>
        /// <param name="MessageId">Номер редактируемого сообщения.</param>
        /// <param name="Text">Текст редактируемого сообщения.</param>
        public void GetBotEditText(string ChatId, int MessageId, string Text)
        {
            string operation = "/messages/editText?";
            this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&chatId={ChatId}" + $"&msgId={MessageId}" + $"&text={Text}");
        }

        //Отсутствует метод удаления сообщений : "/messages/deleteMessages"

        //Методы для работы с кнопками: (+)

        /// <summary>
        /// Метод,добавляющий в чат бота слой для кнопок.
        /// </summary>
        /// <param name="ButtonsNumber">Количество кнопок.</param>
        public void GetBotNewButtonsSlice(int ButtonsNumber)
        {
            this.Buttons.Add(new List<Button>(ButtonsNumber));
        }

        /// <summary>
        /// Метод,добавляющий в чат бота слой для кнопок.
        /// </summary>
        public void GetBotNewButtonsSlice()
        {
            this.Buttons.Add(new List<Button>(0));
        }

        /// <summary>
        /// Метод,добавляющий кнопку в необходимый слой.
        /// </summary>
        /// <param name="Text">Текст на кнопке.</param>
        /// <param name="SliceNumber">Номер необходимого слоя.</param>
        public void GetBotNewButton(string Text, int SliceNumber, string Style)
        {
            Button NewButton = new Button(Text, this.ButtonsNumber, Style);
            this.Buttons[SliceNumber].Add(NewButton);
            this.ButtonsNumber++;
        }

        /// <summary>
        /// Метод,добавляющий в чат бота кнопку(нулевой слой).
        /// </summary>
        /// <param name="Text">Текст на кнопке.</param>
        public void GetBotNewButton(string Text, string Style)
        {
            Button NewButton = new Button(Text, this.ButtonsNumber, Style);
            this.Buttons[0].Add(NewButton);
            this.ButtonsNumber++;
        }

        /// <summary>
        /// Метод,загружащий в определенный чат бота кнопки.
        /// </summary>
        /// <param name="ChatId">Номер чата, куда необходимо загрузить кнопки.</param>
        public void GetBotLoadButtons(string ChatId)
        {
            string operation = "/messages/sendText?";
            string Buttons_Json = JsonConvert.SerializeObject(this.Buttons);
            this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&chatId={ChatId}" + $"&text={" "}" + $"&inlineKeyboardMarkup={Buttons_Json}");
        }

        /// <summary>
        /// Метод,позволяющий получить ответ на запрос кнопки.
        /// </summary>
        /// <param name="QueryId">Номер запроса(event) кнопки.</param>
        /// <returns></returns>
        public string GetBotAnswerCallbackQuery(string QueryId)
        {
            string answer = "";
            string operation = "/messages/answerCallbackQuery?";
            answer = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&queryId={QueryId}");
            return answer;
        }
        /// <summary>
        /// Метод,позволяющий получить ответ на запрос кнопки.
        /// </summary>
        /// <param name="QueryId">Номер запроса(event) кнопки.</param>
        /// <param name="text">Текст, выводимый в чат после нажатия кнопки.</param>
        /// <param name="showAlert">Флаг всплытия окна(true/false).</param>
        /// <returns></returns>
        public string GetBotAnswerCallbackQuery(string QueryId, string text, bool showAlert)
        {
            string operation = "/messages/answerCallbackQuery?";
            string answer = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&queryId={QueryId}" + $"&text={text}" + $"&showAlert={Convert.ToString(showAlert).ToLower()}");
            return answer;
        }
        /// <summary>
        /// Метод,позволяющий получить ответ на запрос кнопки.
        /// </summary>
        /// <param name="QueryId">Номер запроса(event) кнопки.</param>
        /// <param name="url">URL адрес,на который переходит пользователь после нажатия кнопки.</param>
        /// <returns></returns>
        public string GetBotAnswerCallbackQuery(string QueryId, string url)
        {
            string operation = "/messages/answerCallbackQuery?";
            string answer = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&queryId={QueryId}" + $"&url={url}");
            return answer;
        }
        /// <summary>
        /// Метод,позволяющий получить ответ на запрос кнопки.
        /// </summary>
        /// <param name="QueryId">Номер запроса(event) кнопки.</param>
        /// <param name="text">Текст, выводимый в чат после нажатия кнопки.</param>
        /// <param name="showAlert">Флаг всплытия окна(true/false).</param>
        /// <param name="url">URL адрес,на который переходит пользователь после нажатия кнопки.</param>
        /// <returns></returns>
        public string GetBotAnswerCallbackQuery(string QueryId, string text, bool showAlert, string url)
        {
            string operation = "/messages/answerCallbackQuery?";
            string answer = this.webclient.DownloadString(this.api_base_url + operation + $"token={this.token}" + $"&queryId={QueryId}" + $"&text={text}" + $"&showAlert={Convert.ToString(showAlert).ToLower()}" + $"&url={url}");
            return answer;
        }
    }
}
