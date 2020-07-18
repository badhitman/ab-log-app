////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.Util;
using System;
using System.Net;
using System.Text;

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

        public async void StartForegroundService(int service_port)
        {
            string msg = $"StartForegroundService(port={service_port})";
            Log.Debug(TAG, msg);
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, msg, TAG);

            HttpListenerPort = service_port;
            httpServer.Prefixes.Add($"http://*:{service_port}/");
            httpServer.Start();
            IAsyncResult result = httpServer.BeginGetContext(new AsyncCallback(ListenerCallback), httpServer);
        }

        public async void StopForegroundService()
        {
            Log.Debug(TAG, "StopForegroundService()");
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, "StopForegroundService()", TAG);

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
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, s_request, TAG);

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
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