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
        public override int ItemCount { get { using (LogsContext db = new LogsContext()) { return db.Logs.Count(); } } }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            LogViewHolder vh = holder as LogViewHolder;
            lock (LogsContext.DbLocker)
            {
                using (LogsContext db = new LogsContext())
                {
                    LogRowModel row = db.Logs.OrderByDescending(x=>x.CreatedAt).Skip(position).FirstOrDefault();
                    vh.LogMessageTextMessage.Text = row.Name;
                    vh.LogMessageDateTimeStamp.Text = row.CreatedAt.ToString();
                    vh.LogMessageStatus.Text = row.Status.ToString();
                    vh.LogMessageTag.Text = row.TAG.ToString();
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.log_list_item, parent, false);

            LogViewHolder vh = new LogViewHolder(itemView);
            return vh;
        }
    }
}