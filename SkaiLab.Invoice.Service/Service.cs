using System;
using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public class Service : IService
    {
        private readonly IDataContext dataContext;
        public Service(IDataContext context)
        {
            this.dataContext = context;
        }
        protected InvoiceContext Context()
        {
            return dataContext.Context();
        }

        public Option Option => dataContext.Option;

    }
}
