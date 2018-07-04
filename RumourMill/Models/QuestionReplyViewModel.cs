using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RumourMill.Models
{
    public class QuestionReplyViewModel
    {
        public string ReplyText { get; set; }
        public string LeaderName { get; set; }
        public string QuestionText { get; set; }
        public int QuestionId { get; set; }
        public DateTime TimeAsked { get; set; }
        public bool isApproved { get; set; }
        public bool isAnswered { get; set; }
    }
}