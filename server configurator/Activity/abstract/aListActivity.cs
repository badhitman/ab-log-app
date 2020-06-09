////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////
using System;
using Android.OS;
using Google.Android.Material.FloatingActionButton;

namespace ab
{
    public abstract class aListActivity : aActivity
    {
        protected abstract int ButtonAdd { get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FloatingActionButton ButtonAddAction = FindViewById<FloatingActionButton>(ButtonAdd);
            ButtonAddAction.Click += ButtonAddOnClick;
        }

        protected abstract void ButtonAddOnClick(object sender, EventArgs eventArgs);
    }
}

