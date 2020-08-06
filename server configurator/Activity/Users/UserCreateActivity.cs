////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
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
    public class UserCreateActivity : AbstractActivity
    {
        public static new readonly string TAG = "● user-create-activity";

        public Dictionary<int, string> TelegramUsers;

        protected override int ViewId => Resource.Layout.user_activity;
        protected override int ToolbarId => Resource.Id.user_toolbar;
        protected override int DrawerLayoutId => Resource.Id.user_drawer_layout;
        protected override int NavId => Resource.Id.user_nav_view;

        protected AppCompatTextView UserCardHeader;
        protected AppCompatTextView UserCardSubHeader;

        protected AppCompatEditText UserName;
        protected AppCompatEditText UserEmail;
        protected AppCompatEditText UserPhone;
        protected Spinner TelegramAccount;

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
            TelegramAccount = FindViewById<Spinner>(Resource.Id.spinnerJoinedTelegramAccount);

            UserAlarmSubscribing = FindViewById<Switch>(Resource.Id.user_alarms_switch);
            UserCommandsAllowed = FindViewById<Switch>(Resource.Id.user_commands_switch);

            UserFooterLayout = FindViewById<LinearLayout>(Resource.Id.user_footer_layout);

            UserCardButtonOk = FindViewById<AppCompatButton>(Resource.Id.user_button_ok);

            int user_id = Intent.Extras.GetInt(nameof(UserModel.Id), 0);
            List<TelegramUserModel> telegram_users = new List<TelegramUserModel>();
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    telegram_users = db.TelegramUsers.Where(x => x.LinkedUserId == null || x.LinkedUserId == user_id).ToList();
                }
            }

            TelegramUsers = new Dictionary<int, string>() { { 0, "" } };
            if (telegram_users != null && telegram_users.Count > 0)
            {
                telegram_users.ForEach(x => { TelegramUsers.Add(x.Id, x.ToString()); });
            }

            ArrayAdapter<string> adapterUsers = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, TelegramUsers.Values.ToList());
            adapterUsers.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            TelegramAccount.Adapter = adapterUsers;
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
                errMsg = GetText(Resource.String.error_empty_name_user) + System.Environment.NewLine;
            }
            if (string.IsNullOrWhiteSpace(UserEmail.Text) && string.IsNullOrWhiteSpace(UserPhone.Text))
            {
                errMsg = GetText(Resource.String.error_empty_contacts_user) + System.Environment.NewLine;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (!string.IsNullOrWhiteSpace(UserName.Text) && db.Users.Where(x => x.Name.Trim().ToLower() == UserName.Text.ToLower() && x.Id != userId).Any())
                    {
                        errMsg = GetText(Resource.String.error_duplicate_name_user) + System.Environment.NewLine;
                    }
                    if (!string.IsNullOrWhiteSpace(UserEmail.Text) && db.Users.Where(x => x.Email.Trim().ToLower() == UserEmail.Text.ToLower() && x.Id != userId).Any())
                    {
                        errMsg = GetText(Resource.String.error_duplicate_email_user) + System.Environment.NewLine;
                    }
                    if (!string.IsNullOrWhiteSpace(UserPhone.Text) && db.Users.Where(x => x.Phone.Trim().ToLower() == UserPhone.Text.ToLower() && x.Id != userId).Any())
                    {
                        errMsg = GetText(Resource.String.error_duplicate_phone_user) + System.Environment.NewLine;
                    }
                }
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

            int selected_telegram_id = TelegramUsers.Keys.ToList()[TelegramAccount.SelectedItemPosition];
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = new UserModel()
                    {
                        AlarmSubscriber = UserAlarmSubscribing.Checked,
                        CommandsAllowed = UserCommandsAllowed.Checked,
                        Name = UserName.Text.Trim(),
                        Email = UserEmail.Text.Trim(),
                        Phone = UserPhone.Text.Trim()
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    if (selected_telegram_id > 0)
                    {
                        TelegramUserModel telegramUser = db.TelegramUsers.Find(selected_telegram_id);
                        telegramUser.LinkedUserId = user.Id;
                        db.SaveChanges();
                    }
                }
            }
            StartActivity(typeof(UsersListActivity));
        }
    }
}

