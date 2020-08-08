using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlightReservationSystem.Models;

namespace FlightReservationSystem.Controllers
{
    public class TicketsController : Controller
    {
        private FlightReservationSystemContext db = new FlightReservationSystemContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Flight).Include(t => t.Schedule);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Reservations/Book
        public ActionResult Book(int sid, int cid, DateTime doj, string name, string Tclass)
        {
            Ticket model = new Ticket()
            {
                CustomerId = cid,
                scheduleId = sid,
                dateOfJourney = doj.Date, 
                travelclass = Tclass
            };
            ViewBag.flightname = name;
         
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteBooking([Bind(Include = "paymentMode,totalAmount,bankDetails")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                db.SaveChanges();
            }

            Ticket ticket = TempData["Ticket"] as Ticket;
            ticket.UserId = User.Identity.Name;
            ticket.CustomerId = payment.paymentId;

            db.Tickets.Add(ticket);
            db.SaveChanges();

            Schedule schedule = db.Schedules.Find(ticket.scheduleId);
            if (ticket.travelclass.Contains("First"))
            {
                schedule.FCseats--;
            }
            else if (ticket.travelclass.Contains("Second"))
            {
                schedule.SCseats--;
            }
            else
            {
                schedule.TCseats--;
            }
            db.Entry(schedule).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment([Bind(Include = "flightId,scheduleId,dateOfJourney,passengerName,gender,phoneNumber,address,emergencyContact,travelclass")] Ticket ticket)
        {
            TempData["Ticket"] = ticket;
            ticket.seatNo = db.Tickets.Include(t => t.ticketId).Where(t => t.travelclass == ticket.travelclass).Count() + 1;
            Payment payment = new Payment()
            {
                totalAmount = getcostofticket(ticket.ticketId, ticket.travelclass)
            };

            return View(payment);
        }

        private double getcostofticket(int scheduleId, string travelclass)
        {
            double cost = 0.00;
            Schedule s = new Schedule();
            var query = "SELECT cost" + travelclass + " FROM Schedule where scheduleId=" + scheduleId;
            string cString = ConfigurationManager.ConnectionStrings["FlightReservationSystemContext"].ConnectionString;
            using (SqlConnection c = new SqlConnection(cString))
            {
                c.Open();
                using (SqlCommand cmd = new SqlCommand(query, c))
                {
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            cost = rdr.GetDouble(rdr.GetOrdinal("cost" + travelclass));
                        }
                    }
                }
            }
            return (cost);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("CancelTicket")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            Schedule schedule = db.Schedules.Find(ticket.scheduleId);
            if (ticket.travelclass.Contains("First"))
            {
                schedule.FCseats++;
            }
            else if (ticket.travelclass.Contains("Second"))
            {
                schedule.SCseats++; 
            }
            else
            {
                schedule.TCseats++;
            }
            db.Entry(schedule).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Flights, "CustomerId", "firstName");
            ViewBag.scheduleId = new SelectList(db.Schedules, "scheduleId", "cityDep");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ticketId,UserId,CustomerId,scheduleId,dateOfJourney,seatNo,passengerName,phoneNumber,address,travelclass")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Flights, "CustomerId", "firstName", ticket.CustomerId);
            ViewBag.scheduleId = new SelectList(db.Schedules, "scheduleId", "cityDep", ticket.scheduleId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Flights, "CustomerId", "firstName", ticket.CustomerId);
            ViewBag.scheduleId = new SelectList(db.Schedules, "scheduleId", "cityDep", ticket.scheduleId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ticketId,UserId,CustomerId,scheduleId,dateOfJourney,seatNo,passengerName,phoneNumber,address,travelclass")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Flights, "CustomerId", "firstName", ticket.CustomerId);
            ViewBag.scheduleId = new SelectList(db.Schedules, "scheduleId", "cityDep", ticket.scheduleId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
