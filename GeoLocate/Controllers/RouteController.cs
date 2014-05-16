using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeoLocate;
using GeoLocate.Models;
using System.Diagnostics;
using System.Globalization;

namespace GeoLocate.Controllers
{
    public class RouteController : BaseController
    {
        private GeoLocateEntities db = new GeoLocateEntities();

        // GET: /Route/
        public async Task<ActionResult> Index()
        {
            var userroutes = db.UserRoutes.Include(u => u.AspNetUser);
            return View(await userroutes.ToListAsync());
        }

        // GET: /Route/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoute userroute = await db.UserRoutes.FindAsync(id);
            if (userroute == null)
            {
                return HttpNotFound();
            }
            return View(userroute);
        }

        // GET: /Route/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: /Route/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,UserID,Name,Description,Timestamp")] UserRoute userroute)
        {
            if (ModelState.IsValid)
            {
                db.UserRoutes.Add(userroute);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName", userroute.UserID);
            return View(userroute);
        }

        // GET: /Route/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoute userroute = await db.UserRoutes.FindAsync(id);
            if (userroute == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName", userroute.UserID);
            return View(userroute);
        }

        // POST: /Route/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UserID,Name,Description,Timestamp")] UserRoute userroute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userroute).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName", userroute.UserID);
            return View(userroute);
        }

        // GET: /Route/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserRoute userroute = await db.UserRoutes.FindAsync(id);
            if (userroute == null)
            {
                return HttpNotFound();
            }
            return View(userroute);
        }

        // POST: /Route/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            UserRoute userroute = await db.UserRoutes.FindAsync(id);
            db.UserRoutes.Remove(userroute);
            await db.SaveChangesAsync();
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


        public ActionResult RoutePoints(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Check / Change defaults
            // Force NumberDecimalSeparator to point for Coord parsing
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            UserCoord model = new UserCoord();
            try
            {
                var qUserCoords = (from t in db.UserCoords
                                   join i in db.UserRouteCoords on t.ID equals i.CoordId
                                   where i.UserRouteId == id
                                   select t).ToList();

                if (qUserCoords == null)
                {
                    return HttpNotFound();
                }

                return View(qUserCoords);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                ModelState.AddModelError("", ex.ToString());
            }
            return View(model);
        }


    }
}
