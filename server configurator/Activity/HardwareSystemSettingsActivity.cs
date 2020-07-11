////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Widget;
using System.Linq;
using System.Threading.Tasks;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class HardwareSystemSettingsActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.hardware_system_settings_activity;
        protected override int ToolbarId => Resource.Id.hardware_system_settings_toolbar;
        protected override int DrawerLayoutId => Resource.Id.hardware_system_settings_app_drawer_layout;
        protected override int NavId => Resource.Id.hardware_system_settings_app_nav_view;

        WebView webView;

        public override void OnBackPressed()
        {
            if (webView?.CanGoBack() ?? false)
            {
                webView.GoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            int id = Intent.Extras.GetInt(nameof(HardwareModel.Id), 0);
            string address = Intent.Extras.GetString(nameof(HardwareModel.Address), string.Empty);
            string password = Intent.Extras.GetString(nameof(HardwareModel.Password), string.Empty);
            webView = (WebView)FindViewById(Resource.Id.webViewSystemSettings);
            MyWebViewClient myWebViewClient = new MyWebViewClient(id, address);
            myWebViewClient.ToastNotify += delegate (int[] resource_id)
            {
                if (resource_id.Length == 0)
                    return;
                string msg = string.Empty;
                foreach (int i in resource_id)
                {
                    msg += $"• {GetString(i)}{System.Environment.NewLine}";
                }
                Toast.MakeText(this, msg.Trim(), ToastLength.Short).Show();
            };
            webView.SetWebViewClient(myWebViewClient);
            //Task.Run(()=> { webView.LoadUrl($"http://{address}/{password}/"); });
            webView.LoadUrl($"http://{address}/{password}/");



            //TabHost tabHost = (TabHost)FindViewById(Resource.Id.tabHost);

            //tabHost.Setup();

            //TabHost.TabSpec tabSpec = tabHost.NewTabSpec("xp1");
            //tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xp1);
            //tabSpec.SetIndicator("XP1");
            //tabHost.AddTab(tabSpec);

            //tabSpec = tabHost.NewTabSpec("xp2");
            //tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xp2);
            //tabSpec.SetIndicator("XP2");
            //tabHost.AddTab(tabSpec);

            //tabSpec = tabHost.NewTabSpec("xt2");
            //tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xt2);
            //tabSpec.SetIndicator("XT2");
            //tabHost.AddTab(tabSpec);

            //tabSpec = tabHost.NewTabSpec("xp56");
            //tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xp56);
            //tabSpec.SetIndicator("XP5/6");
            //tabHost.AddTab(tabSpec);

            //tabHost.CurrentTab = 0;
        }

        //private void MyWebViewClient_ToastNotify(string message)
        //{
        //    throw new System.NotImplementedException();
        //}

        protected override void OnResume()
        {
            base.OnResume();

        }

        protected override void OnPause()
        {
            base.OnPause();

        }
    }
}