using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AttendanceAPI3.Models.DTOs
{
    public class PackageDto
    {
        [Required]
        [StringLength(100)]
        public string PackageName { get; set; }

        [StringLength(250)]
        public string PackageDescription { get; set; }

        public IFormFile Sheet { get; set; }
        
        public IFormFile? FacesFolder { get; set; }

        public IFormFile? VoicesFolder { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
        
        //public int User_Id { get; set; }
    }
}
