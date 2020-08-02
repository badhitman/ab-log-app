////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Services;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class LogsActivity : AbstractActivity
    {
        public static new readonly string TAG = "● logs-activity";

        protected override int ViewId => Resource.Layout.logs_list_activity;
        protected override int ToolbarId => Resource.Id.logs_toolbar;
        protected override int DrawerLayoutId => Resource.Id.logs_drawer_layout;
        protected override int NavId => Resource.Id.logs_nav_view;

        TextView textView;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        LogListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.logs_list);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            TextView_Click(null, null);

            textView = FindViewById<TextView>(Resource.Id.logs_title);
        }

        private void TextView_Click(object sender, System.EventArgs e)
        {
            mAdapter = new LogListAdapter();

            mRecyclerView.SetAdapter(mAdapter);
            if (sender != null)
            {
                Toast.MakeText(this, "reload logs", ToastLength.Short).Show();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            textView.Click += TextView_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            textView.Click -= TextView_Click;
        }
    }
}

