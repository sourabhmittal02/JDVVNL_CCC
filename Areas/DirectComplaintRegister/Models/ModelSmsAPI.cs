using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirectComplaintRegister.Models
{
    public class ModelSmsAPI
    {

        private string _smsApiURL = System.Configuration.ConfigurationManager.AppSettings["smsApiURL"];
        private string _username = System.Configuration.ConfigurationManager.AppSettings["username"];
        private string _password = System.Configuration.ConfigurationManager.AppSettings["password"];
        private string _senderid = System.Configuration.ConfigurationManager.AppSettings["senderid"];
        private string _secureKey = System.Configuration.ConfigurationManager.AppSettings["secureKey"];

        private string _consumer = System.Configuration.ConfigurationManager.AppSettings["Consumer"];
        private string _cC = System.Configuration.ConfigurationManager.AppSettings["CC"];
        private string _NCMS_Consumer = System.Configuration.ConfigurationManager.AppSettings["NCMS_ConsumerSMS"];
        private string _NCMS_CC = System.Configuration.ConfigurationManager.AppSettings["NCMS_CCSMS"];


        private string _to;
        private string _smstext;
        private string _smstemplete;
        public string SmsApiURL { get { return _smsApiURL; } }

        public string Username { get { return _username; } }
        public string Password { get { return _password; } }
        public string SenderId { get { return _senderid; } }

        public string SecureKey { get { return _secureKey; } }
        public string To { get { return _to; } set { _to = value; } }
        public string Smstext { get { return _smstext; } set { _smstext = value; } }
        public string Smstemplete { get { return _smstemplete; } set { _smstemplete = value; } }


        public string ConsumerSMS { get { return _consumer; } }
        public string CCSMS { get { return _cC; } }
        public string NCMS_ConsumerSMS { get { return _NCMS_Consumer; } }
        public string NCMS_CCSMS { get { return _NCMS_CC; } }

    }

    public class ModelSmsAPISendSMS
    {
        private string _to;
        private string _smstext;
        private string _templateid;
        private string _id;

        public string to { get { return _to; } set { _to = value; } }
        public string smsText { get { return _smstext; } set { _smstext = value; } }
        public string templateid { get { return _templateid; } set { _templateid = value; } }
        public string id { get { return _id; } set { _id = value; } }

    }


}
