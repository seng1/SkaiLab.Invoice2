using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public interface IAppResource
    {
        string GetResource(string key);
    }
}
