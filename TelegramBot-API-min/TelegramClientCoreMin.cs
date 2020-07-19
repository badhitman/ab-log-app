////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using SimpleWebClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using TelegramBot.TelegramMetadata;
using TelegramBot.TelegramMetadata.AvailableTypes;
using TelegramBot.TelegramMetadata.GettingUpdates;
using TelegramBot.TelegramMetadata.Methods.Metadata;
using TelegramBot.TelegramMetadata.MethodsMetadata.Metadata;
using static TelegramBot.TelegramMetadata.AvailableTypes.MessageClass;

namespace TelegramBotMin
{
    /// <summary>
    /// Типы данных сообщения
    /// </summary>
    public enum TelegramDataTypes
    {
        /// <summary>
        /// Признак того что к сообщению прикреплена локация
        /// </summary>
        Location,

        /// <summary>
        /// Признак того что в сообщении есть локация
        /// </summary>
        LocationText,

        /// <summary>
        /// Признак того что к сообщению прикреплено фото
        /// </summary>
        Photo,

        /// <summary>
        /// Признак того что к сообщению прикреплено видео
        /// </summary>
        Video,

        /// <summary>
        /// Признак того что к сообщению прикреплено аудио
        /// </summary>
        Audio,

        /// <summary>
        /// Признак того что к сообщению прикреплен документ
        /// </summary>
        Document,

        /// <summary>
        /// Признак того что у сообщения назначено описание медиа-данных
        /// </summary>
        Caption,

        /// <summary>
        /// Признак того что в сообщении есть текст
        /// </summary>
        Text,

        /// <summary>
        /// Признак того данное сообщение-уведомление говорит о том что в групу добавлен новый учасник
        /// </summary>
        NewChatMembers
    }

    /// <summary>
    /// Все методы в Bot API нечувствительны к регистру. Мы поддерживаем методы HTTP GET и POST.
    /// Для передачи параметров в запросах Bot API используйте URL запросы (https://en.wikipedia.org/wiki/Query_string)
    /// или application/json или application/x-www-form-urlencoded или multipart/form-data
    /// При успешном вызове возвращается JSON-объект, содержащий результат.
    /// 
    /// Authorizing your bot
    /// 
    /// Each bot is given a unique authentication token when it is created.The token looks something like 123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11, but we'll use simply <token> in this document instead. You can learn about obtaining tokens and generating new ones in this document.
    /// Making requests
    /// 
    /// All queries to the Telegram Bot API must be served over HTTPS and need to be presented in this form: https://api.telegram.org/bot<token>/METHOD_NAME. Like this for example:
    /// 
    /// https://api.telegram.org/bot123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11/getMe
    /// 
    /// We support GET and POST HTTP methods.We support four ways of passing parameters in Bot API requests:
    /// 
    /// URL query string
    /// application/x-www-form-urlencoded
    /// application/json (except for uploading files)
    /// multipart/form-data (use to upload files)
    /// 
    /// The response contains a JSON object, which always has a Boolean field ‘ok’ and may have an optional String field ‘description’ with a human-readable description of the result. If ‘ok’ equals true, the request was successful and the result of the query can be found in the ‘result’ field. In case of an unsuccessful request, ‘ok’ equals false and the error is explained in the ‘description’. An Integer ‘error_code’ field is also returned, but its contents are subject to change in the future. Some errors may also have an optional field ‘parameters’ of the type ResponseParameters, which can help to automatically handle the error.
    /// 
    /// All methods in the Bot API are case-insensitive.
    /// All queries must be made using UTF-8.
    /// </summary>
    public class TelegramClientCore
    {
        public enum LogMode { Info, Ok, Err, Alert, Trace };

        public delegate void onLogReceivedEvent(string msg, LogMode lm);
        public event onLogReceivedEvent onLogEvent;

        public string http_response_raw { get; private set; } = "";
        public long offset;

        public string HttpRrequestStatus => MyWebClient.HttpRrequestStatus;

        public UserClass Me;
        private string apiUrl { get { return "https://api.telegram.org/bot" + api_bot_token; } }
        private string apiFileUrl { get { return "https://api.telegram.org/file/bot" + api_bot_token + "/"; } }
        private string api_bot_token;

