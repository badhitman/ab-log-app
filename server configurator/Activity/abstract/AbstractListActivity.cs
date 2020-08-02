////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.OS;
using Android.Util;
using Google.Android.Material.FloatingActionButton;

namespace ab
{
    public abstract class AbstractListActivity : AbstractActivity
    {
        public static new readonly string TAG = "● abstract-list-activity";

        protected abstract int ButtonAdd { get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");

            base.OnCreate(savedInstanceState);
            FloatingActionButton ButtonAddAction = FindViewById<FloatingActionButton>(ButtonAdd);
            ButtonAddAction.Click += ButtonAddOnClick;
        }

        protected abstract void ButtonAddOnClick(object sender, EventArgs eventArgs);
    }
}

