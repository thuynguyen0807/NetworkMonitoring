using Microsoft.AspNetCore.Mvc;
using NetworkMonitor.Data;
using NetworkMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkMonitor.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DeviceController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Devices> objList = _db.Devices;
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
        public IActionResult Create(Devices obj)
        {
            if (ModelState.IsValid)
            {
                _db.Devices.Add(obj);
                _db.SaveChanges();
                ViewBag.message = "Added device successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.message = "Add device fail";
            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Devices.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Devices obj)
        {
            if (ModelState.IsValid)
            {
                _db.Devices.Update(obj);
                _db.SaveChanges();
                ViewBag.message = "Updated device successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
            ViewBag.message = "Update device fail";
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _db.Devices.Find(id);
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
            var obj = _db.Devices.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Devices.Remove(obj);
            _db.SaveChanges();
            ViewBag.message = "Deleted device successfully!";
            return RedirectToAction("Index");
        }
    }
}
