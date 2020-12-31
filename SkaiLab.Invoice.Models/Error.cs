using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class Error
    {
        public string ErrorText { get; set; }
        public Error()
        {

        }
        public Error(string errorText)
        {
            this.ErrorText = errorText;
        }
    }
}
