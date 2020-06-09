////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class UserListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        public override int ItemCount
        {
            get
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    lock (gs.DbLocker)
                    {
                        return db.Users.Count();
                    }
                }
            }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            UserViewHolder vh = holder as UserViewHolder;
            lock (gs.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = db.Users.OrderBy(x => x.Id).Skip(position).FirstOrDefault();
                    vh.Name.Text = user.Name;
                    vh.AlarmSubscriber.Text = user.AlarmSubscriber ? "{A}" : " ";
                    vh.CommandsAllowed.Text = user.CommandsAllowed ? "[C]" : " ";
                    vh.Email.Text = $"e-mail: {user.Email}";
                    vh.Phone.Text = $"phone: {user.Phone}";
                    vh.TelegramId.Text = $"telegram: {user.TelegramId}";
                }
            }
            if (string.IsNullOrWhiteSpace(vh.AlarmSubscriber.Text + vh.CommandsAllowed.Text))
            {
                vh.CommandsAllowed.Text = "≡ OFF ≡";
                vh.CommandsAllowed.SetTextColor(Color.LightGray);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.user_list_item, parent, false);

            UserViewHolder vh = new UserViewHolder(itemView, OnClick);
            return vh;
        }
    }
}