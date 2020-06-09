////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.View.Menu;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.Internal;
using Google.Android.Material.Navigation;

namespace ab
{
    public abstract class aActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private static int selectedMenuId = 0;
        //
        protected abstract int NavId { get; }
        protected abstract int ViewId { get; }
        protected abstract int ToolbarId { get; }
        protected abstract int DrawerLayoutId { get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(ViewId);

            Toolbar toolbar = FindViewById<Toolbar>(ToolbarId);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(DrawerLayoutId);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(NavId);
            switch (selectedMenuId)
            {
                case Resource.Id.nav_hardware:
                    ((navigationView.Menu as NavigationMenu).GetItem(0) as MenuItemImpl).SetChecked(true);
                    break;
                case Resource.Id.nav_users:
                    ((navigationView.Menu as NavigationMenu).GetItem(1) as MenuItemImpl).SetChecked(true);
                    break;
                case Resource.Id.nav_cloud_queue:
                    ((navigationView.Menu as NavigationMenu).GetItem(2) as MenuItemImpl).SetChecked(true);
                    break;
                case Resource.Id.nav_smart_button:
                    ((navigationView.Menu as NavigationMenu).GetItem(3) as MenuItemImpl).SetChecked(true);
                    break;
                default:
                    break;
            }

            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gs.SelectedListPosition = 0;
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(DrawerLayoutId);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = selectedMenuId = item.ItemId;
            if (id == Resource.Id.action_logs)
            {
                StartActivity(typeof(LogsActivity));
                return true;
            }
            else if (id == Resource.Id.action_about)
            {
                StartActivity(typeof(AboutPageActivity));
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = selectedMenuId = item.ItemId;

            if (id == Resource.Id.nav_hardware)
            {
                StartActivity(typeof(HardwaresListActivity));
            }
            else if (id == Resource.Id.nav_users)
            {
                StartActivity(typeof(UsersActivity));
            }
            else if (id == Resource.Id.nav_cloud_queue)
            {
                StartActivity(typeof(CloudActivity));
            }
            else if (id == Resource.Id.nav_smart_button)
            {
                StartActivity(typeof(ServicesActivity));
            }
            else if (id == Resource.Id.nav_goto_official_site)
            {
                Xamarin.Essentials.Browser.OpenAsync("https://ab-log.ru/?ref=https://github.com/badhitman/ab-log-app");
            }
            else if (id == Resource.Id.nav_developer_board)
            {
                Xamarin.Essentials.Browser.OpenAsync("https://github.com/badhitman/ab-log-app");
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(DrawerLayoutId);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

