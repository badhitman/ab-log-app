////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Util;
using Android.Webkit;
using System.IO;

namespace ab.Services
{
    public class MyWebViewClient : WebViewClient
    {
        public delegate void ShouldUrlLoadingHandler(string url);
        public event ShouldUrlLoadingHandler ShouldUrlLoading;

        static string TAG = nameof(MyWebViewClient);

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

        [System.Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            //return base.ShouldOverrideUrlLoading(view, url);
            Log.Debug(TAG, $"ShouldOverrideUrlLoading - {url}");
            ShouldUrlLoading?.Invoke(url);
            return true;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            //return base.ShouldOverrideUrlLoading(view, request);
            string url = request.Url.ToString();
            Log.Debug(TAG, $"ShouldOverrideUrlLoading - {url}");
            ShouldUrlLoading?.Invoke(request.Url.ToString());
            return true;
        }
    }
}