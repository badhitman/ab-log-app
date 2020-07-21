////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class CloudActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.cloud_activity;
        protected override int ToolbarId => Resource.Id.cloud_toolbar;
        protected override int DrawerLayoutId => Resource.Id.cloud_drawer_layout;
        protected override int NavId => Resource.Id.cloud_nav_view;

        #region fields
        private Switch switch_enable_first_cloud;
        private EditText cloud_first_email_address;
        private EditText cloud_first_email_login;
        private EditText cloud_first_pass;
        private EditText cloud_first_pop3;
        private EditText cloud_first_smtp;

        private Spinner second_cloud_mode_spinner;

        private Switch switch_enable_second_cloud;
        private EditText cloud_second_email_address;
        private EditText cloud_second_email_login;
        private EditText cloud_second_pass;
        private EditText cloud_second_pop3;
        private EditText cloud_second_smtp;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.second_cloud_mode_spinner);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Resources.GetStringArray(Resource.Array.cloud_modes));

            spinner.Adapter = adapter;

            #region first cloud
            switch_enable_first_cloud = FindViewById<Switch>(Resource.Id.switch_enable_first_cloud);
            switch_enable_first_cloud.Checked = Preferences.Get(Resources.GetResourceEntryName(switch_enable_first_cloud.Id), false);
            //
            cloud_first_email_address = FindViewById<EditText>(Resource.Id.cloud_first_email_address);
            cloud_first_email_address.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_email_address.Id), "");
            //
            cloud_first_email_login = FindViewById<EditText>(Resource.Id.cloud_first_email_login);
            cloud_first_email_login.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_email_login.Id), "");
            //
            cloud_first_pass = FindViewById<EditText>(Resource.Id.cloud_first_pass);
            cloud_first_pass.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_pass.Id), "");
            //
            cloud_first_pop3 = FindViewById<EditText>(Resource.Id.cloud_first_pop3);
            cloud_first_pop3.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_pop3.Id), "");
            //
            cloud_first_smtp = FindViewById<EditText>(Resource.Id.cloud_first_smtp);
            cloud_first_smtp.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_smtp.Id), "");
            #endregion

            #region second cloud
            switch_enable_second_cloud = FindViewById<Switch>(Resource.Id.switch_enable_second_cloud);
            switch_enable_second_cloud.Checked = Preferences.Get(Resources.GetResourceEntryName(switch_enable_second_cloud.Id), false);
            //
            cloud_second_email_address = FindViewById<EditText>(Resource.Id.cloud_second_email_address);
            cloud_second_email_address.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_email_address.Id), "");
            //
            cloud_second_email_login = FindViewById<EditText>(Resource.Id.cloud_second_email_login);
            cloud_second_email_login.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_email_login.Id), "");
            //
            cloud_second_pass = FindViewById<EditText>(Resource.Id.cloud_second_pass);
            cloud_second_pass.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_pass.Id), "");
            //
            cloud_second_pop3 = FindViewById<EditText>(Resource.Id.cloud_second_pop3);
            cloud_second_pop3.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_pop3.Id), "");
            //
            cloud_second_smtp = FindViewById<EditText>(Resource.Id.cloud_second_smtp);
            cloud_second_smtp.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_smtp.Id), "");

            second_cloud_mode_spinner = FindViewById<Spinner>(Resource.Id.second_cloud_mode_spinner);
            second_cloud_mode_spinner.SetSelection(Preferences.Get(Resources.GetResourceEntryName(second_cloud_mode_spinner.Id), 0), true);
            #endregion
        }

        protected override void OnResume()
        {
            base.OnResume();
            #region first cloud
            switch_enable_first_cloud.CheckedChange += CloudEnable_CheckedChange;
            //
            cloud_first_email_address.FocusChange += CloudPref_FocusChange;
            cloud_first_email_login.FocusChange += CloudPref_FocusChange;
            cloud_first_pass.FocusChange += CloudPref_FocusChange;
            cloud_first_pop3.FocusChange += CloudPref_FocusChange;
            cloud_first_smtp.FocusChange += CloudPref_FocusChange;
            #endregion

            #region second cloud
            switch_enable_second_cloud.CheckedChange += CloudEnable_CheckedChange;
            //
            cloud_second_email_address.FocusChange += CloudPref_FocusChange;
            cloud_second_email_login.FocusChange += CloudPref_FocusChange;
            cloud_second_pass.FocusChange += CloudPref_FocusChange;
            cloud_second_pop3.FocusChange += CloudPref_FocusChange;
            cloud_second_smtp.FocusChange += CloudPref_FocusChange;

            second_cloud_mode_spinner.ItemSelected += SecondCloudMode_ItemSelected;
            #endregion
        }

        protected override void OnPause()
        {
            base.OnPause();
            #region first cloud
            switch_enable_first_cloud.CheckedChange -= CloudEnable_CheckedChange;
            //
            cloud_first_email_address.FocusChange -= CloudPref_FocusChange;
            cloud_first_email_login.FocusChange -= CloudPref_FocusChange;
            cloud_first_pass.FocusChange -= CloudPref_FocusChange;
            cloud_first_pop3.FocusChange -= CloudPref_FocusChange;
            cloud_first_smtp.FocusChange -= CloudPref_FocusChange;
            #endregion

            #region second cloud
            switch_enable_second_cloud.CheckedChange -= CloudEnable_CheckedChange;
            //
            cloud_second_email_address.FocusChange -= CloudPref_FocusChange;
            cloud_second_email_login.FocusChange -= CloudPref_FocusChange;
            cloud_second_pass.FocusChange -= CloudPref_FocusChange;
            cloud_second_pop3.FocusChange -= CloudPref_FocusChange;
            cloud_second_smtp.FocusChange -= CloudPref_FocusChange;

            second_cloud_mode_spinner.ItemSelected -= SecondCloudMode_ItemSelected;
            #endregion
        }

        private void CloudPref_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                EditText editText = (EditText)sender;
                Preferences.Set(Resources.GetResourceEntryName(editText.Id), editText.Text);
                LinearLayout linearParent = editText.Parent as LinearLayout;
                if (linearParent.Tag.ToString() == GetText(Resource.String.main_cloud_layout))
                {
                    switch_enable_first_cloud.Checked = false;
                }
                else
                {
                    switch_enable_second_cloud.Checked = false;
                }
            }
        }

        private void CloudEnable_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch togglerSwitch = (Switch)sender;

            LinearLayout linearParentLayout = togglerSwitch.Parent.Parent as LinearLayout;

            string email_address = ((AppCompatEditText)linearParentLayout.GetChildAt(1)).Text;
            string login_user = ((AppCompatEditText)linearParentLayout.GetChildAt(2)).Text;
            string pass_user = ((AppCompatEditText)linearParentLayout.GetChildAt(3)).Text;
            string pop3_server_address = ((AppCompatEditText)linearParentLayout.GetChildAt(4)).Text;
            string smtp_server_address = ((AppCompatEditText)linearParentLayout.GetChildAt(5)).Text;

            Preferences.Set(Resources.GetResourceEntryName(togglerSwitch.Id), e.IsChecked);
            if (e.IsChecked)
            {
                if (string.IsNullOrWhiteSpace(email_address) || string.IsNullOrWhiteSpace(login_user) || string.IsNullOrWhiteSpace(pass_user) || string.IsNullOrWhiteSpace(pop3_server_address) || string.IsNullOrWhiteSpace(smtp_server_address))
                {
                    togglerSwitch.Checked = false;
                    Toast.MakeText(this, GetText(Resource.String.err_empty_prop_any_cloud), ToastLength.Short).Show();
                    return;
                }

                LinearLayout ll = togglerSwitch.Parent as LinearLayout;
                ll.AddView(new ProgressBar(this) { ScaleX = 0.4f, ScaleY = 0.4f }, 1);
                togglerSwitch.Enabled = false;
                _ = TestEmailConnect(email_address, login_user, pass_user, pop3_server_address, smtp_server_address, togglerSwitch);
            }
        }

        private async Task TestEmailConnect(string email_address, string login_user, string pass_user, string pop3_server_address, string smtp_server_address, Switch switchToggler)
        {
            Regex regexServerPort = new Regex(@":\d+$");
            int smtp_server_port;

            Match matchServerPort = regexServerPort.Match(smtp_server_address);
            if (!matchServerPort.Success)
            {
                RunOnUiThread(() =>
                {
                    switchToggler.Checked = false;
                    Toast.MakeText(this, GetText(Resource.String.error_cloud_email_smtp_port), ToastLength.Long).Show();
                });
                goto Finish;
            }
            smtp_server_port = int.Parse(matchServerPort.Value.Substring(1));
            smtp_server_address = smtp_server_address.Substring(0, smtp_server_address.Length - matchServerPort.Value.Length);

            int pop3_server_port;
            matchServerPort = regexServerPort.Match(pop3_server_address);
            if (!matchServerPort.Success)
            {
                RunOnUiThread(() =>
                {
                    switchToggler.Checked = false;
                    Toast.MakeText(this, GetText(Resource.String.error_cloud_email_pop3_port), ToastLength.Long).Show();
                });
                goto Finish;
            }
            pop3_server_port = int.Parse(matchServerPort.Value.Substring(1));
            pop3_server_address = pop3_server_address.Substring(0, pop3_server_address.Length - matchServerPort.Value.Length);

            MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(email_address, email_address));
            emailMessage.To.Add(new MailboxAddress(email_address, email_address));
            emailMessage.Subject = GetType().Name;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = nameof(TestEmailConnect)
            };

            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtp_server_address, smtp_server_port, true);
                    await client.AuthenticateAsync(login_user, pass_user);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, GetText(Resource.String.cloud_connection_smtp_successfully), ToastLength.Short).Show();
                    });
                }
                catch (Exception e)
                {
                    RunOnUiThread(() =>
                    {
                        switchToggler.Checked = false;
                        Toast.MakeText(this, $"{GetText(Resource.String.error_connecting_smtp_cloud)}{System.Environment.NewLine}{e.Message}", ToastLength.Long).Show();

                        switchToggler.Enabled = true;
                        (switchToggler.Parent as LinearLayout).RemoveViewAt(1);
                    });
                    return;
                }
            }

            using (Pop3Client client = new Pop3Client())
            {
                try
                {
                    await client.ConnectAsync(pop3_server_address, pop3_server_port, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(login_user, pass_user);

                    if (!client.Capabilities.HasFlag(Pop3Capabilities.UIDL))
                        throw new Exception("The POP3 server does not support UIDL!");

                    //System.Collections.Generic.IList<string> uids = client.GetMessageUids();

                    //for (int i = 0; i < client.Count; i++)
                    //{
                    // check that we haven't already downloaded this message
                    // in a previous session
                    //if (previouslyDownloadedUids.Contains(uids[i]))
                    //    continue;

                    //MimeMessage message = client.GetMessage(i);

                    // write the message to a file
                    //message.WriteTo(string.Format("{0}.msg", uids[i]));

                    // add the message uid to our list of downloaded uids
                    //previouslyDownloadedUids.Add(uids[i]);
                    //}

                    await client.DisconnectAsync(true);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, GetText(Resource.String.cloud_connection_pop3_successfully), ToastLength.Short).Show();
                    });
                }
                catch (Exception e)
                {
                    RunOnUiThread(() =>
                    {
                        switchToggler.Checked = false;
                        Toast.MakeText(this, $"{GetText(Resource.String.error_connecting_pop3_cloud)}{System.Environment.NewLine}{e.Message}", ToastLength.Long).Show();

                        switchToggler.Enabled = true;
                        (switchToggler.Parent as LinearLayout).RemoveViewAt(1);
                    });
                    return;
                }
            }

        Finish:
            switchToggler.Enabled = true;
            (switchToggler.Parent as LinearLayout).RemoveViewAt(1);
        }

        private void SecondCloudMode_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            Preferences.Set(Resources.GetResourceEntryName(spinner.Id), spinner.SelectedItemPosition);
        }
    }
}