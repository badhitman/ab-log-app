////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class PortsConfigActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.ports_config_activity;
        protected override int ToolbarId => Resource.Id.ports_config_toolbar;
        protected override int DrawerLayoutId => Resource.Id.ports_config_drawer_layout;
        protected override int NavId => Resource.Id.ports_config_nav_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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