using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models.DTOs
{
    public class PackegeListDto
    {
        [Required]
        [StringLength(100)]
        public string PackageName { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string creator { get; set; }
    }
}
