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
        [Key, Column(Order = 0)]
        public int PostId { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }    

        [ForeignKey(nameof(UserId))]
//#warning add navigational property 
        public virtual User User { get; set; }

        [ForeignKey(nameof(PostId))]
//#warning add navigational property 
        public virtual Post post { get; set; }
    }
}
