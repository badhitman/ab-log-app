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
    public class HardwareAddActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.hardware_activity;
        protected override int ToolbarId => Resource.Id.hardware_toolbar;
        protected override int DrawerLayoutId => Resource.Id.hardware_drawer_layout;
        protected override int NavId => Resource.Id.hardware_nav_view;

        protected AppCompatTextView HardwareCardHeader;
        protected AppCompatTextView HardwareCardSubHeader;

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

        protected string ReadView(int userId = 0)
        {
            string errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(HardwareName.Text))
            {
                errMsg = "Error empty 'Name' hardware\n";
            }
            if (string.IsNullOrWhiteSpace(HardwareAddress.Text))
            {
                errMsg = $"{(errMsg + "Error empty 'Address' hardware").Trim()}\n";
            }
            if (string.IsNullOrWhiteSpace(HardwarePassword.Text))
            {
                errMsg = $"{(errMsg + "Error empty 'Password' hardware").Trim()}\n";
            }

            lock (gs.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (!string.IsNullOrWhiteSpace(HardwareName.Text) && db.Hardwares.Where(x => x.Name.Trim().ToLower() == HardwareName.Text.ToLower() && x.Id != userId).Any())
                    {
                        errMsg = $"{(errMsg + "Error duplicate 'Name' hardware").Trim()}\n";
                    }
                    if (!string.IsNullOrWhiteSpace(HardwareAddress.Text) && db.Hardwares.Where(x => x.Address.Trim().ToLower() == HardwareAddress.Text.ToLower() && x.Id != userId).Any())
                    {
                        errMsg = $"{(errMsg + "Error duplicate 'Address' hardware").Trim()}\n";
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

            lock (gs.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    db.Hardwares.Add(new HardwareModel()
                    {
                        AlarmSubscriber = HardwareAlarmSubscribing.Checked,
                        CommandsAllowed = HardwareCommandsAllowed.Checked,
                        Name = HardwareName.Text.Trim(),
                        Address = HardwareAddress.Text.Trim(),
                        Password = HardwarePassword.Text.Trim()
                    });

                    db.SaveChanges();
                }
            }
            StartActivity(typeof(HardwaresListActivity));
        }
    }
}