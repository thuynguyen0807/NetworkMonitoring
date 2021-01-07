using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkMonitor.Data;
using NetworkMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Controllers
{
    public class OIDsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OIDsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<OIDs> objList = _db.OIDs;
            return View(objList);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OIDs obj)
        {
            if (ModelState.IsValid)
            {
                _db.OIDs.Add(obj);
                _db.SaveChanges();
                ViewBag.message = "Added OID successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.message = "Add OID fail";
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.OIDs.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OIDs obj)
        {
            if (ModelState.IsValid)
            {
                _db.OIDs.Update(obj);
                _db.SaveChanges();
                ViewBag.message = "Updated OID successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
            ViewBag.message = "Update OID fail";
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.OIDs.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST - DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int? id)
        {
            var obj = _db.OIDs.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.OIDs.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
