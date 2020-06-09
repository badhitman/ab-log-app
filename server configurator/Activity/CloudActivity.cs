////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class CloudActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.cloud_activity;
        protected override int ToolbarId => Resource.Id.cloud_toolbar;
        protected override int DrawerLayoutId => Resource.Id.cloud_drawer_layout;
        protected override int NavId => Resource.Id.cloud_nav_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}