        public TelegramClientCore(string _api_bot_token)
        {
            api_bot_token = _api_bot_token;
            Me = getMe().Result;
        }

        internal void onLogReceivedCall(string msg_txt, LogMode lm)
        {
            if (onLogEvent != null)
                onLogEvent(msg_txt, lm);
        }

        private async void SendRequest(string api_bot_method_name, NameValueCollection request_param, InputFileClass file_post)
        {
            http_response_raw = "";
            if (request_param != null && !string.IsNullOrEmpty(request_param["captiom"]) && string.IsNullOrWhiteSpace(request_param["parse_mode"]))
                request_param["captiom"] = HttpUtility.UrlEncode(request_param["captiom"]);

            await Task.Run(() =>
            {
                http_response_raw = MyWebClient.SendRequest(apiUrl + "/" + api_bot_method_name, HttpMethod.Post, request_param, new List<PostedFile>() { new PostedFile() { Data = file_post.Data, FieldName = file_post.FieldName, FileName = file_post.FileName } }, MyWebClient.RequestContentTypes.MultipartFormData);
            });
        }

        private async void SendRequest(string api_bot_method_name, NameValueCollection request_param)
        {
            http_response_raw = "";
            if (request_param != null && !string.IsNullOrEmpty(request_param["text"]) && string.IsNullOrWhiteSpace(request_param["parse_mode"]))
                request_param["text"] = HttpUtility.UrlEncode(request_param["text"]);

            await Task.Run(() =>
            {
                http_response_raw = MyWebClient.SendRequest(apiUrl + "/" + api_bot_method_name, HttpMethod.Post, request_param, null, MyWebClient.RequestContentTypes.ApplicationXWwwFormUrlencoded);
            });
        }

        /// <summary>
        /// Скачать файл из облака Telegram
        /// </summary>
        /// <param name="t_file">Файл для загрузки</param>
        /// <returns>байты данных</returns>
        public async Task<byte[]> DownloadTelegramFile(FileClass t_file)
        {
            onLogReceivedCall("Попытка загрузки файла: id[" + t_file.file_id + "] path[" + t_file.file_path + "]", LogMode.Trace);
            byte[] t_bytes_data = new byte[0];
            using (WebClient wc = new WebClient())
            {
                try
                {
                    await Task.Run(() =>
                    {
                        t_bytes_data = wc.DownloadData(apiFileUrl + t_file.file_path);
                    });
                    onLogReceivedCall("OK. Файл загружен", LogMode.Ok);
                }
                catch (WebException we)
                {
                    onLogReceivedCall("Ошибка загрузкий файла: " + we.Message, LogMode.Err);
                }
            }
            return t_bytes_data;
        }

        /// <summary>
        /// Получить входящие обновления в бот
        /// </summary>
        /// <param name="_limit">Димит на получение данных</param>
        /// <returns></returns>
        public async Task<Update[]> getUpdates(int _limit = 10)
        {
            getUpdatesJSON updates_filter = new getUpdatesJSON()
            {
                offset = offset + 1,
                limit = _limit,
                allowed_updates = new string[0]
            };

            await Task.Run(() =>
            {
                SendRequest(nameof(getUpdates), updates_filter.GetFiealds(new string[0]));
            });

            if (string.IsNullOrEmpty(http_response_raw))
                return new Update[0];

            getUpdatesJSON.Result result = (getUpdatesJSON.Result) await SerialiserJSON.ReadObject(typeof(getUpdatesJSON.Result), http_response_raw);
            return result.result;
        }

        #region basic

        /// <summary>
        /// A simple method for testing your bot's auth token. Requires no parameters.
        /// </summary>
        /// <returns>Returns basic information about the bot in form of a User (https://core.telegram.org/bots/api#user) object.</returns>
        public async Task<UserClass> getMe()
        {
            SendRequest(nameof(getMe), null);
            if (string.IsNullOrEmpty(http_response_raw))
                return null;
            getMeJSON.Result result = (getMeJSON.Result) await SerialiserJSON.ReadObject(typeof(getMeJSON.Result), http_response_raw);
            return result.result;
        }

