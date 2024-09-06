using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceAPI3.Models.DTOs
{
    public class SequanceDataDto
    {
       
        public string SequanceName { get; set; }

        
        public string SequanceDescription { get; set; }

       
        public DateTime StartTime { get; set; }

        
        public DateTime EndTime { get; set; }

        public string creator { get; set; }


        public string ExcelSheetUrl { get; set; }
        public string FacesFolderUrl { get; set; }
        public string VoicesFolderUrl { get; set; }

    }
}
