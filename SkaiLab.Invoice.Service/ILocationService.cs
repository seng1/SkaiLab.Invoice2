using SkaiLab.Invoice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public interface ILocationService:IService
    {
        void Create(string organisationId, Location location);
        void Update(string organisationId, Location location);
        Location GetLocation(string organisationId, long id);
        List<Location> GetLocations(string organisationId);
    }
}
