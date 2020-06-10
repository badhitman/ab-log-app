﻿////////////////////////////////////////////////
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
    public class UserCreateActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.user_activity;
        protected override int ToolbarId => Resource.Id.user_toolbar;
        protected override int DrawerLayoutId => Resource.Id.user_drawer_layout;
        protected override int NavId => Resource.Id.user_nav_view;

        protected AppCompatTextView UserCardHeader;
        protected AppCompatTextView UserCardSubHeader;

        protected AppCompatEditText UserName;
        protected AppCompatEditText UserEmail;
        protected AppCompatEditText UserPhone;
        protected AppCompatEditText UserTelegram;

        protected Switch UserAlarmSubscribing;
        protected Switch UserCommandsAllowed;

        protected AppCompatButton UserCardButtonOk;

        protected LinearLayout UserFooterLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            UserCardHeader = FindViewById<AppCompatTextView>(Resource.Id.user_card_header);
            UserCardSubHeader = FindViewById<AppCompatTextView>(Resource.Id.user_cart_subtile);

            UserName = FindViewById<AppCompatEditText>(Resource.Id.user_name);
            UserEmail = FindViewById<AppCompatEditText>(Resource.Id.user_email);
            UserPhone = FindViewById<AppCompatEditText>(Resource.Id.user_phone);
            UserTelegram = FindViewById<AppCompatEditText>(Resource.Id.user_telegram);

            UserAlarmSubscribing = FindViewById<Switch>(Resource.Id.user_alarms_switch);
            UserCommandsAllowed = FindViewById<Switch>(Resource.Id.user_commands_switch);

            UserFooterLayout = FindViewById<LinearLayout>(Resource.Id.user_footer_layout);

            UserCardButtonOk = FindViewById<AppCompatButton>(Resource.Id.user_button_ok);
        }

        protected override void OnResume()
        {
            base.OnResume();
            UserCardButtonOk.Click += HandlerUserButtonOk_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            UserCardButtonOk.Click -= HandlerUserButtonOk_Click;
        }

        protected string ReadView(int userId = 0)
        {
            string errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(UserName.Text))
            {
                errMsg = "Error empty 'Name' user\n";
            }
            if (string.IsNullOrWhiteSpace(UserEmail.Text) && string.IsNullOrWhiteSpace(UserPhone.Text))
            {
                errMsg = $"{(errMsg + "Error empty 'Contacts' for user").Trim()}\n";
            }

            lock (gs.DbLocker)
            {
                //using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                //{
                if (!string.IsNullOrWhiteSpace(UserName.Text) && DatabaseContext.UsersCached.Where(x => x.Name.Trim().ToLower() == UserName.Text.ToLower() && x.Id != userId).Any())
                {
                    errMsg = $"{(errMsg + "Error duplicate 'Name' user").Trim()}\n";
                }
                if (!string.IsNullOrWhiteSpace(UserEmail.Text) && DatabaseContext.UsersCached.Where(x => x.Email.Trim().ToLower() == UserEmail.Text.ToLower() && x.Id != userId).Any())
                {
                    errMsg = $"{(errMsg + "Error duplicate 'Email' user").Trim()}\n";
                }
                if (!string.IsNullOrWhiteSpace(UserPhone.Text) && DatabaseContext.UsersCached.Where(x => x.Phone.Trim().ToLower() == UserPhone.Text.ToLower() && x.Id != userId).Any())
                {
                    errMsg = $"{(errMsg + "Error duplicate 'Phone' user").Trim()}\n";
                }
                if (!string.IsNullOrWhiteSpace(UserTelegram.Text) && DatabaseContext.UsersCached.Where(x => x.TelegramId.Trim().ToLower() == UserTelegram.Text.ToLower() && x.Id != userId).Any())
                {
                    errMsg = $"{(errMsg + "Error duplicate 'Telegram Id' user").Trim()}\n";
                }
                //}
            }

            return errMsg.Trim();
        }

        protected virtual void HandlerUserButtonOk_Click(object sender, EventArgs e)
        {
            string errMsg = ReadView();

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                UserCardSubHeader.Text = errMsg;
                UserCardSubHeader.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            lock (gs.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = new UserModel()
                    {
                        AlarmSubscriber = UserAlarmSubscribing.Checked,
                        CommandsAllowed = UserCommandsAllowed.Checked,
                        Name = UserName.Text.Trim(),
                        Email = UserEmail.Text.Trim(),
                        Phone = UserPhone.Text.Trim(),
                        TelegramId = UserTelegram.Text.Trim()
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    DatabaseContext.UsersCached.Add(user);
                }
            }
            StartActivity(typeof(UsersActivity));
        }
    }
}

