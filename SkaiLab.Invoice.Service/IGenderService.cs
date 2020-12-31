using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface IGenderService:IService
    {
        List<Models.Gender> GetGenders();
    }
}
