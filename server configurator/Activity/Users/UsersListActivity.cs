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
    public class UsersListActivity : AbstractListActivity
    {
        public static new readonly string TAG = "● users-list-activity";

        protected override int ViewId => Resource.Layout.users_list_activity;
        protected override int ToolbarId => Resource.Id.users_list_toolbar;
        protected override int DrawerLayoutId => Resource.Id.users_list_drawer_layout;
        protected override int ButtonAdd => Resource.Id.users_list_add_button;
        protected override int NavId => Resource.Id.users_list_nav_view;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        UsersListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");

            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.users_list);
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");

            base.OnResume();
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);
            mAdapter = new UsersListAdapter(this);
            mRecyclerView.SetAdapter(mAdapter);

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

            StartActivity(typeof(UserCreateActivity));
        }

        void OnItemClick(object sender, int user_id)
        {
            Log.Debug(TAG, $"OnItemClick - user_id:{user_id}");

            Intent intent = new Intent(this, typeof(UserEditActivity));
            intent.PutExtra(nameof(UserModel.Id), user_id);
            StartActivity(intent);
        }
    }
}

