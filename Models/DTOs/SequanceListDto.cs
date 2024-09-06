using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models.DTOs
{
    public class SequanceListDto
    {
        [Required]
        [StringLength(100)]
        public string SequanceName { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string creator { get; set; }
    }
}