        /// <summary>
        /// Use this method to send text messages.
        /// </summary>
        /// <param name="chat_id">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="text">Optional	Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="parse_mode">Send Markdown (https://core.telegram.org/bots/api#markdown-style) or HTML (https://core.telegram.org/bots/api#html-style), if you want Telegram apps to show bold, italic, fixed-width text or inline URLs (https://core.telegram.org/bots/api#formatting-options) in your bot's message.
        /// Formatting options
        /// The Bot API supports basic formatting for messages. You can use bold and italic text, as well as inline links and pre-formatted code in your bots' messages. Telegram clients will render them accordingly. You can use either markdown-style or HTML-style formatting.
        /// 
        /// Note that Telegram clients will display an alert to the user before opening an inline link (‘Open this link?’ together with the full URL).
        /// 
        /// Links 'tg://user?id=<user_id>' can be used to mention a user by their id without using a username. Please note:
        /// 
        /// These links will work only if they are used inside an inline link.
        /// These mentions are only guaranteed to work if the user has contacted the bot in the past or is a member in the group where he was mentioned.
        /// Markdown style
        /// To use this mode, pass Markdown in the parse_mode field when using sendMessage. Use the following syntax in your message:
        /// 
        /// *bold text*
        /// _italic text_
        /// [inline URL](http://www.example.com/)
        /// [inline mention of a user](tg://user?id=123456789)
        /// `inline fixed-width code`
        /// ```block_language
        /// pre-formatted fixed-width code block
        /// ```
        /// HTML style
        /// To use this mode, pass HTML in the parse_mode field when using sendMessage. The following tags are currently supported:
        /// 
        /// <b>bold</b>, <strong>bold</strong>
        /// <i>italic</i>, <em>italic</em>
        /// <a href="http://www.example.com/">inline URL</a>
        /// <a href="tg://user?id=123456789">inline mention of a user</a>
        /// <code>inline fixed-width code</code>
        /// <pre>pre-formatted fixed-width code block</pre>
        /// Please note:
        /// 
        /// Only the tags mentioned above are currently supported.
        /// Tags must not be nested.
        /// All <, > and & symbols that are not a part of a tag or an HTML entity must be replaced with the corresponding HTML entities (< with &lt;, > with &gt; and & with &amp;).
        /// All numerical HTML entities are supported.
        /// The API currently supports only the following named HTML entities: &lt;, &gt;, &amp; and &quot;.
        /// </param>
        /// <param name="disable_web_page_preview">Optional	Disables link previews for links in this message</param>
        /// <param name="disable_notification">Optional	Sends the message silently (https://telegram.org/blog/channels-2-0#silent-messages). Users will receive a notification with no sound.</param>
        /// <param name="reply_to_message_id">Optional	If the message is a reply, ID of the original message</param>
        /// <param name="reply_markup">InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply [Optional] Additional interface options. A JSON-serialized object for an inline keyboard (https://core.telegram.org/bots#inline-keyboards-and-on-the-fly-updating), custom reply keyboard (https://core.telegram.org/bots#keyboards), instructions to remove reply keyboard or to force a reply from the user.</param>
        /// <returns>On success, the sent Message (https://core.telegram.org/bots/api#message) is returned.</returns>
        public async Task<MessageClass> sendMessage(string chat_id, string text, ParseModes? parse_mode = null, bool disable_web_page_preview = false, bool disable_notification = false, long reply_to_message_id = 0, object reply_markup = null)
        {
            sendMessageJSON send_msg_json = new sendMessageJSON()
            {
                chat_id = chat_id,
                text = text
            };

            List<string> skip_fields = new List<string>();

            if (parse_mode is null)
                skip_fields.Add("parse_mode");
            else
                send_msg_json.parse_mode = parse_mode.ToString();

            if (!disable_notification)
                skip_fields.Add("disable_notification");

            if (reply_to_message_id == 0)
                skip_fields.Add("reply_to_message_id");
            else
                send_msg_json.reply_to_message_id = reply_to_message_id;

            if (reply_markup == null)
                skip_fields.Add("reply_markup");

            SendRequest(nameof(sendMessage), send_msg_json.GetFiealds(skip_fields.ToArray()));

            if (string.IsNullOrEmpty(http_response_raw))
                return null;
            sendMessageJSON.Result result = (sendMessageJSON.Result)await SerialiserJSON.ReadObject(typeof(sendMessageJSON.Result), http_response_raw);
            return result.result;
        }

