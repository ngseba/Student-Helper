using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class GroupEvent
    {
        public int GroupEventID { get; set; }
        public int EventID { get; set; }
        public virtual Event Event { get; set; }
        public int GroupID { get; set; }
        public virtual Group Group { get; set; }
    }
}