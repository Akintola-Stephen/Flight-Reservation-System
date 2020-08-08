using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FlightReservationSystem.Models
{
    [Table("Flight")]
    public class Flight
    {
        [Key]
        public int  CustomerId { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Mobile Number")]
        public string telephone { get; set; }

        [Display(Name = "Home Address")]
        public string address { get; set; }

        public virtual ICollection<Schedule> Schedule { get; set; }
    }
}