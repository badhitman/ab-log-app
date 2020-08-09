////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Services;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using Xamarin.Essentials;

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

        ImageButton RefreshLogView;
        ImageButton SettingsLogView;
        LinearLayout SettingsLogViewArea;
        LinearLayoutCompat SettingsLogViewForm;

        AppCompatCheckBox checkBoxTracView;
        AppCompatCheckBox checkBoxInfoView;
        AppCompatCheckBox checkBoxWarnView;
        AppCompatCheckBox checkBoxErrView;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        LogListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.logs_list);
            RefreshLogView = FindViewById<ImageButton>(Resource.Id.RefreshLogView);
            SettingsLogView = FindViewById<ImageButton>(Resource.Id.SettingsLogView);
            SettingsLogViewArea = FindViewById<LinearLayout>(Resource.Id.SettingsLogViewArea);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            if (Preferences.Get(Constants.LOG_SETTINGS_FORM_VISIBLE, false))
            {
                SettingsLogButton_Click(null, null);
            }
            //
            RefreshLogButton_Click(null, null);
        }

        private void SettingsLogButton_Click(object sender, EventArgs e)
        {
            bool LOG_SETTINGS_FORM_VISIBLE = SettingsLogViewForm == null;
            Preferences.Set(Constants.LOG_SETTINGS_FORM_VISIBLE, LOG_SETTINGS_FORM_VISIBLE);
            //
            if (LOG_SETTINGS_FORM_VISIBLE)
            {
                SettingsLogViewForm = LayoutInflater.Inflate(Resource.Layout.FormSettingsLogView, SettingsLogViewArea, false) as LinearLayoutCompat;
                //
                checkBoxTracView = SettingsLogViewForm.FindViewById<AppCompatCheckBox>(Resource.Id.checkBoxTracView);
                checkBoxTracView.Checked = Preferences.Get(Resources.GetResourceEntryName(checkBoxTracView.Id), true);
                checkBoxTracView.CheckedChange += CheckBoxTracView_CheckedChange;

                checkBoxInfoView = SettingsLogViewForm.FindViewById<AppCompatCheckBox>(Resource.Id.checkBoxInfoView);
                checkBoxInfoView.Checked = Preferences.Get(Resources.GetResourceEntryName(checkBoxInfoView.Id), true);
                checkBoxInfoView.CheckedChange += CheckBoxTracView_CheckedChange;

                checkBoxWarnView = SettingsLogViewForm.FindViewById<AppCompatCheckBox>(Resource.Id.checkBoxWarnView);
                checkBoxWarnView.Checked = Preferences.Get(Resources.GetResourceEntryName(checkBoxWarnView.Id), true);
                checkBoxWarnView.CheckedChange += CheckBoxTracView_CheckedChange;

                checkBoxErrView = SettingsLogViewForm.FindViewById<AppCompatCheckBox>(Resource.Id.checkBoxErrView);
                checkBoxErrView.Checked = Preferences.Get(Resources.GetResourceEntryName(checkBoxErrView.Id), true);
                checkBoxErrView.CheckedChange += CheckBoxTracView_CheckedChange;
                //
                SettingsLogViewArea.AddView(SettingsLogViewForm);
            }
            else
            {
                checkBoxTracView.CheckedChange -= CheckBoxTracView_CheckedChange;
                checkBoxInfoView.CheckedChange -= CheckBoxTracView_CheckedChange;
                checkBoxWarnView.CheckedChange -= CheckBoxTracView_CheckedChange;
                checkBoxErrView.CheckedChange -= CheckBoxTracView_CheckedChange;
                //
                checkBoxTracView = null;
                checkBoxInfoView = null;
                checkBoxWarnView = null;
                checkBoxErrView = null;
                //
                SettingsLogViewArea.RemoveAllViews();
                SettingsLogViewForm = null;
            }
        }

        private void CheckBoxTracView_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            AppCompatCheckBox checkBox = sender as AppCompatCheckBox;
            Preferences.Set(Resources.GetResourceEntryName(checkBox.Id), e.IsChecked);
            RefreshLogButton_Click(null, null);
        }

        protected override void OnResume()
        {
            base.OnResume();
            SettingsLogView.Click += SettingsLogButton_Click;
            RefreshLogView.Click += RefreshLogButton_Click;
            if (SettingsLogViewForm != null)
            {
                checkBoxTracView.CheckedChange += CheckBoxTracView_CheckedChange;
                checkBoxInfoView.CheckedChange += CheckBoxTracView_CheckedChange;
                checkBoxWarnView.CheckedChange += CheckBoxTracView_CheckedChange;
                checkBoxErrView.CheckedChange += CheckBoxTracView_CheckedChange;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            SettingsLogView.Click -= SettingsLogButton_Click;
            RefreshLogView.Click -= RefreshLogButton_Click;

            if (SettingsLogViewForm != null)
            {
                SettingsLogViewForm = LayoutInflater.Inflate(Resource.Layout.FormSettingsLogView, SettingsLogViewArea, false) as LinearLayoutCompat;
                //
                checkBoxTracView.CheckedChange -= CheckBoxTracView_CheckedChange;
                checkBoxInfoView.CheckedChange -= CheckBoxTracView_CheckedChange;
                checkBoxWarnView.CheckedChange -= CheckBoxTracView_CheckedChange;
                checkBoxErrView.CheckedChange -= CheckBoxTracView_CheckedChange;
                //
                SettingsLogViewArea.AddView(SettingsLogViewForm);
            }
        }

        private void RefreshLogButton_Click(object sender, System.EventArgs e)
        {
            mAdapter = new LogListAdapter(checkBoxTracView?.Checked ?? Preferences.Get(Resources.GetResourceEntryName(Resource.Id.checkBoxTracView), true), checkBoxInfoView?.Checked ?? Preferences.Get(Resources.GetResourceEntryName(Resource.Id.checkBoxInfoView), true), checkBoxWarnView?.Checked ?? Preferences.Get(Resources.GetResourceEntryName(Resource.Id.checkBoxWarnView), true), checkBoxErrView?.Checked ?? Preferences.Get(Resources.GetResourceEntryName(Resource.Id.checkBoxErrView), true));

            mRecyclerView.SetAdapter(mAdapter);
            if (sender != null)
            {
                Toast.MakeText(this, "reload logs", ToastLength.Short).Show();
            }
        }
    }
}

