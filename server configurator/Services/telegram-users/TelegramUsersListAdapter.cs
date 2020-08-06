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
using Microsoft.EntityFrameworkCore;

namespace ab.Services
{
    public class TelegramUsersListAdapter : RecyclerView.Adapter
    {
        public static readonly string TAG = "● telegram-users-list-adapter";

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.TelegramUsers.Count(); } } } }
        Context mContext;
        public TelegramUsersListAdapter(Context context)
        {
            Log.Debug(TAG, "~ constructor");
            mContext = context;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Log.Debug(TAG, $"OnBindViewHolder - position:{position}");
            TelegramUserListItemViewHolder telegramUsersViewHolder = holder as TelegramUserListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    TelegramUserModel row = db.TelegramUsers.Include(x => x.LinkedUser).Skip(position).FirstOrDefault();

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

                    if (row.LinkedUser != null)
                    {
                        telegramUsersViewHolder.LinkedUser.Text = row.LinkedUser.Name;
                        telegramUsersViewHolder.LinkedUser.SetTextColor(Color.Black);
                    }
                    else
                    {
                        telegramUsersViewHolder.LinkedUser.Text = "< - >";
                        telegramUsersViewHolder.LinkedUser.SetTextColor(Color.Gray);
                    }

                    telegramUsersViewHolder.LinkedUser.Tag = row.Id;
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