////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

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
            Spinner spinner = FindViewById<Spinner>(Resource.Id.second_cloud_mode_spinner);

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Resources.GetStringArray(Resource.Array.cloud_modes));
            
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            //string toast = string.Format("The mode is {0}", spinner.GetItemAtPosition(e.Position));
            //Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
    }
}