////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using ab.Model;
using Android.Util;
using Android.Webkit;
using Xamarin.Essentials;

namespace ab.Services
{
    public class MyWebViewClient : WebViewClient
    {
        private int object_id;
        private string hardware_ip_address;

        public delegate void ToastNotifyHandler(int[] resource_id);
        public event ToastNotifyHandler ToastNotify;

        static string TAG = nameof(MyWebViewClient);
        HardwareModel hardware;

        public MyWebViewClient(int id, string ip)
        {
            object_id = id;
            hardware_ip_address = ip;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            //return base.ShouldOverrideUrlLoading(view, request);            
            string url = request.Url.ToString();
            Log.Debug(TAG, url);
            if (request.Url.Host == "ab-log.ru")
            {
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
                url += "?ref=https://github.com/badhitman/ab-log-app";
                Browser.OpenAsync(url, BrowserLaunchMode.External);
                return true;
            }

            string cf = request.Url.GetQueryParameter("cf") ?? string.Empty;
            string eip = request.Url.GetQueryParameter("eip") ?? string.Empty;
            string pwd = request.Url.GetQueryParameter("pwd") ?? string.Empty;

            if (request.Url.Host != hardware_ip_address)
            {
                if(hardware != null)
                view.LoadUrl($"http://{hardware.Address}/{hardware.Password}/?cf=1");
                return true;
            }

            if (cf == "1")
            {
                bool set_ip_address = !string.IsNullOrWhiteSpace(eip) && Regex.IsMatch(eip, @"\d+\.\d+\.\d+\.\d+");
                bool set_password = !string.IsNullOrWhiteSpace(pwd) && pwd.Length < 4;

                if (set_ip_address || set_password)
                {
                    Log.Debug(TAG, "save data to DB");
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            hardware = db.Hardwares.Find(object_id);
                            List<int> messages = new List<int>();
                            if (set_ip_address && hardware.Address != eip)
                            {
                                hardware.Address = hardware_ip_address = eip;
                                messages.Add(Resource.String.hardware_ip_are_saved_title);
                            }
                            if (set_password && hardware.Password != pwd)
                            {
                                hardware.Password = pwd;
                                messages.Add(Resource.String.hardware_password_are_saved_title);
                            }
                            if (messages.Count > 0)
                            {
                                db.Hardwares.Update(hardware);
                                db.SaveChanges();

                                ToastNotify?.Invoke(messages.ToArray());
                            }
                        }
                    }
                }
            }

            if (request.Url.Host == hardware_ip_address)
            {
                Log.Debug(TAG, $"go to -> {request.Url}");
                view.LoadUrl(request.Url.ToString());
            }
            else
            {
                HttpClient httpClient = new HttpClient() { };
                _ = httpClient.GetAsync(request.Url.ToString()).Result;
                Log.Debug(TAG, $"redirect to -> http://{hardware_ip_address}/{pwd}/?cf={cf}");
                
                view.LoadUrl($"http://{hardware_ip_address}/{pwd}/?cf={cf}");
            }
            return true;
        }
    }
}