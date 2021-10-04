using System;
using System.Collections.Generic;

#nullable disable

namespace Imagegram.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Caption { get; set; }
        public string Image { get; set; }
        public int CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Account Creator { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
