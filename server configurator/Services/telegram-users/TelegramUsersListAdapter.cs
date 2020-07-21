////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Xamarin.Forms.Internals;

namespace ab.Services
{
    public class TelegramUsersListAdapter : RecyclerView.Adapter
    {
        public static Dictionary<int, string> LinkedUsers;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.TelegramUsers.Count(); } } } }

        public TelegramUsersListAdapter()
        {
            LinkedUsers = new Dictionary<int, string>();
            LinkedUsers.Add(0, "");
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    db.Users.ForEach(x => { LinkedUsers.Add(x.Id, x.Name); });
                }
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TelegramUsersViewHolder telegramUsersViewHolder = holder as TelegramUsersViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    TelegramUserModel row = db.TelegramUsers.OrderByDescending(x => x.Id).Skip(position).FirstOrDefault();

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
            Spinner spinner = (Spinner)sender;
            int telegram_user_id = (int)spinner.Tag;
            int user_id = LinkedUsers.Keys.ElementAt(e.Position);

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    //UserModel userModel = db.Users.Find(user_id);
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.telegram_user_list_item, parent, false);

            TelegramUsersViewHolder telegramUsersViewHolder = new TelegramUsersViewHolder(itemView);
            return telegramUsersViewHolder;
        }
    }
}