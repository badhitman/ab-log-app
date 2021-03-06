﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class UserEditActivity : UserCreateActivity
    {
        public static new readonly string TAG = "● user-edit-activity";

        protected override int ViewId => Resource.Layout.user_activity;
        protected int userId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");

            base.OnCreate(savedInstanceState);
            UserFooterLayout = FindViewById<LinearLayout>(Resource.Id.user_footer_layout);
            int user_id = Intent.Extras.GetInt(nameof(UserModel.Id), 0);

            if (user_id < 1)
            {
                string err_title = GetText(Resource.String.err_title_2);
                UserName.Text = err_title;
                UserName.Enabled = false;

                UserEmail.Text = err_title;
                UserEmail.Enabled = false;

                UserPhone.Text = err_title;
                UserPhone.Enabled = false;

                UserAlarmSubscribing.Enabled = false;
                UserCommandsAllowed.Enabled = false;

                UserCardSubHeader.Text = err_title;
                UserCardSubHeader.Enabled = false;

                UserCardButtonOk.Enabled = false;
                return;
            }

            UserModel user = null;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {

                    user = db.Users.Find(user_id);
                }
            }
            userId = user.Id;
            UserName.Text = user.Name;
            UserEmail.Text = user.Email;
            UserPhone.Text = user.Phone;
            UserAlarmSubscribing.Checked = user.AlarmSubscriber;
            UserCommandsAllowed.Checked = user.CommandsAllowed;

            TelegramUserModel telegramUser = null;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {

                    telegramUser = db.TelegramUsers.FirstOrDefault(x => x.LinkedUserId == user.Id);
                }
            }
            if (telegramUser != null)
            {
                TelegramAccount.SetSelection(TelegramUsers.Keys.ToList().IndexOf(telegramUser.Id));
            }

            UserCardHeader.Text = GetText(Resource.String.edit_user_title);
            UserCardSubHeader.Text = GetText(Resource.String.edit_user_sub_title);

            AppCompatButton buttonDeleteUser = new AppCompatButton(this) { Text = GetText(Resource.String.delete_title) };
            buttonDeleteUser.SetTextColor(Color.DarkRed);
            buttonDeleteUser.Click += ButtonDeleteUser_Click;
            buttonDeleteUser.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            UserFooterLayout.AddView(buttonDeleteUser);
        }

        private void ButtonDeleteUser_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "ButtonDeleteUser_Click");

            AppCompatButton buttonDeleteUser = sender as AppCompatButton;
            buttonDeleteUser.Enabled = false;
            UserCardButtonOk.Enabled = false;
            UserCardButtonOk.Text = GetText(Resource.String.ok_mute_button_with_remove_user);
            UserName.Enabled = false;
            UserEmail.Enabled = false;
            UserPhone.Enabled = false;
            UserAlarmSubscribing.Enabled = false;
            UserCommandsAllowed.Enabled = false;
            UserCardSubHeader.Text = GetText(Resource.String.delete_user_card_sub_title);
            UserCardHeader.Text = GetText(Resource.String.delete_user_card_header);
            UserCardSubHeader.SetTextColor(Color.IndianRed);
            buttonDeleteUser.SetTextColor(Color.Gray);

            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = GetText(Resource.String.footer_text_with_remove_user), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            UserFooterLayout.AddView(appCompatTextView);

            AppCompatButton buttonConfirmDeleteUser = new AppCompatButton(this) { Text = GetText(Resource.String.button_confirm_remove) };
            buttonConfirmDeleteUser.SetTextColor(Color.DarkRed);
            buttonConfirmDeleteUser.Click += new EventHandler((sender, eventArg) =>
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        UserModel user = db.Users.Find(userId);
                        db.Users.Remove(user);
                        db.SaveChanges();

                        StartActivity(typeof(UsersListActivity));
                    }
                }
            });
            buttonConfirmDeleteUser.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            UserFooterLayout.AddView(buttonConfirmDeleteUser);
        }

        protected override void HandlerUserButtonOk_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "HandlerUserButtonOk_Click");

            string errMsg = ReadView(userId);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                UserCardSubHeader.Text = errMsg;
                UserCardSubHeader.SetTextColor(Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            int selected_telegram_id = TelegramUsers.Keys.ToList()[TelegramAccount.SelectedItemPosition];
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = db.Users.Find(userId);

                    user.Name = UserName.Text.Trim();
                    user.Email = UserEmail.Text.Trim();
                    user.Phone = UserPhone.Text.Trim();

                    user.AlarmSubscriber = UserAlarmSubscribing.Checked;
                    user.CommandsAllowed = UserCommandsAllowed.Checked;

                    db.Users.Update(user);
                    if (selected_telegram_id > 0)
                    {
                        TelegramUserModel telegramUser = db.TelegramUsers.Find(selected_telegram_id);
                        telegramUser.LinkedUserId = user.Id;
                        List<TelegramUserModel> other_telegram_users = db.TelegramUsers.Where(x => x.LinkedUserId == user.Id && x.Id == selected_telegram_id).ToList();
                        if (other_telegram_users != null && other_telegram_users.Count > 0)
                        {
                            other_telegram_users.ForEach(x => { x.LinkedUserId = null; });
                        }
                    }
                    else
                    {
                        TelegramUserModel telegramUser = db.TelegramUsers.FirstOrDefault(x => x.LinkedUserId == userId);
                        if (telegramUser != null)
                        {
                            telegramUser.LinkedUserId = null;
                        }
                    }
                    db.SaveChanges();
                }
            }
            StartActivity(typeof(UsersListActivity));
        }
    }
}