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
        private static string RouteMessage = "RouteMessage";

        public ActionResult Index()
        {
            GeoLocate.Models.UserRoute model = new Models.UserRoute();

            ViewBag.Message = TempData[RouteMessage];

            if (!string.IsNullOrEmpty(ViewBag.Message))
            {
                TempData[RouteMessage] = string.Empty;
            }
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
                                List<UserCoord> currentList = CurrentRouteList;
                                if (currentList == null)
                                {
                                    currentList = new List<UserCoord>();
                                }
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

                        TempData[RouteMessage] = string.Format("Route {0} Saved Successfully !", userRoute.Name);

                        CurrentRouteList = null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());

                    ModelState.AddModelError("", ex.ToString());
                    TempData[RouteMessage] = ex.ToString();
                }
            }
            return RedirectToAction("Index", "Home");
        } // ActionResult SaveUserRoute(UserRoute userRoute)

        #region Private Methods

        private List<UserCoord> _currentRouteList;
        private List<UserCoord> CurrentRouteList
        {
            get
            {
                var cache = System.Web.HttpContext.Current.Cache;

                if (cache[UserRouteItems] != null)
                {
                    _currentRouteList = cache[UserRouteItems] as List<UserCoord>;
                }
                return _currentRouteList;
            }
            set
            {
                var cache = System.Web.HttpContext.Current.Cache;

                cache[UserRouteItems] = value;
            }
        }

        #endregion Private Methods
    }
}