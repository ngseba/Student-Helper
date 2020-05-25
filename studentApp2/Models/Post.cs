using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace studentApp2.Models
{
    public class Post
    {
        public int PostID { get; set; }
        public string PostTitle { get; set; }
        public string PostBody { get; set; }
        public PostType PostType { get; set; }

        public GeneralPost GeneralPost;
        public YearPost YearPost;
        public GroupPost GroupPost;

    }

    public class PostViewModel
    {
        public Post Post { get; set; }
        public int PostYear { get; set; }
        public PostType PostType { get; set; }
        public int GroupID { get; set; }

        public string GroupName { get; set; }

    }

    public enum PostType
    {
        General,
        Year,
        Group
    }
}