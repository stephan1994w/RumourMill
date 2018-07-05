using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RumourMill.Models
{
    public class QuestionReply
    {
        public int qID { get; set; }
        public string qText { get; set; }
        public List<string> rTexts{ get; set; }
    }
}