//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeoLocate
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRouteCoord
    {
        public long UserRouteId { get; set; }
        public long CoordId { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        public virtual UserCoord UserCoord { get; set; }
        public virtual UserRoute UserRoute { get; set; }
    }
}
