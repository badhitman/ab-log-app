////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using Microsoft.EntityFrameworkCore;

namespace ab.Services
{
    public class ScriptsListAdapter : RecyclerView.Adapter
    {
        public static readonly string TAG = "● scripts-list-adapter";

        public event EventHandler<int> ItemClick;
        Context mContext;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.Scripts.Count(); } } } }

        public ScriptsListAdapter(Context _mContext)
        {
            mContext = _mContext;
        }

        void OnClick(int scriptId)
        {
            if (ItemClick != null)
                ItemClick(this, scriptId);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ScriptListItemViewHolder scriptListItemViewHolder = holder as ScriptListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    ScriptModel script = db.Scripts.Skip(position).Include(x => x.TriggerPort).ThenInclude(x => x.Hardware).FirstOrDefault();
                    scriptListItemViewHolder.ScriptId = script.Id;
                    //
                    scriptListItemViewHolder.Name.Text = script.Name;
                    if (script.TriggerPort != null)
                    {
                        scriptListItemViewHolder.Name.Text += $" (tg: {script.TriggerPort.Hardware.Name} > {script.TriggerPort} > {(script.TriggerPortState == true ? "ON" : (script.TriggerPortState == false ? "OFF" : "Switch"))})";
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.scripts_list_item, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            ScriptListItemViewHolder scriptListItemViewHolder = new ScriptListItemViewHolder(itemView, OnClick);
            return scriptListItemViewHolder;
        }
    }
}