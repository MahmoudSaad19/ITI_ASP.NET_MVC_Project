using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_V1._0.Models
{
    public class Comment
    {
        [Key]
        public int CID { get; set; }

        [Column("PostId")]
        public int PostId { get; set; }

        [StringLength(450)]
        [Column("UserId")]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Comments")]
        public virtual User User { get; set; }

        [ForeignKey(nameof(PostId))]
        [InverseProperty("Comments")]
        public virtual Post Post { get; set; }
    }
}
