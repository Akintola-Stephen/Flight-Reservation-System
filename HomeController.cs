using FlightReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlightReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private FlightReservationSystemContext db = new FlightReservationSystemContext();
        public ActionResult Index()
        {

            var srclist = new List<Schedule>();
            var destlist = new List<Schedule>();
            string cString = ConfigurationManager.ConnectionStrings["FlightReservationSystemContext"].ConnectionString;
            using (SqlConnection c = new SqlConnection(cString))
            {
                c.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT cityDep FROM Schedule", c))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            srclist.Add(new Schedule
                            {
                                cityDep = rdr.GetString(0)
                            });
                        }
                    }
                }
            }

            ViewBag.source = new SelectList(srclist, "cityDep", "cityDep");
            ViewBag.date = new DateTime();
            return View();
        }

        [HttpPost]
        public ActionResult SearchResults(string source, string dest, DateTime dateOfJourney)
        {
            ViewBag.Source = source;
            ViewBag.Dest = dateOfJourney;
            ViewBag.ScheduleMessage = "";
            if (DateTime.Compare(dateOfJourney, DateTime.Today) > 0)
            {
                var data = from s in db.Schedules
                           where s.cityDep == source && s.depatureDate == dateOfJourney
                           select s;
                if (data.ToList().Count() == 0)
                {
                    ViewBag.ScheduleMessage = "No flights on the entered date, below are the flights from other days";
                    data = from s in db.Schedules
                           where s.cityDep == source  && DateTime.Compare(s.depatureDate, DateTime.Today) > 0
                           select s;
                }
                return View(data.ToList());
            }
            else
            {
                var data = from s in db.Schedules
                           where s.cityDep == source  && DateTime.Compare(s.depatureDate, DateTime.Today) >= 0
                           select s;
                if (dateOfJourney.CompareTo(DateTime.Today) == 0)
                    ViewBag.ScheduleMessage = "Flights Can't be booked  for today.";
                else
                    ViewBag.ScheduleMessage = "Entered past date, flights from requested source to destination are listed below";
                return View(data.ToList());

            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}