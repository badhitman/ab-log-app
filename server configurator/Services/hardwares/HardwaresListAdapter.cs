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
    public class HardwaresListAdapter : RecyclerView.Adapter
    {
        public readonly string TAG = "hardwares-list-adapter";

        public event EventHandler<int> ItemClick;
        Context mContext;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.Hardwares.Count(); } } } }

        public HardwaresListAdapter(Context context)
        {
            Log.Debug(TAG, "~ constructor");
            mContext = context;
        }

        void OnClick(int hardware_id)
        {
            Log.Debug(TAG, $"OnClick - hardware_id:{hardware_id}");
            ItemClick?.Invoke(this, hardware_id);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Log.Debug(TAG, $"OnBindViewHolder - position:{position}");
            HardwareListItemViewHolder hardwareViewHolder = holder as HardwareListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    HardwareModel hardware = db.Hardwares.Skip(position).FirstOrDefault();
                    hardwareViewHolder.ObjectId = hardware.Id;
                    //
                    hardwareViewHolder.Name.Text = hardware.Name;
                    hardwareViewHolder.Address.Text = hardware.Address;
                    if (!hardware.AlarmSubscriber)
                    {
                        hardwareViewHolder.AlarmSubscriber.Text = mContext.GetText(Resource.String.mute_marker_title);
                        hardwareViewHolder.AlarmSubscriber.SetTextColor(Color.LightGray);
                    }
                    if (!hardware.CommandsAllowed)
                    {
                        hardwareViewHolder.CommandsAllowed.Text = mContext.GetText(Resource.String.deaf_marker_title);
                        hardwareViewHolder.CommandsAllowed.SetTextColor(Color.LightGray);
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            Log.Debug(TAG, $"OnCreateViewHolder");
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.hardwares_list_item, parent, false);
            HardwareListItemViewHolder hardwareViewHolder = new HardwareListItemViewHolder(itemView, OnClick);
            return hardwareViewHolder;
        }
    }
}