﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NijieDownloader.Library.Model;
using HtmlAgilityPack;
using Nandaka.Common;
using System.Collections.Specialized;

using System.Threading.Tasks;
using System.ComponentModel;

namespace NijieDownloader.Library
{
    public partial class Nijie
    {
        public static event NijieEventHandler LoggingEventHandler;

        public delegate void NijieEventHandler(object sender, bool result);

        private static bool _isLoggedIn;
        public static bool IsLoggedIn {
            get
            {
                return Nijie._isLoggedIn;
            }
            private set
            {
                Nijie._isLoggedIn = value;
                if (LoggingEventHandler != null)
                {
                    LoggingEventHandler(null, value);
                }
            }
        }

        public bool Login(string userName, string password, bool useHttps)
        {
            var info = PrepareLoginInfo(userName, password, useHttps);
            return DoLogin(info, useHttps);
        }

        public void Logout()
        {
            ExtendedWebClient.ClearCookie();
            IsLoggedIn = false;
        }

        public void LoginAsync(string userName, string password, Action<bool, string> callback, bool useHttps)
        {
            var task = Task.Factory.StartNew<bool>(() => Login(userName, password, useHttps));
            task.ContinueWith(x => {
                try
                {
                    if(x.Result)
                        callback(x.Result, "Login Success.");
                    else
                        callback(x.Result, "Invalid username or password.");
                }
                catch (AggregateException ex)
                {
                    callback(false, "Error: " + ex.InnerException.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        private NijieLoginInfo PrepareLoginInfo(string userName, string password, bool useHttps)
        {
            ExtendedWebClient client = new ExtendedWebClient();
            NijieLoginInfo info = new NijieLoginInfo() { UserName = userName, Password = password, ReturnUrl = "", Ticket = "", RememberLogin = false };

            HtmlDocument doc = getPage(Util.FixUrl(NijieConstants.NIJIE_LOGIN_URL, useHttps)).Item1;

            var tickets = doc.DocumentNode.SelectNodes("//input[@name='ticket']");
            if (tickets != null && tickets.Count > 0)
                info.Ticket = tickets[0].Attributes["value"].Value;

            var returnUrls = doc.DocumentNode.SelectNodes("//input[@name='url']");
            if (returnUrls != null && returnUrls.Count > 0)
                info.ReturnUrl = returnUrls[0].Attributes["value"].Value;

            return info;
        }

        private bool DoLogin(NijieLoginInfo info, bool useHttps)
        {
            IsLoggedIn = false;
            ExtendedWebClient client = new ExtendedWebClient();
            NameValueCollection loginInfo = new NameValueCollection();
            loginInfo.Add("email", info.UserName);
            loginInfo.Add("password", info.Password);
            if (info.RememberLogin)
                loginInfo.Add("save", "on");
            loginInfo.Add("ticket", info.Ticket);
            loginInfo.Add("url", info.ReturnUrl);

            var result = client.UploadValues(Util.FixUrl(NijieConstants.NIJIE_LOGIN_URL2, useHttps), "POST", loginInfo);
            //String data = Encoding.UTF8.GetString(result);

            var location = client.Response.ResponseUri.ToString();
            if (!String.IsNullOrWhiteSpace(location))
            {
                if (location.Contains(@"//nijie.info/login.php?"))
                    IsLoggedIn = false;
                else
                    IsLoggedIn = true;
            }
            return IsLoggedIn;
        }

    }
}
