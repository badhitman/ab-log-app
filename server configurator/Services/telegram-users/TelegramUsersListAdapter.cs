////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.EntityFrameworkCore.Internal;

namespace ab.Services
{
    public class TelegramUsersListAdapter : RecyclerView.Adapter
    {
        public readonly string TAG = "telegram-users-list-adapter";

        public static Dictionary<int, string> LinkedUsers;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.TelegramUsers.Count(); } } } }
        Context mContext;
        public TelegramUsersListAdapter(Context context)
        {
            Log.Debug(TAG, "~ constructor");
            mContext = context;
            LinkedUsers = new Dictionary<int, string>
            {
                { 0, "" }
            };
            List<UserModel> users;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    users = db.Users.ToList();
                }
            }
            users.ForEach(x => { LinkedUsers.Add(x.Id, x.Name); });
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int telegram_user_id)
        {
            Log.Debug(TAG, $"OnBindViewHolder - telegram_user_id:{telegram_user_id}");
            TelegramUserListItemViewHolder telegramUsersViewHolder = holder as TelegramUserListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    TelegramUserModel row = db.TelegramUsers.Find(telegram_user_id);

                    telegramUsersViewHolder.TelegramId.Text = row.TelegramId + (string.IsNullOrWhiteSpace(row.UserName) ? "" : $" (@{row.UserName})");

                    if (string.IsNullOrEmpty(row.Name))
                    {
                        telegramUsersViewHolder.TelegramFirstSecondName.Text = "<no name>";
                        telegramUsersViewHolder.TelegramFirstSecondName.SetTextColor(Color.LightGray);
                    }
                    else
                    {
                        telegramUsersViewHolder.TelegramFirstSecondName.Text = row.Name;
                        telegramUsersViewHolder.TelegramFirstSecondName.SetTextColor(Color.Black);
                    }

                    if (LinkedUsers.ContainsKey(row.LinkedUserId))
                    {
                        telegramUsersViewHolder.LinkedUserSpinner.SetSelection(LinkedUsers.Keys.IndexOf(row.LinkedUserId));
                    }
                    else
                    {
                        telegramUsersViewHolder.LinkedUserSpinner.SetSelection(0);
                    }
                    telegramUsersViewHolder.LinkedUserSpinner.Tag = row.Id;
                }
            }

            telegramUsersViewHolder.LinkedUserSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(LinkedUser_ItemSelected);
        }

        private void LinkedUser_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Log.Debug(TAG, "LinkedUser_ItemSelected");
            Spinner spinner = (Spinner)sender;
            int telegram_user_id = (int)spinner.Tag;
            int user_id = LinkedUsers.Keys.ElementAt(e.Position);

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    TelegramUserModel telegramUserModel = db.TelegramUsers.Find(telegram_user_id);
                    if (user_id != telegramUserModel.LinkedUserId)
                    {
                        telegramUserModel.LinkedUserId = user_id;
                        db.TelegramUsers.Update(telegramUserModel);
                        db.SaveChanges();
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            Log.Debug(TAG, "OnCreateViewHolder");
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.telegram_user_list_item, parent, false);

            TelegramUserListItemViewHolder telegramUsersViewHolder = new TelegramUserListItemViewHolder(itemView);
            return telegramUsersViewHolder;
        }
    }
}