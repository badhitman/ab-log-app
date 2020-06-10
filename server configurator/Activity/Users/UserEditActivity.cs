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
    public class UserEditActivity : UserCreateActivity
    {
        protected override int ViewId => Resource.Layout.user_activity;
        protected int userId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ReadView();

            lock (DatabaseContext.DbLocker)
            {
                //using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                //{
                if (gs.SelectedListPosition < 0 || DatabaseContext.UsersCached.Count() < gs.SelectedListPosition + 1)
                {
                    string err_title = Resources.GetString(Resource.String.err_title_2);
                    UserName.Text = err_title;
                    UserName.Enabled = false;

                    UserEmail.Text = err_title;
                    UserEmail.Enabled = false;

                    UserPhone.Text = err_title;
                    UserPhone.Enabled = false;

                    UserTelegram.Text = err_title;
                    UserTelegram.Enabled = false;

                    UserAlarmSubscribing.Enabled = false;
                    UserCommandsAllowed.Enabled = false;

                    UserCardSubHeader.Text = err_title;
                    UserCardSubHeader.Enabled = false;

                    UserCardButtonOk.Enabled = false;
                    return;
                }

                UserModel user = DatabaseContext.UsersCached.Skip(gs.SelectedListPosition).FirstOrDefault();
                userId = user?.Id ?? 0;
                UserName.Text = user?.Name;
                UserEmail.Text = user?.Email;
                UserPhone.Text = user?.Phone;
                UserTelegram.Text = user?.TelegramId;
                UserAlarmSubscribing.Checked = user.AlarmSubscriber;
                UserCommandsAllowed.Checked = user.CommandsAllowed;
                //}
            }

            UserCardHeader.Text = Resources.GetString(Resource.String.edit_user_title);
            UserCardSubHeader.Text = Resources.GetString(Resource.String.edit_user_sub_title);

            AppCompatButton buttonDeleteUser = new AppCompatButton(this) { Text = Resources.GetString(Resource.String.delete_title) };
            buttonDeleteUser.SetTextColor(Color.DarkRed);
            buttonDeleteUser.Click += ButtonDeleteUser_Click;
            buttonDeleteUser.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            UserFooterLayout.AddView(buttonDeleteUser);
        }

        private void ButtonDeleteUser_Click(object sender, EventArgs e)
        {
            AppCompatButton buttonDeleteUser = sender as AppCompatButton;
            buttonDeleteUser.Enabled = false;
            UserCardButtonOk.Enabled = false;
            UserCardButtonOk.Text = Resources.GetString(Resource.String.ok_mute_button_with_remove_user);
            UserName.Enabled = false;
            UserEmail.Enabled = false;
            UserPhone.Enabled = false;
            UserTelegram.Enabled = false;
            UserAlarmSubscribing.Enabled = false;
            UserCommandsAllowed.Enabled = false;
            UserCardSubHeader.Text = Resources.GetString(Resource.String.delete_user_card_sub_title);
            UserCardHeader.Text = Resources.GetString(Resource.String.delete_user_card_header);
            UserCardSubHeader.SetTextColor(Color.IndianRed);
            buttonDeleteUser.SetTextColor(Color.Gray);

            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = Resources.GetString(Resource.String.footer_text_with_remove_user), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            UserFooterLayout.AddView(appCompatTextView);

            AppCompatButton buttonConfirmDeleteUser = new AppCompatButton(this) { Text = Resources.GetString(Resource.String.button_confirm_remove) };
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
                        DatabaseContext.UsersCached.RemoveAt(gs.SelectedListPosition);
                        StartActivity(typeof(UsersActivity));
                    }
                }
            });
            buttonConfirmDeleteUser.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            UserFooterLayout.AddView(buttonConfirmDeleteUser);
        }

        protected override void HandlerUserButtonOk_Click(object sender, EventArgs e)
        {
            string errMsg = ReadView(userId);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                UserCardSubHeader.Text = errMsg;
                UserCardSubHeader.SetTextColor(Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = db.Users.Find(userId);

                    user.Name = UserName.Text.Trim();
                    user.Email = UserEmail.Text.Trim();
                    user.Phone = UserPhone.Text.Trim();
                    user.TelegramId = UserTelegram.Text.Trim();

                    user.AlarmSubscriber = UserAlarmSubscribing.Checked;
                    user.CommandsAllowed = UserCommandsAllowed.Checked;

                    db.Users.Update(user);
                    db.SaveChanges();
                    DatabaseContext.UsersCached[gs.SelectedListPosition] = user;
                }
            }
            StartActivity(typeof(UsersActivity));
        }
    }
}