        /// <summary>
        /// Use this method to forward messages of any kind.
        /// </summary>
        /// <param name="chat_id">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="from_chat_id">Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)</param>
        /// <param name="message_id">Message identifier in the chat specified in from_chat_id</param>
        /// <param name="disable_notification">Sends the message silently (https://telegram.org/blog/channels-2-0#silent-messages). Users will receive a notification with no sound.</param>
        /// <returns>On success, the sent Message is returned.</returns>
        public async Task<MessageClass> forwardMessage(string chat_id, string from_chat_id, long message_id, bool disable_notification = false)
        {
            forwardMessageJSON forward_msg_json = new forwardMessageJSON()
            {
                chat_id = chat_id,
                from_chat_id = from_chat_id,
                message_id = message_id
            };
            List<string> skip_fields = new List<string>();
            if (!disable_notification)
                skip_fields.Add("disable_notification");

            SendRequest(nameof(forwardMessage), forward_msg_json.GetFiealds(skip_fields.ToArray()));
            if (string.IsNullOrEmpty(http_response_raw))
                return null;
            forwardMessageJSON.Result result = (forwardMessageJSON.Result)await SerialiserJSON.ReadObject(typeof(forwardMessageJSON.Result), http_response_raw);
            return result.result;
        }

        /// <summary>
        /// Use this method to send general files. On success, the sent Message is returned. Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
        /// </summary>
        /// <param name="chat_id">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="document">File to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one using multipart/form-data. More info on Sending Files »</param>
        /// <param name="thumb">InputFile or String 	Optional 	Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail‘s width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can’t be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>. More info on Sending Files »</param>
        /// <param name="caption">Document caption (may also be used when resending documents by file_id), 0-200 characters</param>
        /// <param name="disable_notification">Sends the message silently. Users will receive a notification with no sound.</param>
        /// <param name="reply_to_message_id">If the message is a reply, ID of the original message</param>
        /// <param name="reply_markup">InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply [Optional] Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.</param>
        /// <returns>On success, the sent Message is returned</returns>
        public async Task<MessageClass> sendDocument(string chat_id, object document, string caption = null, object thumb = null, ParseModes? parse_mode = null, bool disable_notification = false, long reply_to_message_id = 0, object reply_markup = null)
        {
            sendDocumentJSON send_document_json = new sendDocumentJSON()
            {
                chat_id = chat_id
            };

            List<string> skip_fields = new List<string>() { "document", "thumb" };

            if (string.IsNullOrEmpty(caption))
                skip_fields.Add("caption");

            if (parse_mode is null)
                skip_fields.Add("parse_mode");
            else
                send_document_json.parse_mode = parse_mode.ToString();

            if (!disable_notification)
                skip_fields.Add("disable_notification");

            if (reply_to_message_id == 0)
                skip_fields.Add("reply_to_message_id");
            else
                send_document_json.reply_to_message_id = reply_to_message_id.ToString();

            if (reply_markup == null)
                skip_fields.Add("reply_markup");

            if (document is string || document is int)
            {
                SendRequest(nameof(sendDocument), send_document_json.GetFiealds(skip_fields.ToArray()));
                if (string.IsNullOrEmpty(http_response_raw))
                    return null;
            }
            else if (document is InputFileClass)
            {
                skip_fields.Add("document");
                SendRequest(nameof(sendDocument), send_document_json.GetFiealds(skip_fields.ToArray()), (InputFileClass)document);
                if (string.IsNullOrEmpty(http_response_raw))
                    return null;
            }
            sendDocumentJSON.Result result = (sendDocumentJSON.Result)await SerialiserJSON.ReadObject(typeof(sendDocumentJSON.Result), http_response_raw);
            return result.result;
        }

