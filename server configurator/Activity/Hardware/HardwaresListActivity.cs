////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using ab.Services;
using Android.App;
using Android.OS;
using AndroidX.RecyclerView.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class HardwaresListActivity : aListActivity
    {
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
            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.hardwares_list);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new HardwaresListAdapter(this);

            mRecyclerView.SetAdapter(mAdapter);           
        }

        protected override void OnResume()
        {
            base.OnResume();
            mAdapter.ItemClick += OnItemClick;
        }

        protected override void OnPause()
        {
            base.OnPause();
            mAdapter.ItemClick -= OnItemClick;
        }

        protected override void ButtonAddOnClick(object sender, EventArgs eventArgs)
        {
            StartActivity(typeof(HardwareAddActivity));
        }

        void OnItemClick(object sender, int position)
        {
            gs.SelectedListPosition = position;
            StartActivity(typeof(HardwareEditActivity));
        }
    }
}

