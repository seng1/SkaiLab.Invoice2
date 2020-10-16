using System;
using SkaiLab.Invoice.Dal.Models;

namespace SkaiLab.Invoice.Service
{
    public interface IDataContext
    {
        InvoiceContext Context();
        Models.Option Option { get; }
    }
}
