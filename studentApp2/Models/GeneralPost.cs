using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class GeneralPost
    {
        public int GeneralPostID { get; set; }
        public int PostID { get; set; }
        public virtual Post Post { get; set; }
    }
}