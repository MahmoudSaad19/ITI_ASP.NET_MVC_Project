using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FB_Project_DB_Model_int_Key.Models
{
    public class FriendRequest
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int ReqId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("FriendRequests")]
        public virtual User User { get; set; }

        [ForeignKey(nameof(ReqId))]
        public virtual User reqFriend { get; set; }
    }
}
