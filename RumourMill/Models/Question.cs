//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RumourMill.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Question
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Status { get; set; }
        public bool IsAnswered { get; set; }
        public Nullable<System.DateTime> TimeAsked { get; set; }
        public string TimeAgo { get; set; }
        public string EditReason { get; set; }
    }
}
