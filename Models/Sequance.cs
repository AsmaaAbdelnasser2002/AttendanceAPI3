using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models
{
    public class Sequance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SequanceId { get; set; }

        [Required]
        [StringLength(100)]
        public string SequanceName { get; set; }

        [StringLength(250)]
        public string SequanceDescription { get; set; }
        
        public byte[] Sheet { get; set; }

        public byte[]? FacesFolder { get; set; }

        public byte[]? VoicesFolder { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [ForeignKey("User")]
        public string User_Id { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Package")]
        public int? Package_Id { get; set; }
        public virtual Package? Package { get; set; }
        public virtual List<Session>? Session { get; set; }
    }
}
