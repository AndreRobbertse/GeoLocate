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
        private static string UserRouteItems = "UserRouteItems";

        public ActionResult Index()
        {
            GeoLocate.Models.UserRoute model = new Models.UserRoute();
            return View(model);
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

                                // Update Session List
                                var currentList = CurrentRouteList;
                                currentList.Add(usrCoord);
                                CurrentRouteList = currentList;
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

        }// ActionResult SaveUserCoord(UserCoordJson userCoord)

        public ActionResult SaveUserRoute(UserRoute userRoute)
        {
            if (userRoute != null && !string.IsNullOrEmpty(userRoute.Name))
            {
                try
                {
                    var currentList = CurrentRouteList;

                    using (var context = new GeoLocateDataContext())
                    {
                        // Save Route
                        context.NewUserRoute(userRoute);

                        // Save Route Points
                        context.NewUserRoutePoint(userRoute, currentList);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    ModelState.AddModelError("", ex.ToString());
                }
            }
            return RedirectToAction("Index", "Home");
        } // ActionResult SaveUserRoute(UserRoute userRoute)

        #region Private Methods

        private List<UserCoord> CurrentRouteList
        {
            get
            {
                List<UserCoord> listFromSession = new List<UserCoord>();
                if (Session[UserRouteItems] != null)
                {
                    try
                    {
                        listFromSession = (List<UserCoord>)Session[UserRouteItems];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
                return listFromSession;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    Session[UserRouteItems] = value;
                }
            }
        }

        #endregion Private Methods
    }
}