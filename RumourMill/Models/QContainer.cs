using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RumourMill.Models
{
    public class QContainer
    {
        public List<QuestionReplyViewModel> qrvmodel { get; set; }
        public ArrayList questionCount { get; set; }
    }
}