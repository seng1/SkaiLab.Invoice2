using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Models
{
    public class SharedResources : IAppResource
    {
        private IStringLocalizer<SharedResources> _localizer;
        public SharedResources(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        public string GetResource(string key)
        {
            return _localizer[key];
        }
    }
}
