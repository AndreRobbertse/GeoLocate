using GeoLocate.Internal;
using GeoLocate.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeoLocate.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveUserCoord(UserCoordJson userCoord)
        {
            if (userCoord != null)
            {
                try
                {
                    if (userCoord.Timestamp > 0)
                    {
                        DateTime coordDate = new DateTime(0001, 1, 1, 0, 0, 0);
                        DateTime.TryParse(userCoord.CreatedDate, out coordDate);

                        //string dateformat = "yyyy-MM-dd h:mm:ss";
                        //DateTime.TryParseExact(userCoord.CreatedDate, dateformat, null, System.Globalization.DateTimeStyles.AssumeLocal, out coordDate);
                        if (coordDate.Year > 1)
                        {
                            using (var context = new GeoLocateDataContext())
                            {
                                UserCoord usrCoord = new UserCoord()
                                {
                                    Accuracy = userCoord.Accuracy,
                                    Altitude = userCoord.Alt,
                                    Heading = userCoord.Heading,
                                    Latitude = userCoord.Lat,
                                    Longitude = userCoord.Long,
                                    Speed = userCoord.Speed,
                                    Timestamp = coordDate
                                };
                                context.NewUserCoord(usrCoord);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    ModelState.AddModelError("", ex.ToString());
                }
            }
            return View();
            
        }

    }
}