using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System.Collections.Generic;

namespace SkaiLab.Invoice.Service
{
    public interface IProductService : IService
    {
        void GetTotalPages(ProductFilter filter);
        List<Product> GetProducts(ProductFilter filter);
        Product GetProduct(long id, string organisationId);
        void Create(Product product,string createdBy);
        void Update(Product product);
        List<Product> GetProductsForPurchase(string organisationId);
        List<Product> GetProductsForSale(string organisationId);
        int GetInventoryQty(long productId, long locationId, string organisationId);
    }
}
