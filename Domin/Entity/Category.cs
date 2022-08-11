using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required(ErrorMessageResourceType =typeof(Resoures.ResourceData),ErrorMessageResourceName ="CategoryName")]
        [MaxLength(20,ErrorMessageResourceType = typeof(Resoures.ResourceData), ErrorMessageResourceName = "MaxLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Resoures.ResourceData), ErrorMessageResourceName = "MinLength")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentStaut { get; set; }
    }
}