        /// <summary>
        /// Use this method to send photos
        /// </summary>
        /// <param name="chat_id">Unique identifier for the target chat or username of the target channel (in the format @channelusername)</param>
        /// <param name="photo">Photo to send. Pass a file_id as String to send a photo that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a photo from the Internet, or upload a new photo using multipart/form-data. More info on Sending Files » https://core.telegram.org/bots/api#sending-files </param>
        /// <param name="caption">Photo caption (may also be used when resending photos by file_id), 0-200 characters</param>
        /// <param name="disable_notification">Sends the message silently (https://telegram.org/blog/channels-2-0#silent-messages). Users will receive a notification with no sound.</param>
        /// <param name="reply_to_message_id">If the message is a reply, ID of the original message</param>
        /// <param name="reply_markup">InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply [Optional] Additional interface options. A JSON-serialized object for an inline keyboard (https://core.telegram.org/bots#inline-keyboards-and-on-the-fly-updating), custom reply keyboard (https://core.telegram.org/bots#keyboards), instructions to remove reply keyboard or to force a reply from the user.</param>
        /// <returns>On success, the sent Message is returned.</returns>
        public async Task<MessageClass> sendPhoto(string chat_id, object photo, string caption = null, ParseModes? parse_mode = null, bool disable_notification = false, long reply_to_message_id = -1, object reply_markup = null)
        {
            sendPhotoJSON send_photo_json = new sendPhotoJSON()
            {
                chat_id = chat_id
            };

            List<string> skip_fields = new List<string>();

            if (string.IsNullOrEmpty(caption))
                skip_fields.Add("caption");
            else
                send_photo_json.caption = caption;

            if (parse_mode is null)
                skip_fields.Add("parse_mode");
            else
                send_photo_json.parse_mode = parse_mode.ToString();

            if (!disable_notification)
                skip_fields.Add("disable_notification");

            if (reply_to_message_id == 0)
                skip_fields.Add("reply_to_message_id");
            else
                send_photo_json.reply_to_message_id = reply_to_message_id.ToString();

            if (reply_markup == null)
                skip_fields.Add("reply_markup");


            if (photo is string || photo is int)
            {
                send_photo_json.photo = photo;
                SendRequest(nameof(sendPhoto), send_photo_json.GetFiealds(skip_fields.ToArray()));
                if (string.IsNullOrEmpty(http_response_raw))
                    return null;
            }
            else if (photo is InputFileClass)
            {
                skip_fields.Add("photo");
                SendRequest(nameof(sendPhoto), send_photo_json.GetFiealds(skip_fields.ToArray()), (InputFileClass)photo);
                if (string.IsNullOrEmpty(http_response_raw))
                    return null;
            }
            sendPhotoJSON.Result result = (sendPhotoJSON.Result)await SerialiserJSON.ReadObject(typeof(sendPhotoJSON.Result), http_response_raw);
            return result.result;
        }

        /// <summary>
        /// Use this method to get basic info about a file and prepare it for downloading. For the moment, bots can download files of up to 20MB in size. The file can then be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>, where <file_path> is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile again.
        /// Note: This function may not preserve the original file name and MIME type. You should save the file's MIME type and name (if available) when the File object is received.
        /// </summary>
        /// <param name="file_id">File identifier to get info about</param>
        /// <returns>On success, a File object is returned.</returns>
        public async Task<FileClass> getFile(string file_id)
        {
            getFileJSON get_file_json = new getFileJSON() { file_id = file_id };
            SendRequest(nameof(getFile), get_file_json.GetFiealds(new string[0]));
            if (string.IsNullOrEmpty(http_response_raw.Trim()))
                return null;
            getFileJSON.Result result = (getFileJSON.Result)await SerialiserJSON.ReadObject(typeof(getFileJSON.Result), http_response_raw);
            return result.result;
        }

        /// <summary>
        /// Use this method to get a list of administrators in a chat. If the chat is a group or a supergroup and no administrators were appointed, only the creator will be returned.
        /// </summary>
        /// <param name="chat_id">Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)</param>
        /// <returns>On success, returns an Array of ChatMember objects that contains information about all chat administrators except other bots.</returns>
        public async Task<ChatMemberClass[]> getChatAdministrators(string chat_id)
        {
            getChatAdministratorsJSON get_chat_administrators_json = new getChatAdministratorsJSON() { chat_id = chat_id };
            SendRequest(nameof(getChatAdministrators), get_chat_administrators_json.GetFiealds(new string[0]));
            if (string.IsNullOrEmpty(http_response_raw.Trim()))
                return null;
            getChatAdministratorsJSON.Result result = (getChatAdministratorsJSON.Result)await SerialiserJSON.ReadObject(typeof(getChatAdministratorsJSON.Result), http_response_raw);
            return result.result;
        }

        #endregion
    }
}
