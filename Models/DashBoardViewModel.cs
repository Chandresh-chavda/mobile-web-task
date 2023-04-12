using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DashBoardViewModel
    {
        [ValidateNever]
        public IEnumerable<Dashboard> dashboards { get; set; }
        public string? SearchItem { get; set; }
    }
}
