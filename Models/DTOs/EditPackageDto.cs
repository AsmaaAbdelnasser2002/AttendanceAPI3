using System.ComponentModel.DataAnnotations;

namespace AttendanceAPI3.Models.DTOs
{
    public class EditPackageDto
    {
        [Required]
        [StringLength(100)]
        public string PackageName { get; set; }

        [StringLength(250)]
        public string PackageDescription { get; set; }

        public IFormFile? Sheet { get; set; }

        public IFormFile? FacesFolder { get; set; }

        public IFormFile? VoicesFolder { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
