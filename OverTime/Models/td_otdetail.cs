//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OverTime.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class td_otdetail
    {
        public long id { get; set; }
        public string empcode { get; set; }
        public System.DateTime date { get; set; }
        public Nullable<double> ot_normal { get; set; }
        public Nullable<double> ot_holiday { get; set; }
        public string time { get; set; }
        public string ot_description { get; set; }
        public int group_id { get; set; }
    }
}