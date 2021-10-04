using System;
using System.Collections.Generic;

#nullable disable

namespace Imagegram.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Account Creator { get; set; }
        public virtual Post Post { get; set; }
    }
}
