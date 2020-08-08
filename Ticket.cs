using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace FlightReservationSystem.Models
{
    [Table("Ticket")]
    public class Ticket
    {
        [Key]
        public int ticketId { get; set; }

        // [ForeignKey("UserId")]
        public string  UserId { get; set; }
        //public virtual ApplicationUser User { get; set; }

        [ForeignKey("Flight")]
        [Display(Name = "Passanger ID")]
        public int ? CustomerId { get; set; }

        [ForeignKey("Schedule")]
        public int ? scheduleId { get; set; }

        [Display(Name ="Flight TakeOf Date")]
        [DataType(DataType.Date)]
        public DateTime dateOfJourney { get; set; }

        [Display(Name = "Seat Number")]
        public int seatNo { get; set; }

        [Display(Name = "Passanger Name")]
        public string passengerName { get; set; }

        public char gender { get; set; }

        [Display(Name = "Mobile Number")]
        [Phone]
        public string phoneNumber { get; set; }

        [Display(Name = "Public Address")]
        public string address { get; set; }

        [Display(Name = "Ticket Class")]
        public string travelclass { get; set; }

        public virtual Flight Flight { get; set; }
        public virtual Schedule Schedule { get; set; }
    }
}