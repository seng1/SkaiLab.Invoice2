using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SkaiLab.Invoice.Dal.Models;
using SkaiLab.Invoice.Models;

namespace SkaiLab.Invoice.Service
{
    public class DataContext : IDataContext
    {
        public DataContext(IOptions<Option> option)
        {
            this.Option = option.Value;
        }
        public Option Option { get; }

        public InvoiceContext Context()
        {
            var optionsBuilder = new DbContextOptionsBuilder<InvoiceContext>();
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer(Option.ConnectionString);
            return new InvoiceContext(optionsBuilder.Options);
        }
    }
}
