﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;
using Android.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class PortsConfigActivity : AbstractActivity
    {
        public static new readonly string TAG = "● ports-config-activity";

        protected override int ViewId => Resource.Layout.ports_config_activity;
        protected override int ToolbarId => Resource.Id.ports_config_toolbar;
        protected override int DrawerLayoutId => Resource.Id.ports_config_drawer_layout;
        protected override int NavId => Resource.Id.ports_config_nav_view;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            TabHost tabHost = (TabHost)FindViewById(Resource.Id.tabHost);

            tabHost.Setup();

            TabHost.TabSpec tabSpec = tabHost.NewTabSpec("xp1");
            tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xp1);
            tabSpec.SetIndicator("XP1");
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("xp2");
            tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xp2);
            tabSpec.SetIndicator("XP2");
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("xt2");
            tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xt2);
            tabSpec.SetIndicator("XT2");
            tabHost.AddTab(tabSpec);

            tabSpec = tabHost.NewTabSpec("xp56");
            tabSpec.SetContent(Resource.Id.linear_layout_ports_config_xp56);
            tabSpec.SetIndicator("XP5/6");
            tabHost.AddTab(tabSpec);

            tabHost.CurrentTab = 0;
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