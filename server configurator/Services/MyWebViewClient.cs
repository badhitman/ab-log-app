////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

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
                        if (Regex.IsMatch(eip, @"\d+\.\d+\.\d+\.\d+"))
                        {
                            hardware.Address = hardware_ip_address = eip;
                        }
                        if (!string.IsNullOrWhiteSpace(pwd) && pwd.Length <= 3)
                        {
                            hardware.Password = pwd;
                        }
                        db.Hardwares.Update(hardware);
                        db.SaveChanges();
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
                HttpResponseMessage v = httpClient.GetAsync(request.Url.ToString()).Result;

                view.LoadUrl($"http://{hardware_ip_address}/{pwd}/?cf={cf}");
            }

            return true;
        }
    }
}