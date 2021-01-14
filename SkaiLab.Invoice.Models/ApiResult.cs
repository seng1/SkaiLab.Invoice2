using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class ApiResult
    {
        public int Code { get; set; }
        public string ErrorText { get; set; }
        public bool IsSccuess { get; set; }
        public string UserId { get; set; }
    }
}
