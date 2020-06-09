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
    public class UsersActivity : aListActivity
    {
        protected override int ViewId => Resource.Layout.users_list_activity;
        protected override int ToolbarId => Resource.Id.users_list_toolbar;
        protected override int DrawerLayoutId => Resource.Id.users_list_drawer_layout;
        protected override int ButtonAdd => Resource.Id.users_list_add_button;
        protected override int NavId => Resource.Id.users_list_nav_view;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        UserListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.users_list);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new UserListAdapter();

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
            StartActivity(typeof(UserCreateActivity));
        }

        void OnItemClick(object sender, int position)
        {
            gs.SelectedListPosition = position;
            StartActivity(typeof(UserEditActivity));
        }
    }
}

