////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.Content.Res;
using Android.Util;
using Android.Webkit;
using System.IO;
using System.Reflection;

namespace ab.Services
{
    public class MyWebViewClient : WebViewClient
    {
        private int object_id;
        private string hardware_ip_address;

        public delegate void ToastNotifyHandler(int[] resource_id);
        public event ToastNotifyHandler ToastNotify;

        public delegate void ShouldUrlLoadingHandler(string url);
        public event ShouldUrlLoadingHandler ShouldUrlLoading;

        static string TAG = nameof(MyWebViewClient);
        HardwareModel hardware;

        public static string WebPathBase
        {
            get
            {
                string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                string full_www_path = Path.Combine(basePath, "www");
                if(!Directory.Exists(full_www_path))
                {
                    Directory.CreateDirectory(full_www_path);
                }
                return full_www_path;
            }
        }

        public static string bootstrap_min_css
        {
            get
            {
                string file_name = "bootstrap.min.css";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string bootstrap_min_js
        {
            get
            {
                string file_name = "bootstrap.min.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string jquery_slim_min_js
        {
            get
            {
                string file_name = "jquery.slim.min.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string popper_min_js
        {
            get
            {
                string file_name = "popper.min.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_root_js
        {
            get
            {
                string file_name = "onload.root.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_pt_js
        {
            get
            {
                string file_name = "onload.pt.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_cf1_js
        {
            get
            {
                string file_name = "onload.cf1.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_cf2_js
        {
            get
            {
                string file_name = "onload.cf2.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_cf3_js
        {
            get
            {
                string file_name = "onload.cf3.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_cf4_js
        {
            get
            {
                string file_name = "onload.cf4.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_cf7_js
        {
            get
            {
                string file_name = "onload.cf7.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        public static string onload_cf9_js
        {
            get
            {
                string file_name = "onload.cf9.js";
                string file_path = Path.Combine(WebPathBase, file_name);
                return file_path;
            }
        }

        //public MyWebViewClient(int id, string ip)
        //{
        //    object_id = id;
        //    hardware_ip_address = ip;
        //}

        [System.Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            //return base.ShouldOverrideUrlLoading(view, url);
            ShouldUrlLoading?.Invoke(url);
            Log.Debug(TAG, $"ShouldOverrideUrlLoading - {url}");
            return true;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            //return base.ShouldOverrideUrlLoading(view, request);
            ShouldUrlLoading?.Invoke(request.Url.ToString());
            string url = request.Url.ToString();
            Log.Debug(TAG, $"ShouldOverrideUrlLoading - {url}");
            return true;
            //if (request.Url.Host == "ab-log.ru")
            //{
            //    if (!url.EndsWith("/"))
            //    {
            //        url += "/";
            //    }
            //    url += "?ref=https://github.com/badhitman/ab-log-app";
            //    Browser.OpenAsync(url, BrowserLaunchMode.External);
            //    return true;
            //}

            //string cf = request.Url.GetQueryParameter("cf") ?? string.Empty;
            //string eip = request.Url.GetQueryParameter("eip") ?? string.Empty;
            //string pwd = request.Url.GetQueryParameter("pwd") ?? string.Empty;

            //if (request.Url.Host != hardware_ip_address)
            //{
            //    if (hardware != null)
            //        view.LoadUrl($"http://{hardware.Address}/{hardware.Password}/?cf=1");
            //    return true;
            //}

            //if (cf == "1")
            //{
            //    bool set_ip_address = !string.IsNullOrWhiteSpace(eip) && Regex.IsMatch(eip, @"\d+\.\d+\.\d+\.\d+");
            //    bool set_password = !string.IsNullOrWhiteSpace(pwd) && pwd.Length < 4;

            //    if (set_ip_address || set_password)
            //    {
            //        Log.Debug(TAG, "save data to DB");
            //        lock (DatabaseContext.DbLocker)
            //        {
            //            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
            //            {
            //                hardware = db.Hardwares.Find(object_id);
            //                List<int> messages = new List<int>();
            //                if (set_ip_address && hardware.Address != eip)
            //                {
            //                    hardware.Address = hardware_ip_address = eip;
            //                    messages.Add(Resource.String.hardware_ip_are_saved_title);
            //                }
            //                if (set_password && hardware.Password != pwd)
            //                {
            //                    hardware.Password = pwd;
            //                    messages.Add(Resource.String.hardware_password_are_saved_title);
            //                }
            //                if (messages.Count > 0)
            //                {
            //                    db.Hardwares.Update(hardware);
            //                    db.SaveChanges();

            //                    ToastNotify?.Invoke(messages.ToArray());
            //                }
            //            }
            //        }
            //    }
            //}

            //if (request.Url.Host == hardware_ip_address)
            //{
            //    Log.Debug(TAG, $"go to -> {request.Url}");
            //    view.LoadUrl(request.Url.ToString());
            //}
            //else
            //{
            //    HttpClient httpClient = new HttpClient() { };
            //    _ = httpClient.GetAsync(request.Url.ToString()).Result;
            //    Log.Debug(TAG, $"redirect to -> http://{hardware_ip_address}/{pwd}/?cf={cf}");

            //    view.LoadUrl($"http://{hardware_ip_address}/{pwd}/?cf={cf}");
            //}
            //return true;
        }
    }
}