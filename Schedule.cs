using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace FlightReservationSystem.Models
{
    [Table("Schedule")]
    public class Schedule
    {
        [Key]
        public int scheduleId { get; set; }
        [ForeignKey("Flight")]
        public int ? CustomerId { get; set; }

        [Display(Name = "Departing City")]
        public string cityDep { get; set; }

        public int cityDes { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Departure Date")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime depatureDate { get; set; }

        [Display(Name = "Departure Time")]
        public TimeSpan depatureTime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Arrival Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime arrivalDate { get; set; }

        [Display(Name = "Arrival Time")]
        public TimeSpan arrivalTime { get; set; }

        [Display(Name = "Ticket Price")]
        public int price { get; set; }

        [Display(Name = "1st Class")]
        public int FCseats { get; set; }

        [Display(Name = "2nd Class")]
        public int SCseats { get; set; }

        [Display(Name = "Third Class")]
        public int TCseats { get; set; } 

        public virtual Flight Flight { get; set; }
    }
}