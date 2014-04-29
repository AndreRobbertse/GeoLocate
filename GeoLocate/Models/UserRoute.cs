using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeoLocate.Models
{
    public class UserRoute
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Route Name minimum of 3 and Max 250 characters.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

    }
}