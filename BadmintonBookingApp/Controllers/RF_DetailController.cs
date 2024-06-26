﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BadmintonBookingApp.Data;
using BadmintonBookingApp.Models.Reservations;
using BadmintonBookingApp.Repositories;

namespace BadmintonBookingApp.Controllers
{
    public class RF_DetailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReservation _eFReservation;
        DateTime b = ReservationsController.b;
        DateTime s = ReservationsController.s;
        DateTime e = ReservationsController.e;
        //int CurrentRev = ReservationsController.CurrentRev;
        public static List<RF_Detail> listRFD = new List<RF_Detail>();
        public RF_DetailController(ApplicationDbContext context, IReservation eFReservation)
        {
            _context = context;
            _eFReservation = eFReservation;
        }

        // GET: RF_Detail
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RF_Details.Include(r => r.Court).Include(r => r.Reservation);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult RFD()
        {
            
            ViewBag.Court = new SelectList(_eFReservation.GetAllValidCourt(b, s, e), "Id", "CourtName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RFD([Bind("Note,ReservationId,CourtId")] RF_Detail rf)
        {

            if (ModelState.IsValid)
            {
                //rf.Court = _context.Courts.FirstOrDefault(p => p.Id == rf.CourtId);
                listRFD.Add(rf);
                /* var listRFD = new List<RF_Detail>();
                listRFD.Add(rf);
                if (listRFD.Count > 3) ;*/
                //_context.RF_Details.Add(rf);
                //await _context.SaveChangesAsync();
                ViewBag.Court = new SelectList(_eFReservation.GetAllValidCourt(b, s, e), "Id", "CourtName");
                return View(rf);
            }
          
            ViewBag.Court = new SelectList(_eFReservation.GetAllValidCourt(b, s, e), "Id", "CourtName");
            return RedirectToAction("RFD");
        }
        public async Task<IActionResult> Finish()
        {
            //var rf = _context.RF_Details.Where(p => p.ReservationId == CurrentRev).Include(p => p.Court).ToList();
            DateTime d = DateTime.Now;
            ReservationsController.tempRev.CreateDate = d;
            _context.Reservations.Add(ReservationsController.tempRev);
            await _context.SaveChangesAsync();
            int id = ReservationsController.tempRev.Id;
            ReservationsController.tempRev = null;
            foreach (var item in listRFD)
            {
                item.ReservationId = id;
                item.Court = null;
                _context.RF_Details.Add(item);
                await _context.SaveChangesAsync();
            }
            RF_DetailController.listRFD.Clear();
            ViewBag.CD = d.ToString("dd/MM/yy HH:mm:ss");
            ViewBag.BD = ReservationsController.b.ToString("dd/MM/yy");
            ViewBag.ST = ReservationsController.s.ToString("HH:mm");
            ViewBag.ED = ReservationsController.e.ToString("HH:mm");
            List<RF_Detail> showRFD = _context.RF_Details.Include(p=>p.Court).Where(p=>p.ReservationId == id).ToList();
            return View(showRFD);
        }

        // GET: RF_Detail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rF_Detail = await _context.RF_Details
                .Include(r => r.Court)
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rF_Detail == null)
            {
                return NotFound();
            }

            return View(rF_Detail);
        }

        // GET: RF_Detail/Create
        public IActionResult Create()
        {
            ViewData["CourtId"] = new SelectList(_context.Courts, "Id", "Id");
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id");
            return View();
        }

        // POST: RF_Detail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Note,ReservationId,CourtId")] RF_Detail rF_Detail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rF_Detail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourtId"] = new SelectList(_context.Courts, "Id", "Id", rF_Detail.CourtId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", rF_Detail.ReservationId);
            return View(rF_Detail);
        }

        // GET: RF_Detail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rF_Detail = await _context.RF_Details.FindAsync(id);
            if (rF_Detail == null)
            {
                return NotFound();
            }
            ViewData["CourtId"] = new SelectList(_context.Courts, "Id", "Id", rF_Detail.CourtId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", rF_Detail.ReservationId);
            return View(rF_Detail);
        }

        // POST: RF_Detail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Note,ReservationId,CourtId")] RF_Detail rF_Detail)
        {
            if (id != rF_Detail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rF_Detail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RF_DetailExists(rF_Detail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourtId"] = new SelectList(_context.Courts, "Id", "Id", rF_Detail.CourtId);
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", rF_Detail.ReservationId);
            return View(rF_Detail);
        }

        // GET: RF_Detail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rF_Detail = await _context.RF_Details
                .Include(r => r.Court)
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rF_Detail == null)
            {
                return NotFound();
            }

            return View(rF_Detail);
        }

        // POST: RF_Detail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rF_Detail = await _context.RF_Details.FindAsync(id);
            if (rF_Detail != null)
            {
                _context.RF_Details.Remove(rF_Detail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RF_DetailExists(int id)
        {
            return _context.RF_Details.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Remove(int Id)
        {
            //var rf = _context.RF_Details.FirstOrDefault(e => e.Id == Id);
            //_context.RF_Details.Remove(rf);
            //await _context.SaveChangesAsync();
            List<RF_Detail> temp = listRFD;
            foreach(var item in temp)
            {
                if(item.CourtId == Id)
                {
                    listRFD.Remove(item);
                    break;
                }
            }
            return RedirectToAction("RFD");
        }
    }
}
