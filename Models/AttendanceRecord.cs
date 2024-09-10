﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace AttendanceAPI3.Models
{
    public class AttendanceRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }

        [Required]
        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^(Present|Absent)$", ErrorMessage = "Invalid Status. Valid Status are 'Present', 'Absent'.")]

        public string Status { get; set; }

        [ForeignKey("User")]
        public string User_Id { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Session")]
        public int Session_Id { get; set; }
        public virtual Session Session { get; set; }
    }
}
