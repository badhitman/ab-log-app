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
using System.Text.RegularExpressions;
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

            Android.Net.Uri uri = Android.Net.Uri.Parse(request.Url.ToString());
            string remote_ip_address = request.RemoteEndPoint.Address.ToString();

            string pt = uri.GetQueryParameter("pt");
            string dir = uri.GetQueryParameter("dir");
            string mdid = uri.GetQueryParameter("mdid");
            string v = uri.GetQueryParameter("v");
            string value = uri.GetQueryParameter("value");
            string st = uri.GetQueryParameter("st");
            string m = uri.GetQueryParameter("m");
            string click = uri.GetQueryParameter("click");
            string cnt = uri.GetQueryParameter("cnt");

            HardwareModel hardware;
            string hw_name = string.Empty;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    hardware = db.Hardwares.FirstOrDefault(x => x.Address == remote_ip_address);
                }
            }

            hw_name = hardware?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(hw_name))
            {
                hw_name = remote_ip_address;
            }
            else
            {
                hw_name += $" ({remote_ip_address})";

            }

            string bot_message = $"Сообщение от устройства \"{hw_name}\": {Environment.NewLine}";

            if (!string.IsNullOrWhiteSpace(pt))
            {
                if (Regex.IsMatch(pt, @"^\d+$"))
                {
                    PortHardwareModel portHardware = null;
                    int pt_num_int = 0;
                    pt_num_int = int.Parse(pt);
                    if (pt_num_int > 0 && hardware != null)
                    {
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                portHardware = db.PortsHardwares.FirstOrDefault(x => x.HardwareId == hardware.Id && x.PortNumb == pt_num_int);
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(pt) && string.IsNullOrWhiteSpace(portHardware?.Name))
                    {
                        pt = $"Порт: P{pt}; ";
                    }
                    else
                    {
                        pt = $"Порт: \"{portHardware.Name}\" (P{pt}); ";
                    }
                }
                else
                {
                    pt = $"Порт: {pt}; ";
                }
                bot_message += $"{pt}";
            }

            if (!string.IsNullOrWhiteSpace(m))
            {
                m = (m == "1" ? "Освобождние после длительного удержания" : $"Длительное удержание") + "; ";
                bot_message += $"{m}";
            }

            if (!string.IsNullOrWhiteSpace(click))
            {
                click = (click == "1" ? "обычный клик" : "двойной клик") + "; ";
                bot_message += $"{click}";
            }

            if (!string.IsNullOrWhiteSpace(dir))
            {
                if (dir == "0")//падение значения
                {
                    dir = "Значение опустилось ниже порога; ";
                }
                else if (dir == "1")//повышение знчения
                {
                    dir = "Значение превышает пороговое; ";
                }
                else
                {
                    dir = $"ошибка определения вектора 'dir':{dir}; ";
                }
                bot_message += $"{dir}";
            }

            if (!string.IsNullOrWhiteSpace(mdid))
            {
                mdid = $"mdid=\"{mdid}\"; ";
                bot_message += $"{mdid}";
            }

            if (!string.IsNullOrWhiteSpace(st))
            {
                st = $"st=\"{st}\"; ";
                bot_message += $"{st}";
            }

            if (!string.IsNullOrWhiteSpace(v))
            {
                v = $"v=\"{v}\"; ";
                bot_message += $"{v}";
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                value = $"value=\"{value}\"; ";
                bot_message += $"{value}";
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
                                    telegramClient.sendMessage(telegramUser.TelegramId.ToString(), bot_message.Trim());
                                }
                            }
                        }
                    }
                }
            }



            string log_msg = $"incoming http request (from > {remote_ip_address}): {request.Url.Query}";
            //log_msg = "telegramClient?.Me == null";
            Log.Info(TAG, log_msg);
            using (LogsContext log = new LogsContext())
            {
                log.AddLogRow(LogStatusesEnum.Info, log_msg, TAG);
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