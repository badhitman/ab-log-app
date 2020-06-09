////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class ServicesActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.services_activity;
        protected override int ToolbarId => Resource.Id.services_toolbar;
        protected override int DrawerLayoutId => Resource.Id.services_drawer_layout;
        protected override int NavId => Resource.Id.services_nav_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}