using GeoLocate.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GeoLocate.Internal
{
    public class GeoLocateDataContext : GeoLocateEntityContext
    {
        private ApplicationUser _currentUser;
        private ApplicationUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    _currentUser = manager.FindByName(HttpContext.Current.User.Identity.Name);
                }
                return _currentUser;
            }
        }

        public void NewUserCoord(UserCoord userCoord)
        {
            if (userCoord != null)
            {
                userCoord.UserID = CurrentUser.Id;

                Context.UserCoords.Add(userCoord);
                Context.SaveChanges();
            }
        }

        public void NewUserRoute(GeoLocate.UserRoute userRoute)
        {
            if (userRoute != null)
            {
                userRoute.UserID = CurrentUser.Id;
                userRoute.Timestamp = DateTime.Now;

                Context.UserRoutes.Add(userRoute);
                Context.SaveChanges();
            }
        }

        public void NewUserRoutePoint(GeoLocate.UserRoute userRoute, List<GeoLocate.UserCoord> coordList)
        {
            if (userRoute != null && coordList != null && coordList.Count > 0)
            {
                foreach (GeoLocate.UserCoord coord in coordList)
                {
                    UserRouteCoord routePoint = new UserRouteCoord()
                    {
                        CoordId = coord.ID,
                        Timestamp = DateTime.Now,
                        UserRouteId = userRoute.ID
                    };
                    Context.UserRouteCoords.Add(routePoint);
                    Context.SaveChanges();
                }
            }
        }

    }


    public partial class GeoLocateEntityContext : IDisposable
    {
        #region Locals

        private GeoLocateEntities context;
        public GeoLocateEntities Context
        {
            get
            {
                if (context == null)
                {
                    context = new GeoLocateEntities();
                }
                return context;
            }
        }

        public void Dispose()
        {
            if (this.Context != null)
            {
                this.context.Dispose();
                this.context = null;
            }

        }

        #endregion Locals
    }
}