using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CocoChat.Models
{
    public class Chat
    {
        public string username { get; set; }
        public string text { get; set; }
        public int timeout { get; set; }
    }

}