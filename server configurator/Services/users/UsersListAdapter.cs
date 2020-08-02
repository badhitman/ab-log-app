////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class UsersListAdapter : RecyclerView.Adapter
    {
        public static readonly string TAG = "● users-list-adapter";

        public event EventHandler<int> ItemClick;
        Context mContext;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.Users.Count(); } } } }

        public UsersListAdapter(Context context)
        {
            Log.Debug(TAG, "~ constructor");

            mContext = context;
        }

        void OnClick(int user_id)
        {
            Log.Debug(TAG, $"OnClick - user_id:{user_id}");

            ItemClick?.Invoke(this, user_id);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Log.Debug(TAG, $"OnBindViewHolder - position:{position}");

            UserListItemViewHolder userViewHolder = holder as UserListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    UserModel user = db.Users.Skip(position).FirstOrDefault();
                    userViewHolder.ObjectId = user.Id;
                    //
                    userViewHolder.Name.Text = user.Name;
                    userViewHolder.AlarmSubscriber.Text = user.AlarmSubscriber ? "{a}" : " ";
                    userViewHolder.CommandsAllowed.Text = user.CommandsAllowed ? "[c]" : " ";
                    //userViewHolder.Email.Text = $"e-mail: {user.Email}";
                    //userViewHolder.Phone.Text = $"phone: {user.Phone}";
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
            Log.Debug(TAG, "OnCreateViewHolder");

            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.users_list_item, parent, false);

            UserListItemViewHolder userViewHolder = new UserListItemViewHolder(itemView, OnClick);
            return userViewHolder;
        }
    }
}