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
    
    public partial class Log
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Reason { get; set; }
        public string User { get; set; }
        public System.DateTime TimeEdited { get; set; }
        public string OldText { get; set; }
        public string NewText { get; set; }
    }
}
