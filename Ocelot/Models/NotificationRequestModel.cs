using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class NotificationRequestModel
    {
        public string ToRecipient { get; set; }
        public string CCRecipient { get; set; }
        public int IsThereAttachment { get; set; }
        public string AttachmentLocation { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public int Channel { get; set; }
        public int SenderIDAccount { get; set; }
    }
}