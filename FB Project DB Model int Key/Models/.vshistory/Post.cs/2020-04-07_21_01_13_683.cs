using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_V1._0.Models
{
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
//#warning extra statment need to be removed
//            Comments = new HashSet<Comment>();
        }

        [Key]
        public int PostID { get; set; }

        [StringLength(450)]
        [Column("UserId")]
        [Required]
//#warning add required attribute
        public string UserId { get; set; }
        public DateTime Date { get; set; }
//#warning add required attribute
        [Required]
        public string BodyText { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Posts")]
        public virtual User User { get; set; }

        [InverseProperty("Post")]
        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("Post")]
        public virtual ICollection<PostLikes> PostLikes { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
