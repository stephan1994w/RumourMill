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
    
    public partial class Reply
    {
        public int ReplyID { get; set; }
        public int fk_QuestionId { get; set; }
        public int fk_LeaderId { get; set; }
        public string ReplyText { get; set; }
    }
}
