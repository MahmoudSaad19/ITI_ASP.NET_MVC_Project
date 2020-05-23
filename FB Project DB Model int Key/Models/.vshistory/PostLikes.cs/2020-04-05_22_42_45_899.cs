using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_V1._0.Models
{
    public class PostLikes
    {
        [Column("PostId")]
        [Key]
        public int PostId { get; set; }

        [StringLength(450)]
        [Column("UserId")]
        public string UserId { get; set; }    

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual User Friend { get; set; }
    }
}
