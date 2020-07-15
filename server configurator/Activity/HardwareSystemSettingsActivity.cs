﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class HardwareSystemSettingsActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.hardware_system_settings_activity;
        protected override int ToolbarId => Resource.Id.hardware_system_settings_toolbar;
        protected override int DrawerLayoutId => Resource.Id.hardware_system_settings_app_drawer_layout;
        protected override int NavId => Resource.Id.hardware_system_settings_app_nav_view;
        static string TAG = nameof(HardwareSystemSettingsActivity);
        string[] supported_firmwares = new string[] { "(fw: 4.45b5)" };

        WebView webView;
        TextView hardwareCardSubHeader;
        HardwareModel hardware;
        LinearLayout HardwareSystemSettingsLayout;

        Regex a_href_regex = new Regex(@"<a\s+href=([^>]+)>([^<]+)</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex form_regex = new Regex(@"<form\s+action=([^>]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex input_regex = new Regex(@"<input[^>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex input_type_regex = new Regex(@"\s+type=(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex input_name_regex = new Regex(@"\s+name=(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        bool external_web_mode = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            int id = Intent.Extras.GetInt(nameof(HardwareModel.Id), 0);
            hardwareCardSubHeader = FindViewById<TextView>(Resource.Id.hardware_system_settings_card_sub_header);
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    hardware = db.Hardwares.Find(id);
                }
            }
            hardwareCardSubHeader.Text = hardware.Name;
            HardwareSystemSettingsLayout = FindViewById<LinearLayout>(Resource.Id.hardware_system_settings_layout);

            GetHttp($"http://{hardware.Address}/{hardware.Password}/");
        }

        public async void GetHttp(string url)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse(url);
            if (uri.Host == "ab-log.ru")
            {
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
                url += "?ref=https://github.com/badhitman/ab-log-app";
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
                return;
            }

            HardwareSystemSettingsLayout.RemoveAllViews();
            HardwareSystemSettingsLayout.AddView(new ProgressBar(this) { Indeterminate = true });

            webView = new WebView(this);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            webView.Settings.DomStorageEnabled = true;

            MyWebViewClient myWebViewClient = new MyWebViewClient();

            myWebViewClient.ShouldUrlLoading += delegate (string new_url) { GetHttp(new_url); };

            webView.SetWebViewClient(myWebViewClient);
            string html_raw = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage httpResponseMessage = await client.GetAsync(url);
                    html_raw = await httpResponseMessage.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
                }
            }

            string cf = uri.GetQueryParameter("cf") ?? string.Empty;
            string pt = uri.GetQueryParameter("pt") ?? string.Empty;
            string eip = uri.GetQueryParameter("eip") ?? string.Empty;
            string pwd = uri.GetQueryParameter("pwd") ?? string.Empty;

            await Task.Run(() =>
            {
                html_raw = html_raw.Replace("<br>", $"<br>{System.Environment.NewLine}");

                string onload_js;

                /////////////////////////////////////////////////////////////
                /// настройки: основные (ip,gw и т.п.)
                if (cf == "1")
                {
                    onload_js = MyWebViewClient.onload_cf1_js;

                    bool set_ip_address = !string.IsNullOrWhiteSpace(eip);
                    bool set_password = !string.IsNullOrWhiteSpace(pwd);

                    if (set_ip_address || set_password)
                    {
                        Log.Debug(TAG, "save data to DB");
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                List<string> messages = new List<string>();
                                if (set_ip_address && hardware.Address != eip)
                                {
                                    hardware.Address = eip;
                                    messages.Add(GetText(Resource.String.hardware_ip_are_saved_title));
                                }
                                if (set_password && hardware.Password != pwd)
                                {
                                    hardware.Password = pwd;
                                    messages.Add(GetText(Resource.String.hardware_password_are_saved_title));
                                }
                                if (messages.Count > 0)
                                {
                                    db.Hardwares.Update(hardware);
                                    db.SaveChanges();
                                    RunOnUiThread(() =>
                                    {
                                        Toast.MakeText(this, $" • {string.Join($"{System.Environment.NewLine} • ", messages)}", ToastLength.Short).Show();
                                    });
                                    StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
                                }
                            }
                        }
                    }

                    if (external_web_mode)
                    {
                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });

                        html_raw = html_raw
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}")
                        .Replace("<option", $"{System.Environment.NewLine}<option")
                        .Replace("<select", "<select class=\"form-control\"");

                        html_raw = a_href_regex.Replace(html_raw, (Match match) =>
                        {
                            switch (match.Groups[2].Value.ToLower())
                            {
                                case "back":
                                    return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>";
                                default:
                                    return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-outline-info btn-sm\" role=\"button\">{match.Groups[2].Value}</a>";
                            }
                        });

                        html_raw = input_regex.Replace(html_raw, (Match match) =>
                        {
                            string input_raw = match.Groups[0].Value;
                            bool input_with_out_type = true;
                            input_raw = input_type_regex.Replace(input_raw, (Match sub_match) =>
                            {
                                input_with_out_type = false;
                                string input_type_raw = sub_match.Groups[0].Value;
                                switch (sub_match.Groups[1].Value.ToLower())
                                {
                                    case "checkbox":
                                        return $" class=\"form-check-input pb-4 mb-4 ml-1\" {input_type_raw}";
                                    case "submit":
                                        return $" class=\"btn btn-outline-primary btn-block\" {input_type_raw}";
                                    case "hidden":
                                        return input_type_raw;
                                    default:
                                        return $" class=\"form-control\" {input_type_raw}";
                                }
                            });

                            if (input_with_out_type)
                            {
                                input_raw = input_raw.Replace("<input", "<input class=\"form-control\"");
                            }

                            input_raw = input_name_regex.Replace(input_raw, (Match sub_match) =>
                            {
                                string input_name_raw = sub_match.Groups[1].Value;
                                switch (input_name_raw.ToLower())
                                {
                                    case "eip":
                                        return $" placeholder=\"IP адрес устройства\" name=\"{input_name_raw}\"";
                                    case "pwd":
                                        return $" placeholder=\"Пароль\" name=\"{input_name_raw}\"";
                                    case "gw":
                                        return $" placeholder=\"Шлюз\" name=\"{input_name_raw}\"";
                                    case "sip":
                                        return $" placeholder=\"IP-адрес главного сервера\" name=\"{input_name_raw}\"";
                                    case "auth":
                                        return $" placeholder=\"MQTT пароль\" name=\"{input_name_raw}\"";
                                    case "sct":
                                        return $" placeholder=\"Скрипт обработки сообщений\" name=\"{input_name_raw}\"";
                                    //case "rtf":
                                    //    return $"{sub_match.Groups[0].Value} <small class=\"form-text text-muted\">Принудительная доставка MQTT</small>";
                                    case "pr":
                                        return $" placeholder=\"Сторожевой сценарий\" name=\"{input_name_raw}\"";
                                    default:
                                        return $" name=\"{input_name_raw}\"";
                                }
                            });

                            return input_raw;
                        });
                        int connected_status = html_raw.ToLower().IndexOf("disconnected");
                        if (connected_status > 0)
                        {
                            html_raw = $"{html_raw.Substring(0, connected_status)}<div class=\"d-flex justify-content-end pt-3\"><span class=\"badge badge-secondary\">Disconnected</span></div>{html_raw.Substring(connected_status + 12)}";
                        }
                        else
                        {
                            connected_status = html_raw.ToLower().IndexOf("connected");
                            if (connected_status > 0)
                            {
                                html_raw = $"{html_raw.Substring(0, connected_status)}<div class=\"d-flex justify-content-end pt-3\"><span class=\"badge badge-success\">Connected</span></div>{html_raw.Substring(connected_status + 9)}";
                            }
                        }
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: Megad-ID
                else if (cf == "2")
                {
                    onload_js = MyWebViewClient.onload_cf2_js;

                    if (external_web_mode)
                    {
                        html_raw = html_raw
                        .Replace("Megad-ID:", $"{System.Environment.NewLine}<label>Megad-ID:</label>{System.Environment.NewLine}")
                        .Replace("<input type=hidden name=cf value=2>", $"<input type=\"hidden\" name=\"cf\" value=\"2\"/>")
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}")
                        .Replace("<input type=submit value=Save>", "<input class=\"btn btn-outline-primary btn-block\" type=\"submit\" value=\"Save\">")
                        .Replace("<input name=mdid", "<input class=\"form-control\" name=\"mdid\" placeholder=\"Идентификатор\"");

                        html_raw = Regex.Replace(html_raw, @"srv loop:\s+(<input[^>]+>)", (Match match) =>
                        {
                            string input_raw = match.Groups[1].Value
                            .Replace("type=checkbox", "type=\"checkbox\"")
                            .Replace(">", " />")
                            .Replace("<input", "<input id=\"srv-loop\" class=\"custom-control-input\"");
                            return $"<div class=\"custom-control custom-checkbox\">{System.Environment.NewLine}{input_raw}{System.Environment.NewLine}<label for=\"srv-loop\" class=\"custom-control-label\">srv loop</label>{System.Environment.NewLine}</div>{System.Environment.NewLine}";
                        });

                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });
                        html_raw = a_href_regex.Replace(html_raw, (Match match) => { return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>"; });

                        html_raw += "<div class=\"alert alert-info mt-3\" role=\"alert\"><h4>Информация</h4><p><strong>Megad-ID</strong> используется в MQTT для формирования подписки <code>Topic</code>, а тк же <code>ClientId</code>. Например, если назначить это значение <u>droid</u>, то упрвляющий блок подпишется в MQTT брокере на топик <u>droid/cmd</u> с идентификатором клиента <u>megad-droid</u>. В то же время: состояние портов управляющий блок будет публиковать в топиках с именами подстать номерам портов. Например для порта №30(P30) сообщения от блока будут в топике <u>droid/30</u>. Информация о всех событиях MQTT подписок/сообщений в брокере доступна в логах приложения.</p><p><strong>srv loop</strong> включает режим регулярной отправки (раз в минуту) на сервер состояние всех портов</p></div>";
                    }
                }
                else if (cf == "3")
                {
                    onload_js = MyWebViewClient.onload_cf3_js;
                }
                else if (cf == "4")
                {
                    onload_js = MyWebViewClient.onload_cf4_js;
                }
                /////////////////////////////////////////////////////////////
                /// настройки: cron
                else if (cf == "7")
                {
                    onload_js = MyWebViewClient.onload_cf7_js;

                    if (external_web_mode)
                    {
                        html_raw = html_raw
                        .Replace("<input type=hidden name=cf value=7>", $"<input type=\"hidden\" name=\"cf\" value=\"7\"/>")
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}")
                        .Replace("<input type=submit value=Save>", "<button type=\"submit\" class=\"btn btn-outline-primary btn-block\">Save</button>");

                        html_raw = Regex.Replace(html_raw, @"<input name=([\w\d]+)", (Match match) =>
                         {
                             return $"<input class=\"form-control\" name=\"{match.Groups[1].Value}\"";
                         });

                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });
                        html_raw = a_href_regex.Replace(html_raw, (Match match) => { return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>"; });

                        html_raw += "<div class=\"alert alert-info mt-3\" role=\"alert\"><h4>Информация</h4>" +
                        "<p><strong>Cur time:</strong> текущее время. В квадратных скобках указан день недели (1-7). Например [5] - пятница.</p>" +
                        "<p><strong>Set time:</strong> здесь можно задать время. Формат ЧЧ:ММ:СС:ДН, то есть 15:30:00:5 - последняя цифра - день недели</p>" +
                        "<p><strong>SCL/SDA:</strong> порты микроконтроллера, к которым подключены часы. Здесь указывается не номер порта (как обычно), а его индекс (можно посмотреть в документации). Это сделано для того, чтобы имелась возможность подключить часы не только к разъему XT2 (зеленые клеммники внизу) и исполнительным модулям MegaD-14-IOR, но и к внутреннему 16-пиновому разъему XP4.</p>" +
                        "<div class=\"shadow p-3 mb-4 bg-white rounded\">В случае использования контроллера в исполнении MegaD-2561-RTC можно оставить эти поля пустыми. Контроллер сам определит наличие платы часов, подключенных к служебному разъему XP4 и синхронизируется с ними.</div>" +
                        "<p><strong>T/Act:</strong>  Сами задания. Их может быть 5 шт.</p>" +
                        "<p><strong>T:</strong> Расписание в формате ЧЧ:ММ:ДН</p>" +
                        "<div class=\"shadow p-3 mb-4 bg-white rounded\"><code>14:30:0</code> - выполнять в 14:30 каждый день (последний 0 - означает каждый день)<br/>" +
                        "<code>08:00:3</code> - выполнить в 8:00 в среду(3 - среда)<br/>" +
                        "<code>03:15:3-7</code> - выполнять в 03:15 со среды по воскресенье включительно(3-7)<br/>" +
                        "<code>*:/03:0</code> - выполняется каждые 3 минуты.Вместо значения \"час\" необходимо задать '*'.Последний ':0' - дни недели. 0 - каждый день.День недели учитывается.Можно задать выполнение циклической операции в определенные дни недели.<br/>" +
                        "<code>/02:15:0</code> - выполняется каждые 2 часа в 15 минут.То есть в 2:15; 4:15; 6:15 и т.д.</div>" +
                        "<p><strong>Act:</strong> стандартное поле сценария. Важно, что здесь работают паузы (команды p). То есть, если необходимо включить, например, автополив на 30 минут, то не обязательно разносить это на два задания. Можно ограничится одним.</p>" +
                        "<div class=\"shadow p-3 mb-4 bg-white rounded\">"+
                        "<code>7:1;P10;7:0</code> - включить седьмой порт, далее следует пауза 10 секунд, после чего порт будет выключен.<br/>" +
                        "<code>10:1;P600;10:0</code> - включить десятый порт, далее следует пауза 600 секунд, после чего порт будет выключен.<br/>" +
                        "<code>8:1;9:0;12:2</code> - включить восьмой порт и выключить девятый, а двендцтый переключить в противоположное состояние.</div>" +
                        "</div>";
                    }
                }
                else if (cf == "9")
                {
                    onload_js = MyWebViewClient.onload_cf9_js;
                }
                else if (!string.IsNullOrWhiteSpace(pt))
                {
                    onload_js = MyWebViewClient.onload_pt_js;
                }
                /////////////////////////////////////////////////////////////
                /// настройки: начальное навигационное окно
                else
                {
                    onload_js = MyWebViewClient.onload_root_js;
                    if (!external_web_mode)
                    {
                        foreach (string s in supported_firmwares)
                        {
                            external_web_mode = html_raw.Contains(s);
                            if (external_web_mode)
                            {
                                break;
                            }
                        }
                    }

                    if (external_web_mode)
                    {
                        html_raw = a_href_regex.Replace(html_raw.Replace("<br>", string.Empty), (Match m) =>
                        {
                            switch (m.Groups[2].Value.ToLower())
                            {
                                case "config":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-primary btn-block mt-2\" role=\"button\">{m.Groups[2].Value}</a>";
                                case "xp1":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-info btn-block\" role=\"button\">{m.Groups[2].Value} // P0-P13 & P14</a>";
                                case "xp2":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-info btn-block\" role=\"button\">{m.Groups[2].Value} // P15(0)-P28(13) & P29(14)</a>";
                                case "ab-log.ru":
                                    return $"<a href=\"{m.Groups[1].Value}\"><u>{m.Groups[2].Value}</u></a>";
                                default:
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-primary btn-block\" role=\"button\">{m.Groups[2].Value}</a>";
                            }
                        });
                    }
                }

                if (!external_web_mode)
                {
                    html_raw +=
                    "<div style=\"color: #856404; background-color: #fff3cd; border-color: #ffeeba; margin-top: 1rem; padding: .75rem 1.25rem; margin-bottom: 1rem; border: 1px solid transparent; box-sizing: border-box; font-size: 1rem; font-weight: 400; line-height: 1.5;\">" +
                    $"Режим модифицированого WEB интерфейса отключён. Версия вашей прошивки не поддерживается.<hr/>Поддерживаются прошивки: <span style=\"color: #fff;background-color: #007bff;display: inline-block;padding: .25em .4em;font-size: 75%;font-weight: 700;line-height: 1;text-align: center;white-space: nowrap;vertical-align: baseline;border-radius: .25rem;box-sizing: border-box;\">{String.Join("</span><span style=\"color: #fff;background-color: #007bff;display: inline-block;padding: .25em .4em;font-size: 75%;font-weight: 700;line-height: 1;text-align: center;white-space: nowrap;vertical-align: baseline;border-radius: .25rem;box-sizing: border-box;\">", supported_firmwares)}</span></div>";
                }
                else
                {
                    html_raw =
                    $"<link rel=\"stylesheet\" href=\"file:///{MyWebViewClient.bootstrap_min_css}\">" + System.Environment.NewLine +
                    $"<script src=\"file:///{MyWebViewClient.jquery_slim_min_js}\"></script>" + System.Environment.NewLine +
                    $"<script src=\"file:///{MyWebViewClient.popper_min_js}\" ></script>" + System.Environment.NewLine +
                    $"<script src=\"file:///{MyWebViewClient.bootstrap_min_js}\"></script>" + System.Environment.NewLine +
                    $"<script src=\"file:///{onload_js}\"></script>" + System.Environment.NewLine + html_raw;
                }
            });
            webView.LoadDataWithBaseURL(url, html_raw, "text/html", "utf-8", null);

            HardwareSystemSettingsLayout.RemoveAllViews();
            HardwareSystemSettingsLayout.AddView(webView);
        }

        protected override void OnResume()
        {
            base.OnResume();

        }

        protected override void OnPause()
        {
            base.OnPause();

        }
    }
}