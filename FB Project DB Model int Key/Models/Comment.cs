using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_DB_Model_int_Key.Models
{
    public class Comment
    {
        [Key]
        public int CID { get; set; }

        [Column("PostId")]
        public int PostId { get; set; }

        [Column("UserId")]
//#warning required
        public int UserId { get; set; }
//#warning add comment body and make it required
        [Required]
        public string BodyText { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Comments")]
        public virtual User User { get; set; }

        [ForeignKey(nameof(PostId))]
        [InverseProperty("Comments")]
        public virtual Post Post { get; set; }
    }
}
