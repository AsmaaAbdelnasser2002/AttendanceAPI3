using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models.DTOs
{
    public class SessionWithSequanceDto
    {
        [Required]
        [StringLength(100)]
        public string SessionName { get; set; }

        [StringLength(255)]
        public string SessionPlace { get; set; }

        [StringLength(250)]
        public string SessionDescription { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string NameOfSequance { get; set; }
    }
}
