////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Services;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System;
using System.IO;
using System.Reflection;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class AboutPageActivity : AbstractActivity
    {
        public static new readonly string TAG = "● about-page-activity";

        protected override int ViewId => Resource.Layout.about_activity;
        protected override int ToolbarId => Resource.Id.about_toolbar;
        protected override int DrawerLayoutId => Resource.Id.about_app_drawer_layout;
        protected override int NavId => Resource.Id.about_app_nav_view;

        LinearLayout linearLayoutAddingInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            linearLayoutAddingInfo = FindViewById<LinearLayout>(Resource.Id.linearLayoutAddingInfo);

            linearLayoutAddingInfo.AddView(new TextView(this) { Text = " " });
            linearLayoutAddingInfo.AddView(new TextView(this) { Text = $"main database file ({gs.SizeDataAsString(new FileInfo(gs.DatabasePathBase).Length)}):" });
            TextView textViewMainDatabaseFilePath = new TextView(this) { Text = gs.DatabasePathBase };
            textViewMainDatabaseFilePath.SetTextColor(Color.DarkGray);
            linearLayoutAddingInfo.AddView(textViewMainDatabaseFilePath);

            linearLayoutAddingInfo.AddView(new TextView(this) { Text = " " });
            linearLayoutAddingInfo.AddView(new TextView(this) { Text = $"logs database file ({gs.SizeDataAsString(new FileInfo(LogsContext.DatabasePathLogs).Length)}):" });
            TextView textViewLogsDatabaseFilePath = new TextView(this) { Text = LogsContext.DatabasePathLogs };
            textViewLogsDatabaseFilePath.SetTextColor(Color.DarkGray);
            linearLayoutAddingInfo.AddView(textViewLogsDatabaseFilePath);

#if DEBUG
            LinearLayout PaletteSystemColors = LayoutInflater.Inflate(Resource.Layout.PaletteSystemColors, linearLayoutAddingInfo, false) as LinearLayout;
            
            Type colorType = typeof(Color);
            
            PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo propInfo in propInfos)
            {
                TextView textViewColor = new TextView(this) { Text = propInfo.Name };
                
                Color color = (Color)propInfo.GetValue(null);
                textViewColor.SetTextColor(color);
                PaletteSystemColors.AddView(textViewColor);
            }
            linearLayoutAddingInfo.AddView(PaletteSystemColors);
#endif
        }
    }
}

