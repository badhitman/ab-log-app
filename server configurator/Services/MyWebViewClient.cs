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
        public static readonly string TAG = "● web-client";

        public delegate void ShouldUrlLoadingHandler(string url);
        public event ShouldUrlLoadingHandler ShouldUrlLoading;

        public static string WebPathBase
        {
            get
            {
                string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                string full_www_path = Path.Combine(basePath, "www");
                if (!Directory.Exists(full_www_path))
                {
                    Directory.CreateDirectory(full_www_path);
                }
                return full_www_path;
            }
        }

        public static string bootstrap_min_css => Path.Combine(WebPathBase, "bootstrap.min.css");
        public static string bootstrap_min_js => Path.Combine(WebPathBase, "bootstrap.min.js");
        public static string jquery_slim_min_js => Path.Combine(WebPathBase, "jquery.slim.min.js");
        public static string popper_min_js => Path.Combine(WebPathBase, "popper.min.js");
        public static string onload_root_js => Path.Combine(WebPathBase, "onload.root.js");
        public static string onload_pt_js => Path.Combine(WebPathBase, "onload.pt.js");
        public static string onload_cf1_js => Path.Combine(WebPathBase, "onload.cf1.js");
        public static string onload_cf2_js => Path.Combine(WebPathBase, "onload.cf2.js");
        public static string onload_cf3_js => Path.Combine(WebPathBase, "onload.cf3.js");
        public static string onload_cf4_js => Path.Combine(WebPathBase, "onload.cf4.js");
        public static string onload_cf7_js => Path.Combine(WebPathBase, "onload.cf7.js");
        public static string onload_cf9_js => Path.Combine(WebPathBase, "onload.cf9.js");
        public static string onload_cf10_js => Path.Combine(WebPathBase, "onload.cf10.js");

        [System.Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            Log.Debug(TAG, $"ShouldOverrideUrlLoading - {url}");
            if (ShouldUrlLoading == null)
            {
                return base.ShouldOverrideUrlLoading(view, url);
            }
            //
            ShouldUrlLoading.Invoke(url);
            return true;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            string url = request.Url.ToString();
            Log.Debug(TAG, $"ShouldOverrideUrlLoading - {url}");
            if (ShouldUrlLoading == null)
            {
                return base.ShouldOverrideUrlLoading(view, request);
            }
            ShouldUrlLoading.Invoke(request.Url.ToString());
            return true;
        }
    }
}