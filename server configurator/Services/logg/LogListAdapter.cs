////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Linq;
using System.Runtime.InteropServices;
using ab.Model;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class LogListAdapter : RecyclerView.Adapter
    {
        bool checkBoxTracView = true;
        bool checkBoxInfoView = true;
        bool checkBoxWarnView = true;
        bool checkBoxErrView = true;

        private IQueryable<LogRowModel> GetContext
        {
            get
            {
                LogsContext db = new LogsContext();
                IQueryable<LogRowModel> q_LogRows = db.Logs.Where(x => true);
                if (!checkBoxTracView)
                {
                    q_LogRows = q_LogRows.Where(x => x.Status != LogStatusesEnum.Trac);
                }
                if (!checkBoxInfoView)
                {
                    q_LogRows = q_LogRows.Where(x => x.Status != LogStatusesEnum.Info);
                }
                if (!checkBoxWarnView)
                {
                    q_LogRows = q_LogRows.Where(x => x.Status != LogStatusesEnum.Warn);
                }
                if (!checkBoxErrView)
                {
                    q_LogRows = q_LogRows.Where(x => x.Status != LogStatusesEnum.Error);
                }
                return q_LogRows;
            }
        }

        public override int ItemCount { get { lock (LogsContext.DbLocker) { using (LogsContext db = new LogsContext()) { return GetContext.Count(); } } } }

        public LogListAdapter(bool trac_view, bool info_view, bool warn_view, bool err_view)
        {
            checkBoxTracView = trac_view;
            checkBoxInfoView = info_view;
            checkBoxWarnView = warn_view;
            checkBoxErrView = err_view;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            LogViewHolder logViewHolder = holder as LogViewHolder;
            lock (LogsContext.DbLocker)
            {
                using (LogsContext db = new LogsContext())
                {
                    LogRowModel row = GetContext.OrderByDescending(x => x.CreatedAt).Skip(position).FirstOrDefault();
                    logViewHolder.LogMessageTextMessage.Text = row.Name;
                    logViewHolder.LogMessageDateTimeStamp.Text = row.CreatedAt.ToString("dd MMM HH:mm:ss");
                    logViewHolder.LogMessageStatus.Text = row.Status.ToString().Substring(0, 1);
                    switch (row.Status)
                    {
                        case LogStatusesEnum.Error:
                            logViewHolder.LogMessageStatus.SetTextColor(Color.Red);
                            break;
                        case LogStatusesEnum.Warn:
                            logViewHolder.LogMessageStatus.SetTextColor(Color.Orange);
                            break;
                        case LogStatusesEnum.Info:
                            logViewHolder.LogMessageStatus.SetTextColor(Color.SeaGreen);
                            break;
                        case LogStatusesEnum.Trac:
                            logViewHolder.LogMessageStatus.SetTextColor(Color.Gray);
                            break;
                    }
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