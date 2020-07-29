////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.RecyclerView.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class HardwaresListActivity : AbstractListActivity
    {
        public new readonly string TAG = "hardwares-list-activity";

        protected override int ViewId => Resource.Layout.hardwares_list_activity;
        protected override int ToolbarId => Resource.Id.hardwares_list_toolbar;
        protected override int DrawerLayoutId => Resource.Id.hardwares_list_drawer_layout;
        protected override int ButtonAdd => Resource.Id.hardwares_list_add_button;
        protected override int NavId => Resource.Id.hardwares_list_nav_view;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        HardwaresListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");

            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.hardwares_list);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new HardwaresListAdapter(this);

            mRecyclerView.SetAdapter(mAdapter);
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");

            base.OnResume();
            mAdapter.ItemClick += OnItemClick;
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");

            base.OnPause();
            mAdapter.ItemClick -= OnItemClick;
        }

        protected override void ButtonAddOnClick(object sender, EventArgs eventArgs)
        {
            Log.Debug(TAG, "ButtonAddOnClick");

            StartActivity(typeof(HardwareAddActivity));
        }

        void OnItemClick(object sender, int hardware_id)
        {
            Log.Debug(TAG, $"OnItemClick - hardware_id:{hardware_id}");

            Intent intent = new Intent(this, typeof(HardwareEditActivity));
            intent.PutExtra(nameof(HardwareModel.Id), hardware_id);
            StartActivity(intent);
        }
    }
}