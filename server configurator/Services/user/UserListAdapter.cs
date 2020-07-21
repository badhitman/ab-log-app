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

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.Users.Count(); } } } }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            UserViewHolder userViewHolder = holder as UserViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = db.Users.Skip(position).FirstOrDefault();
                    userViewHolder.Name.Text = user.Name;
                    userViewHolder.AlarmSubscriber.Text = user.AlarmSubscriber ? "{a}" : " ";
                    userViewHolder.CommandsAllowed.Text = user.CommandsAllowed ? "[c]" : " ";
                    userViewHolder.Email.Text = $"e-mail: {user.Email}";
                    userViewHolder.Phone.Text = $"phone: {user.Phone}";
                }
            }
            if (string.IsNullOrWhiteSpace(userViewHolder.AlarmSubscriber.Text + userViewHolder.CommandsAllowed.Text))
            {
                userViewHolder.CommandsAllowed.Text = "≡ off ≡";
                userViewHolder.CommandsAllowed.SetTextColor(Color.LightGray);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.user_list_item, parent, false);

            UserViewHolder userViewHolder = new UserViewHolder(itemView, OnClick);
            return userViewHolder;
        }
    }
}