////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Linq;
using ab.Model;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class LogListAdapter : RecyclerView.Adapter
    {
        public override int ItemCount { get { lock (LogsContext.DbLocker) { using (LogsContext db = new LogsContext()) { return db.Logs.Count(); } } } }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            LogViewHolder logViewHolder = holder as LogViewHolder;
            lock (LogsContext.DbLocker)
            {
                using (LogsContext db = new LogsContext())
                {
                    LogRowModel row = db.Logs.OrderByDescending(x => x.CreatedAt).Skip(position).FirstOrDefault();
                    logViewHolder.LogMessageTextMessage.Text = row.Name;
                    logViewHolder.LogMessageDateTimeStamp.Text = row.CreatedAt.ToString();
                    logViewHolder.LogMessageStatus.Text = row.Status.ToString();
                    logViewHolder.LogMessageTag.Text = row.TAG.ToString();
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.log_list_item, parent, false);

            LogViewHolder logViewHolder = new LogViewHolder(itemView);
            return logViewHolder;
        }
    }
}