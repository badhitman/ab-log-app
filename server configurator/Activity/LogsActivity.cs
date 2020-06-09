////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class LogsActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.logs_activity;
        protected override int ToolbarId => Resource.Id.logs_toolbar;
        protected override int DrawerLayoutId => Resource.Id.logs_drawer_layout;
        protected override int NavId => Resource.Id.logs_nav_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}

