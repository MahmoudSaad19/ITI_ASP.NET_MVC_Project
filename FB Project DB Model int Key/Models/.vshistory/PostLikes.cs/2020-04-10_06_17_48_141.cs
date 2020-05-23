using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_DB_Model_int_Key.Models
{
    public class PostLikes
    {
        [Column("PostId")]
        [Key]
        public int PostId { get; set; }

        [Column("UserId")]
//#warning required
        public int UserId { get; set; }    

        [ForeignKey(nameof(UserId))]
//#warning add navigational property 
        public virtual User User { get; set; }

        [ForeignKey(nameof(PostId))]
//#warning add navigational property 
        public virtual Post post { get; set; }
    }
}
