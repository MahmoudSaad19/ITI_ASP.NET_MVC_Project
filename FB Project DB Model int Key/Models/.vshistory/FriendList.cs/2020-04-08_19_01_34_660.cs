using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_DB_Model.Models
{
    public class FriendList
    {
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("FriendId")]
        public int FriendId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(FriendId))]
        public virtual User Friend { get; set; }
    }
}
