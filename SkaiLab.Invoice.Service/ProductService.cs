using SkaiLab.Invoice.Models;
using SkaiLab.Invoice.Models.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class ProductService:Service,IProductService
    {
        public ProductService(IDataContext context) : base(context) { }

        public void Create(Product product,string createdBy)
        {
            using var context = Context();
            if(context.Product.Any(u=>u.Code==product.Code && u.OrganisationId == product.OrganisationId))
            {
                throw new Exception("Product code already exist");
            }
            if (string.IsNullOrEmpty(product.Name))
            {
                throw new Exception("Product name is require");
            }
            if (string.IsNullOrEmpty(product.ProductPurchaseInformation.Description))
            {
                throw new Exception("Purchase description is require");
            }
            if (string.IsNullOrEmpty(product.ProductSaleInformation.Description))
            {
                throw new Exception("Sale description is require");
            }
            product.ImageUrl =string.IsNullOrEmpty(product.ImageUrl)?Option.NoProductImageUrl: UploadFile(product.ImageUrl);
            var productDb = new Dal.Models.Product
            {
                Code=product.Code,
                Name=product.Name,
                OrganisationId=product.OrganisationId,
                ImageUrl=product.ImageUrl,
                ProductPurchaseInformation=new Dal.Models.ProductPurchaseInformation
                {
                    Description=product.ProductPurchaseInformation.Description,
                    Price=product.ProductPurchaseInformation.Price,
                    TaxId=product.ProductPurchaseInformation.TaxId,
                    Title=product.ProductPurchaseInformation.Title,
                },
                ProductSaleInformation=new Dal.Models.ProductSaleInformation
                {
                    TaxId=product.ProductSaleInformation.TaxId,
                    Price=product.ProductSaleInformation.Price,
                    Description=product.ProductSaleInformation.Description,
                    Title = product.ProductSaleInformation.Title,
                },
                ProductInventory=product.TrackInventory?new Dal.Models.ProductInventory
                {
                    DefaultLocationId=product.LocationId
                } :null
            };
            context.Product.Add(productDb);
            if (product.TrackInventory)
            {
                foreach(var inventory in product.InventoryHistories)
                {
                    if (inventory.Quantity > 0)
                    {
                        productDb.ProductInventory.ProductInventoryBalance.Add(new Dal.Models.ProductInventoryBalance
                        {
                            LocationId=inventory.LocationId,
                            Quantity=inventory.Quantity
                        });
                        productDb.ProductInventory.ProductInventoryHistory.Add(new Dal.Models.ProductInventoryHistory
                        {
                            Created = CurrentCambodiaTime,
                            CreatedBy = createdBy,
                            Date = CurrentCambodiaTime,
                            Description = "Begining inventory",
                            LocationId = inventory.LocationId,
                            Quantity = inventory.Quantity,
                            UnitPrice = inventory.UnitPrice,
                            ProductInventoryHistoryIn = new Dal.Models.ProductInventoryHistoryIn
                            {

                            }
                        });
                    }
                }
            }
            context.SaveChanges();
        }

        public int GetInventoryQty(long productId, long locationId, string organisationId)
        {
            using var context = Context();
            var inventoryBalance = context.ProductInventoryBalance.FirstOrDefault(u => u.ProductId == productId && u.LocationId == locationId && u.Product.IdNavigation.OrganisationId == organisationId);
            if (inventoryBalance == null)
            {
                return 0;
            }
            return inventoryBalance.Quantity;
        }

        public Product GetProduct(long id, string organisationId)
        {
            using var context = Context();
            var product = context.Product.FirstOrDefault(u => u.Id == id && u.OrganisationId == organisationId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return new Product
            {
                Code = product.Code,
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                OrganisationId = product.OrganisationId,
                TrackInventory = product.ProductInventory != null,
                ProductPurchaseInformation = new ProductSalePurchaseDetail
                {
                    Description = product.ProductPurchaseInformation.Description,
                    Price = product.ProductPurchaseInformation.Price,
                    TaxId = product.ProductPurchaseInformation.TaxId,
                    Title=product.ProductPurchaseInformation.Title
                },
                ProductSaleInformation = new ProductSalePurchaseDetail
                {
                    TaxId = product.ProductSaleInformation.TaxId,
                    Price = product.ProductSaleInformation.Price,
                    Description = product.ProductSaleInformation.Description,
                    Title = product.ProductSaleInformation.Title
                },
                LocationId=product.ProductInventory==null?null:product.ProductInventory.DefaultLocationId
            };
        }

        public List<Product> GetProducts(ProductFilter filter)
        {
            using var context = Context();
            var products = context.Product.Where(u => u.OrganisationId == filter.OrganisationId &&
                                                 (string.IsNullOrEmpty(filter.SearchText) || u.Code.Contains(filter.SearchText)
                                                  || u.Name.Contains(filter.SearchText))
                           ).OrderBy(u => u.Name)
                           .Skip((filter.Page - 1) * filter.PageSize)
                           .Take(filter.PageSize)
                           .Select(u => new Product
                           {
                               Code=u.Code,
                               Id=u.Id,
                               ImageUrl=u.ImageUrl,
                               Name=u.Name,
                               OrganisationId=u.OrganisationId,
                               TrackInventory=u.ProductInventory!=null,
                               ProductPurchaseInformation=new ProductSalePurchaseDetail
                               {
                                   Description=u.ProductPurchaseInformation.Description,
                                   Price=u.ProductPurchaseInformation.Price,
                                   TaxId=u.ProductPurchaseInformation.TaxId,
                                   Title=u.ProductPurchaseInformation.Title
                               },
                               ProductSaleInformation=new ProductSalePurchaseDetail
                               {
                                   Description=u.ProductSaleInformation.Description,
                                   Price=u.ProductSaleInformation.Price,
                                   TaxId=u.ProductSaleInformation.TaxId,
                                   Title = u.ProductSaleInformation.Title
                               },
                               
                           }).ToList();
            return products;
        }

        public List<Product> GetProductsForPurchase(string organisationId)
        {
            using var context = Context();
            var products = context.Product.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.Code).ThenBy(u => u.Name)
                .Select(u => new Product
                {
                    Id=u.Id,
                    Code=u.Code,
                    Name= u.Code +" - "+ u.ProductPurchaseInformation.Title,
                    ImageUrl=u.ImageUrl,
                    ProductPurchaseInformation=new ProductSalePurchaseDetail
                    {
                        Description = u.ProductPurchaseInformation.Description,
                        Price = u.ProductPurchaseInformation.Price,
                        TaxId = u.ProductPurchaseInformation.TaxId,
                        Title=u.ProductPurchaseInformation.Title
                    },
                    TrackInventory=u.ProductInventory!=null,
                    LocationId=u.ProductInventory==null?null:u.ProductInventory.DefaultLocationId
                }).ToList();
            return products;
        }

        public List<Product> GetProductsForSale(string organisationId)
        {
            using var context = Context();
            var products = context.Product.Where(u => u.OrganisationId == organisationId)
                .OrderBy(u => u.Code).ThenBy(u => u.Name)
                .Select(u => new Product
                {
                    Id = u.Id,
                    Code = u.Code,
                    Name = u.Code + " - " + u.ProductSaleInformation.Title,
                    ImageUrl = u.ImageUrl,
                    ProductSaleInformation=new ProductSalePurchaseDetail
                    {
                        Description=u.ProductSaleInformation.Description,
                        Price=u.ProductSaleInformation.Price,
                        TaxId=u.ProductSaleInformation.TaxId,
                        Title = u.ProductSaleInformation.Title
                    },
                    TrackInventory = u.ProductInventory != null,
                    LocationId = u.ProductInventory == null ? null : u.ProductInventory.DefaultLocationId
                }).ToList();
            return products;
        }

        public void GetTotalPages(ProductFilter filter)
        {
            using var context = Context();
           filter.TotalRow = context.Product.Where(u => u.OrganisationId == filter.OrganisationId &&
                                                 (string.IsNullOrEmpty(filter.SearchText) || u.Code.Contains(filter.SearchText)
                                                  || u.Name.Contains(filter.SearchText))).Count();
        }

        public void Update(Product product)
        {
            using var context = Context();
            var productDb = context.Product.FirstOrDefault(u => u.Id == product.Id && u.OrganisationId == product.OrganisationId);
            if (productDb == null)
            {
                throw new Exception("Product not found");
            }
            productDb.Name = product.Name;
            productDb.ProductPurchaseInformation.Description = product.ProductPurchaseInformation.Description;
            productDb.ProductPurchaseInformation.Title = product.ProductPurchaseInformation.Title;
            productDb.ProductPurchaseInformation.Price = product.ProductPurchaseInformation.Price;
            productDb.ProductPurchaseInformation.TaxId = product.ProductPurchaseInformation.TaxId;
            productDb.ProductSaleInformation.TaxId = product.ProductSaleInformation.TaxId;
            productDb.ProductSaleInformation.Price = product.ProductSaleInformation.Price;
            productDb.ProductSaleInformation.Description = product.ProductSaleInformation.Description;
            productDb.ProductSaleInformation.Title = product.ProductSaleInformation.Title;
            if (productDb.ProductInventory != null)
            {
                productDb.ProductInventory.DefaultLocationId = product.LocationId;
            }
            if(!string.IsNullOrEmpty(product.ImageUrl) && !product.ImageUrl.Contains("http"))
            {
                productDb.ImageUrl = UploadFile(product.ImageUrl);
            }
            context.SaveChanges();
        }

    }
}
