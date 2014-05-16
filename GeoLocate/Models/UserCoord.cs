using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace GeoLocate.Models
{
    public class UserCoordJson
    {
        public float Long { get; set; }
        public float Lat { get; set; }
        public float Accuracy { get; set; }
        public float Alt { get; set; }
        public float Heading { get; set; }
        public float Speed { get; set; }
        public long Timestamp { get; set; }
        public string CreatedDate { get; set; }
    }
}
