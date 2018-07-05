using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RumourMill.Models
{
    public class QuestionReplyViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAnswered { get; set; }
        public DateTime TimeAsked { get; set; }

        public int ReplyID { get; set; }
        public int fk_QuestionId { get; set; }
        public int fk_LeaderId { get; set; }
        public string ReplyText { get; set; }
        public DateTime TimeReplied { get; set; }
         
        public int LeaderId { get; set; }
        public string LeaderName { get; set; }
        public string Image { get; set; }

    }
}