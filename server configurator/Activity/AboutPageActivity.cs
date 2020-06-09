////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class AboutPageActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.about_activity;
        protected override int ToolbarId => Resource.Id.about_toolbar;
        protected override int DrawerLayoutId => Resource.Id.about_app_drawer_layout;
        protected override int NavId => Resource.Id.about_app_nav_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}

