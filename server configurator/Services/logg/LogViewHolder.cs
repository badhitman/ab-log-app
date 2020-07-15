////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class LogViewHolder : RecyclerView.ViewHolder
    {
        public TextView LogMessageTextMessage { get; private set; }
        public TextView LogMessageDateTimeStamp { get; private set; }
        public TextView LogMessageStatus { get; private set; }
        public TextView LogMessageTag { get; private set; }

        public LogViewHolder(View itemView) : base(itemView)
        {
            LogMessageTextMessage = itemView.FindViewById<TextView>(Resource.Id.log_list_item_message_text_message);
            LogMessageDateTimeStamp = itemView.FindViewById<TextView>(Resource.Id.log_list_item_message_date_time_stamp);
            LogMessageStatus = itemView.FindViewById<TextView>(Resource.Id.log_list_item_message_status);
            LogMessageTag = itemView.FindViewById<TextView>(Resource.Id.log_list_item_message_tag);
        }
    }
}