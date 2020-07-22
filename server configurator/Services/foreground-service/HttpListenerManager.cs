////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Text;
using TelegramBotMin;
using Xamarin.Essentials;

namespace ab.Services
{
    public class HttpListenerManager : IForegroundService
    {
        static readonly string TAG = "http-manager";
        private readonly HttpListener httpServer;
        LogsContext logsDB = new LogsContext();
        public IPAddress ipAddress => Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        public bool isStartedForegroundService { get { return httpServer?.IsListening ?? false; } }
        public int HttpListenerPort { get; private set; }

        public HttpListenerManager()
        {
            Log.Debug(TAG, "~ constructor");

            httpServer = new HttpListener();
        }

        public void StartForegroundService(int service_port)
        {
            string msg = $"StartForegroundService(port={service_port})";
            Log.Debug(TAG, msg);
            logsDB.AddLogRow(LogStatusesEnum.Trac, msg, TAG);

            HttpListenerPort = service_port;
            httpServer.Prefixes.Add($"http://*:{service_port}/");
            httpServer.Start();
            IAsyncResult result = httpServer.BeginGetContext(new AsyncCallback(ListenerCallback), httpServer);
        }

        public void StopForegroundService()
        {
            Log.Debug(TAG, "StopForegroundService()");
            logsDB.AddLogRow(LogStatusesEnum.Trac, "StopForegroundService()", TAG);

            httpServer.Stop();
            httpServer.Prefixes.Clear();
        }

        public async void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            if (!listener.IsListening)
            {
                return;
            }
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            string s_request = $"ListenerCallback() - request: {request.Url}";
            Log.Debug(TAG, s_request);
            logsDB.AddLogRow(LogStatusesEnum.Trac, s_request, TAG);

            // Obtain a response object.
            HttpListenerResponse response = context.Response;

            string remote_ip_address = request.RemoteEndPoint.Address.ToString();
            string clear_query = request.Url.Query;
            if (clear_query.StartsWith("?"))
            {
                clear_query = clear_query.Substring(1);
            }

            string log_msg = $"incoming http request (from > {remote_ip_address}): {clear_query}";
            //log_msg = "telegramClient?.Me == null";
            Log.Info(TAG, log_msg);
            using (LogsContext log = new LogsContext())
            {
                log.AddLogRow(LogStatusesEnum.Info, log_msg, TAG);
            }

            HardwareModel hardwareModel;
            string hw_name = string.Empty;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    hardwareModel = db.Hardwares.FirstOrDefault(x => x.Address == remote_ip_address);
                    hw_name = hardwareModel?.Name ?? string.Empty;
                    if (string.IsNullOrEmpty(hw_name))
                    {
                        hw_name = remote_ip_address;
                    }
                    else
                    {
                        hw_name += $" ({remote_ip_address})";
                    }
                }
            }

            string token = Preferences.Get(Constants.TELEGRAM_TOKEN, string.Empty);
            if (!string.IsNullOrWhiteSpace(token))
            {
                TelegramClientCore telegramClient = new TelegramClientCore(token);
                if (telegramClient?.Me != null)
                {
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            foreach (UserModel user in db.Users.Where(x => x.AlarmSubscriber))
                            {
                                foreach (TelegramUserModel telegramUser in db.TelegramUsers.Where(xx => xx.LinkedUserId == user.Id))
                                {
                                    telegramClient.sendMessage(telegramUser.TelegramId.ToString(), $"Сообщение от: {hw_name}{Environment.NewLine}{clear_query}");
                                }
                            }
                        }
                    }
                }
            }



            // Construct a response.
            string responseString = "Hello world!";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            result = httpServer.BeginGetContext(new AsyncCallback(ListenerCallback), httpServer);
        }
    }
}