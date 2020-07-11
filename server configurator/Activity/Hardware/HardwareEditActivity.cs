﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using System;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Android.Content;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class HardwareEditActivity : HardwareAddActivity
    {
        protected int hardwareId;
        AppCompatButton buttonDeleteHardware;
        //AppCompatButton ButtonConfigPortsHardware;
        AppCompatButton SystemSettingsHardware;
        HardwareModel hardware;

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
                        string err_title = GetText(Resource.String.err_title_2);
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

                    hardware = db.Hardwares.OrderBy(x => x.Id).Skip(gs.SelectedListPosition).FirstOrDefault();
                    hardwareId = hardware?.Id ?? 0;
                    HardwareName.Text = hardware?.Name;
                    HardwareAddress.Text = hardware?.Address;
                    HardwarePassword.Text = hardware?.Password;

                    HardwareAlarmSubscribing.Checked = hardware.AlarmSubscriber;
                    HardwareCommandsAllowed.Checked = hardware.CommandsAllowed;
                }
            }

            HardwareCardHeader.Text = GetText(Resource.String.edit_hardware_title);
            HardwareCardSubHeader.Text = GetText(Resource.String.edit_hardware_sub_title);

            buttonDeleteHardware = new AppCompatButton(this) { Text = GetText(Resource.String.delete_title) };
            buttonDeleteHardware.SetTextColor(Color.DarkRed);
            buttonDeleteHardware.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            HardwareFooterLayout.AddView(buttonDeleteHardware);

            SystemSettingsHardware = new AppCompatButton(this) { Text = GetText(Resource.String.system_settings_title) };
            SystemSettingsHardware.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            HardwareTopLayout.AddView(SystemSettingsHardware);

            //ButtonConfigPortsHardware = new AppCompatButton(this) { Text = GetText(Resource.String.port_setting_title) };
            //ButtonConfigPortsHardware.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            //HardwareFooterLayout.AddView(ButtonConfigPortsHardware);
        }

        private void SystemSettingsHardware_Click(object sender, EventArgs e)
        {
            if(hardware.Address != HardwareAddress.Text || 
                hardware.Password != HardwarePassword.Text || 
                hardware.Name != HardwareName.Text ||
                hardware.AlarmSubscriber != HardwareAlarmSubscribing.Checked ||
                hardware.CommandsAllowed != HardwareCommandsAllowed.Checked)
            {
                Toast.MakeText(this, Resource.String.saving_is_required, ToastLength.Short).Show();
                return;
            }

            Intent intent = new Intent(this, typeof(HardwareSystemSettingsActivity));
            intent.PutExtra(nameof(hardware.Id), hardware.Id);
            intent.PutExtra(nameof(hardware.Address), hardware.Address);
            intent.PutExtra(nameof(hardware.Password), hardware.Password);
            StartActivity(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            buttonDeleteHardware.Click += ButtonDeleteHardware_Click;
            //ButtonConfigPortsHardware.Click += ButtonConfigPortsHardware_Click;
            SystemSettingsHardware.Click += SystemSettingsHardware_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            buttonDeleteHardware.Click -= ButtonDeleteHardware_Click;
            //ButtonConfigPortsHardware.Click -= ButtonConfigPortsHardware_Click;
            SystemSettingsHardware.Click -= SystemSettingsHardware_Click;
        }

        private void ButtonDeleteHardware_Click(object sender, EventArgs e)
        {
            HardwareCardHeader.Text = GetText(Resource.String.delete_hardware_card_title);

            HardwareCardSubHeader.Text = GetText(Resource.String.delete_hardware_card_sub_title);
            HardwareCardSubHeader.SetTextColor(Color.IndianRed);

            HardwareName.Enabled = false;
            HardwareAddress.Enabled = false;
            HardwarePassword.Enabled = false;

            HardwareAlarmSubscribing.Enabled = false;
            HardwareCommandsAllowed.Enabled = false;

            HardwareCardButtonOk.Enabled = false;
            HardwareCardButtonOk.Text = GetText(Resource.String.ok_mute_button_with_remove_hardware);

            buttonDeleteHardware.Enabled = false;
            buttonDeleteHardware.SetTextColor(Color.Gray);
            buttonDeleteHardware.Click -= ButtonDeleteHardware_Click;

            SystemSettingsHardware.Enabled = false;
            SystemSettingsHardware.SetTextColor(Color.Gray);
            SystemSettingsHardware.Click -= SystemSettingsHardware_Click;

            //ButtonConfigPortsHardware.Enabled = false;
            //ButtonConfigPortsHardware.SetTextColor(Color.Gray);
            //ButtonConfigPortsHardware.Click -= ButtonConfigPortsHardware_Click;

            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = GetText(Resource.String.footer_text_with_remove_hardware), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            HardwareFooterLayout.AddView(appCompatTextView);

            AppCompatButton ButtonConfirmDeleteHardware = new AppCompatButton(this) { Text = GetText(Resource.String.button_confirm_remove) };
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
                    hardware.Name = HardwareName.Text.Trim();
                    hardware.Address = HardwareAddress.Text.Trim();
                    hardware.Password = HardwarePassword.Text.Trim();

                    hardware.AlarmSubscriber = HardwareAlarmSubscribing.Checked;
                    hardware.CommandsAllowed = HardwareCommandsAllowed.Checked;

                    db.Hardwares.Update(hardware);
                    db.SaveChanges();
                }
            }
            StartActivity(typeof(HardwaresListActivity));
        }
    }
}