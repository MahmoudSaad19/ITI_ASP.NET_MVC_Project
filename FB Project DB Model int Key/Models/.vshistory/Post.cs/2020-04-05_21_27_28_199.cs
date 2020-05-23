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
        public int PostId { get; set; }

        [StringLength(450)]
        [Column("UserId")]
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string BodyText { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Posts")]
        public virtual User User { get; set; }
    }
}
