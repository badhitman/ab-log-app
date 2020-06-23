////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class HardwareEditActivity : HardwareAddActivity
    {
        protected int hardwareId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ReadView();

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (gs.SelectedListPosition < 0 || db.Hardwares.Count() < gs.SelectedListPosition + 1)
                    {
                        string err_title = Resources.GetString(Resource.String.err_title_2);
                        HardwareName.Text = err_title;
                        HardwareName.Enabled = false;

                        HardwareAddress.Text = err_title;
                        HardwareAddress.Enabled = false;

                        HardwarePassword.Text = err_title;
                        HardwarePassword.Enabled = false;

                        HardwareAlarmSubscribing.Enabled = false;
                        HardwareCommandsAllowed.Enabled = false;

                        HardwareCardSubHeader.Text = err_title;
                        HardwareCardSubHeader.Enabled = false;

                        HardwareCardButtonOk.Enabled = false;
                        return;
                    }

                    HardwareModel hardware = db.Hardwares.OrderBy(x => x.Id).Skip(gs.SelectedListPosition).FirstOrDefault();
                    hardwareId = hardware?.Id ?? 0;
                    HardwareName.Text = hardware?.Name;
                    HardwareAddress.Text = hardware?.Address;
                    HardwarePassword.Text = hardware?.Password;

                    HardwareAlarmSubscribing.Checked = hardware.AlarmSubscriber;
                    HardwareCommandsAllowed.Checked = hardware.CommandsAllowed;
                }
            }

            HardwareCardHeader.Text = Resources.GetString(Resource.String.edit_hardware_title);
            HardwareCardSubHeader.Text = Resources.GetString(Resource.String.edit_hardware_sub_title);

            AppCompatButton buttonDeleteHardware = new AppCompatButton(this) { Text = Resources.GetString(Resource.String.delete_title) };
            buttonDeleteHardware.SetTextColor(Color.DarkRed);
            buttonDeleteHardware.Click += ButtonDeleteHardware_Click;
            buttonDeleteHardware.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            HardwareFooterLayout.AddView(buttonDeleteHardware);
        }

        private void ButtonDeleteHardware_Click(object sender, EventArgs e)
        {
            HardwareCardHeader.Text = Resources.GetString(Resource.String.delete_hardware_card_title);

            HardwareCardSubHeader.Text = Resources.GetString(Resource.String.delete_hardware_card_sub_title);
            HardwareCardSubHeader.SetTextColor(Color.IndianRed);

            HardwareName.Enabled = false;
            HardwareAddress.Enabled = false;
            HardwarePassword.Enabled = false;

            HardwareAlarmSubscribing.Enabled = false;
            HardwareCommandsAllowed.Enabled = false;

            HardwareCardButtonOk.Enabled = false;
            HardwareCardButtonOk.Text = Resources.GetString(Resource.String.ok_mute_button_with_remove_hardware);

            AppCompatButton ButtonDeletingHardware = sender as AppCompatButton;
            ButtonDeletingHardware.Enabled = false;
            ButtonDeletingHardware.SetTextColor(Color.Gray);

            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = Resources.GetString(Resource.String.footer_text_with_remove_hardware), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            HardwareFooterLayout.AddView(appCompatTextView);

            AppCompatButton ButtonConfirmDeleteHardware = new AppCompatButton(this) { Text = Resources.GetString(Resource.String.button_confirm_remove) };
            ButtonConfirmDeleteHardware.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ButtonConfirmDeleteHardware.SetTextColor(Color.DarkRed);
            ButtonConfirmDeleteHardware.Click += new EventHandler((sender, eventArg) =>
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        HardwareModel hardware = db.Hardwares.Find(hardwareId);
                        db.Hardwares.Remove(hardware);
                        db.SaveChanges();

                        StartActivity(typeof(HardwaresListActivity));
                    }
                }
            });
            HardwareFooterLayout.AddView(ButtonConfirmDeleteHardware);
        }

        protected override void HandlerHardwareButtonOk_Click(object sender, EventArgs e)
        {
            string errMsg = ReadView(hardwareId);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                HardwareCardSubHeader.Text = errMsg;
                HardwareCardSubHeader.SetTextColor(Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    HardwareModel hw = db.Hardwares.Find(hardwareId);

                    hw.Name = HardwareName.Text.Trim();
                    hw.Address = HardwareAddress.Text.Trim();
                    hw.Password = HardwarePassword.Text.Trim();

                    hw.AlarmSubscriber = HardwareAlarmSubscribing.Checked;
                    hw.CommandsAllowed = HardwareCommandsAllowed.Checked;

                    db.Hardwares.Update(hw);
                    db.SaveChanges();
                }
            }
            StartActivity(typeof(HardwaresListActivity));
        }
    }
}