using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models.DTOs
{
    public class SequanceWithPackageDto
    {
        [Required]
        [StringLength(100)]
        public string SequanceName { get; set; }

        [StringLength(250)]
        public string SequanceDescription { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string NameOfPackage { get; set; }   
    }
}
