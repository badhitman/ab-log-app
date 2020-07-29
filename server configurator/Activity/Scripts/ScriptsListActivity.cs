////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.RecyclerView.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class ScriptsListActivity : AbstractListActivity
    {
        protected override int ViewId => Resource.Layout.scripts_list_activity;
        protected override int ToolbarId => Resource.Id.scripts_list_toolbar;
        protected override int DrawerLayoutId => Resource.Id.scripts_list_drawer_layout;
        protected override int ButtonAdd => Resource.Id.scripts_list_add_button;
        protected override int NavId => Resource.Id.scripts_list_nav_view;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        ScriptsListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.scripts_list);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new ScriptsListAdapter(this);

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
            StartActivity(typeof(ScriptAddActivity));
        }

        void OnItemClick(object sender, int scriptId)
        {
            Intent intent = new Intent(this, typeof(ScriptEditActivity));
            intent.PutExtra(nameof(ScriptHardwareModel.Id), scriptId);
            StartActivity(intent);
        }
    }
}

