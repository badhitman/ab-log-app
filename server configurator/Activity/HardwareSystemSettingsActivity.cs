////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Widget;

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
        TextView hardwareCardSubHeader;

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
            
            hardwareCardSubHeader = FindViewById<TextView>(Resource.Id.hardware_system_settings_card_sub_header);
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    hardwareCardSubHeader.Text = db.Hardwares.Find(id).Name;
                }
            }

            webView = FindViewById<WebView>(Resource.Id.webViewSystemSettings);
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
            webView.LoadUrl($"http://{address}/{password}/");
        }

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