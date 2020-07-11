////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using ab.Model;
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

        public MyWebViewClient(int id, string ip)
        {
            object_id = id;
            hardware_ip_address = ip;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            //return base.ShouldOverrideUrlLoading(view, request);
            string cf = request.Url.GetQueryParameter("cf") ?? string.Empty;
            string eip = request.Url.GetQueryParameter("eip") ?? string.Empty;
            string pwd = request.Url.GetQueryParameter("pwd") ?? string.Empty;

            if (request.Url.Host != hardware_ip_address)
            {
                //ref=https://github.com/badhitman/ab-log-app
                string url = request.Url.ToString();
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
                url += "?ref=https://github.com/badhitman/ab-log-app";
                Browser.OpenAsync(url, BrowserLaunchMode.External);
                return true;
            }

            if (cf == "1")
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        HardwareModel hardware = db.Hardwares.Find(this.object_id);
                        List<int> messages = new List<int>();
                        if (!string.IsNullOrWhiteSpace(eip) && Regex.IsMatch(eip, @"\d+\.\d+\.\d+\.\d+") && hardware.Address != eip)
                        {
                            hardware.Address = hardware_ip_address = eip;
                            messages.Add(Resource.String.hardware_ip_are_saved_title);
                        }
                        if (!string.IsNullOrWhiteSpace(pwd) && pwd.Length <= 3 && hardware.Password != pwd)
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

            if (request.Url.Host == hardware_ip_address)
            {
                view.LoadUrl(request.Url.ToString());
            }
            else
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage httpResponseMessage = httpClient.GetAsync(request.Url.ToString()).Result;

                view.LoadUrl($"http://{hardware_ip_address}/{pwd}/?cf={cf}");
            }

            return true;
        }
    }
}