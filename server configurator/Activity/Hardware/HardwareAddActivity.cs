////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore.Internal;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class HardwareAddActivity : AbstractActivity
    {
        protected override int ViewId => Resource.Layout.hardware_activity;
        protected override int ToolbarId => Resource.Id.hardware_toolbar;
        protected override int DrawerLayoutId => Resource.Id.hardware_drawer_layout;
        protected override int NavId => Resource.Id.hardware_nav_view;

        protected AppCompatTextView HardwareCardHeader;
        protected AppCompatTextView HardwareCardSubHeader;

        protected LinearLayout HardwareTopLayout;

        protected AppCompatEditText HardwareName;
        protected AppCompatEditText HardwareAddress;
        protected AppCompatEditText HardwarePassword;

        protected Switch HardwareAlarmSubscribing;
        protected Switch HardwareCommandsAllowed;

        protected AppCompatButton HardwareCardButtonOk;

        protected LinearLayout HardwareFooterLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HardwareCardHeader = FindViewById<AppCompatTextView>(Resource.Id.hardware_card_title);
            HardwareCardSubHeader = FindViewById<AppCompatTextView>(Resource.Id.hardware_card_subtitle);

            HardwareTopLayout = FindViewById<LinearLayout>(Resource.Id.hardware_top_layout);

            HardwareName = FindViewById<AppCompatEditText>(Resource.Id.hardware_name);
            HardwareAddress = FindViewById<AppCompatEditText>(Resource.Id.hardware_address);
            HardwarePassword = FindViewById<AppCompatEditText>(Resource.Id.hardware_password);

            HardwareAlarmSubscribing = FindViewById<Switch>(Resource.Id.hardware_alarms_switch);
            HardwareCommandsAllowed = FindViewById<Switch>(Resource.Id.hardware_commands_switch);

            HardwareFooterLayout = FindViewById<LinearLayout>(Resource.Id.hardware_footer_layout);

            HardwareCardButtonOk = FindViewById<AppCompatButton>(Resource.Id.hardware_button_ok);
        }

        protected override void OnResume()
        {
            base.OnResume();
            HardwareCardButtonOk.Click += HandlerHardwareButtonOk_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            HardwareCardButtonOk.Click -= HandlerHardwareButtonOk_Click;
        }

        protected string ReadView(int objectId = 0)
        {
            string errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(HardwareName.Text))
            {
                errMsg = GetText(Resource.String.error_empty_name_hardware) + System.Environment.NewLine;
            }
            if (string.IsNullOrWhiteSpace(HardwareAddress.Text))
            {
                errMsg = (errMsg + GetText(Resource.String.error_empty_address_hardware)).Trim() + System.Environment.NewLine;
            }
            if (string.IsNullOrWhiteSpace(HardwarePassword.Text))
            {
                errMsg = (errMsg + GetText(Resource.String.error_empty_password_hardware)).Trim() + System.Environment.NewLine;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (!string.IsNullOrWhiteSpace(HardwareName.Text) && db.Hardwares.Where(x => x.Name.Trim().ToLower() == HardwareName.Text.ToLower() && x.Id != objectId).Any())
                    {
                        errMsg = (errMsg + GetText(Resource.String.error_duplicate_name_hardware)).Trim() + System.Environment.NewLine;
                    }
                    if (!string.IsNullOrWhiteSpace(HardwareAddress.Text) && db.Hardwares.Where(x => x.Address.Trim().ToLower() == HardwareAddress.Text.ToLower() && x.Id != objectId).Any())
                    {
                        errMsg = (errMsg + GetText(Resource.String.error_duplicate_address_hardware)).Trim() + System.Environment.NewLine;
                    }
                }
            }

            return errMsg.Trim();
        }

        protected virtual void HandlerHardwareButtonOk_Click(object sender, EventArgs e)
        {
            string errMsg = ReadView();

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                HardwareCardSubHeader.Text = errMsg;
                HardwareCardSubHeader.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    HardwareModel hardware = new HardwareModel()
                    {
                        AlarmSubscriber = HardwareAlarmSubscribing.Checked,
                        CommandsAllowed = HardwareCommandsAllowed.Checked,
                        Name = HardwareName.Text.Trim(),
                        Address = HardwareAddress.Text.Trim(),
                        Password = HardwarePassword.Text.Trim()
                    };
                    db.Hardwares.Add(hardware);
                    db.SaveChanges();
                }
            }
            StartActivity(typeof(HardwaresListActivity));
        }
    }
}