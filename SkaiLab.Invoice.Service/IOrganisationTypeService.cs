using System;
using System.Collections.Generic;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public interface IOrganisationTypeService:IService
    {
        List<OrganisationType> Gets();
       
    }
}
