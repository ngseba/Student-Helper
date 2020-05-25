using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class GroupPost
    {
        public int GroupPostID { get; set; }
        public int PostID { get; set; }
        public virtual Post Post { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}