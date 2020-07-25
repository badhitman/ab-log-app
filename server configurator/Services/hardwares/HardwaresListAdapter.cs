////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class HardwaresListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Context mContext;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.Hardwares.Count(); } } } }

        public HardwaresListAdapter(Context _mContext)
        {
            mContext = _mContext;
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            HardwareViewHolder vh = holder as HardwareViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    HardwareModel hardware = db.Hardwares.Skip(position).FirstOrDefault();
                    vh.Name.Text = hardware.Name;
                    vh.Address.Text = hardware.Address;
                    if (!hardware.AlarmSubscriber)
                    {
                        vh.AlarmSubscriber.Text = mContext.GetText(Resource.String.mute_marker_title);
                        vh.AlarmSubscriber.SetTextColor(Color.LightGray);
                    }
                    if (!hardware.CommandsAllowed)
                    {
                        vh.CommandsAllowed.Text = mContext.GetText(Resource.String.deaf_marker_title);
                        vh.CommandsAllowed.SetTextColor(Color.LightGray);
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.hardwares_list_item, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            HardwareViewHolder vh = new HardwareViewHolder(itemView, OnClick);
            return vh;
        }
    }
}