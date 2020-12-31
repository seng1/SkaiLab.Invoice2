using SkaiLab.Invoice.Models;
using System.Collections.Generic;
using System.Linq;

namespace SkaiLab.Invoice.Service
{
    public class MenuService:Service,IMenuService
    {
        public MenuService(IDataContext context) : base(context)
        {

        }

        public List<Menu> GetMenus(string userId, string organisationId)
        {
            using var context = Context();
            var menuFeatures = context.OrganisationUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.UserId == userId)
                .Select(u => u.MenuFeatureId).ToList();
            if (!menuFeatures.Any())
            {
                return new List<Menu>();
            }
            var result = new List<Menu>
            {
                new Menu
                {
                    IconClass = "menu-icon fa fa-laptop",
                    Name = AppResource.GetResource("Dashboard"),
                    IsActive = true,
                    MenuItems = new List<Menu>(),
                    RouteLink = "/"
                }
            };
            if (menuFeatures.Any(t=>t==(int) MenuFeatureEnum.ReadPurchaseSale || t == (int)MenuFeatureEnum.ReadWritePurchaseSale))
            {
                result.Add(new Menu
                {
                    IconClass = "menu-icon fas fa-file-invoice-dollar",
                    Name = AppResource.GetResource("Sale"),
                    IsActive = false,
                    RouteLink="",
                    MenuItems=new List<Menu>
                    {
                        new Menu
                        {
                             IconClass = "menu-icon fa fa-laptop",
                             Name = AppResource.GetResource("Overview"),
                             IsActive = false,
                             RouteLink="/saleoverview",
                        },
                        new Menu
                        {
                             IconClass = "menu-icon fas fa-file-invoice-dollar",
                             Name =  AppResource.GetResource("Quotes"),
                             IsActive = false,
                             RouteLink="/quote",
                        },
                        new Menu
                        {
                             IconClass = "menu-icon fas fa-file-invoice-dollar",
                             Name = AppResource.GetResource("Invoices"),
                             IsActive = false,
                             RouteLink="/invoice",
                        }
                    }
                });
                result.Add(new Menu
                {
                    IconClass = "menu-icon fa fa-shopping-cart",
                    Name = AppResource.GetResource("Purchase"),
                    IsActive = false,
                    RouteLink = "",
                    MenuItems = new List<Menu>
                    {
                        new Menu
                        {
                             IconClass = "menu-icon fa fa-laptop",
                             Name = AppResource.GetResource("Overview"),
                             IsActive = false,
                             RouteLink="/purchaseoverview",
                        },
                        new Menu
                        {
                             IconClass = "menu-icon fa fa-shopping-cart",
                             Name =AppResource.GetResource("Purchase Orders"),
                             IsActive = false,
                             RouteLink="/order",
                        },
                        new Menu
                        {
                             IconClass = "menu-icon fa fa-money",
                             Name = AppResource.GetResource("Bills"),
                             IsActive = false,
                             RouteLink="/vendor-bill",
                        }
                    }
                });
                result.Add(new Menu
                {
                    IconClass = "menu-icon fab fa-product-hunt",
                    Name = AppResource.GetResource("Products"),
                    IsActive = false,
                    MenuItems = new List<Menu>(),
                    RouteLink = "/product"
                });
                result.Add(new Menu
                {
                    IconClass = "menu-icon fa fa-handshake-o",
                    Name = AppResource.GetResource("Customers"),
                    IsActive = false,
                    MenuItems = new List<Menu>(),
                    RouteLink = "/customer"
                });
                result.Add(new Menu
                {
                    IconClass = "menu-icon fa fa-industry",
                    Name = AppResource.GetResource("Vendors"),
                    IsActive = false,
                    MenuItems = new List<Menu>(),
                    RouteLink = "/vendor"
                });
            }
            if (menuFeatures.Any(t => t == (int)MenuFeatureEnum.Report))
            {
                result.Add(new Menu
                {
                    IconClass = "menu-icon fas fa-chart-bar",
                    Name = AppResource.GetResource("Reports"),
                    IsActive = false,
                    MenuItems = new List<Menu>(),
                    RouteLink = "/report"
                });
            }

            if (menuFeatures.Any(t => t == (int)MenuFeatureEnum.Payroll))
            {
                result.Add(new Menu
                {
                    IconClass = "menu-icon fas fa-portrait",
                    Name = AppResource.GetResource("Pay run"),
                    IsActive = false,
                    MenuItems = new List<Menu>(),
                    RouteLink = "/payroll"
                });
            }
            if (menuFeatures.Any(t => t == (int)MenuFeatureEnum.ManageOrganisactionSetting || t==(int)MenuFeatureEnum.ManageAndInviteUser))
            {
                var setting = new Menu
                {
                    IconClass = "menu-icon fa fa-glass",
                    Name = AppResource.GetResource("Setting"),
                    IsActive = false,
                    MenuItems = new List<Menu>()
                };
                if(menuFeatures.Any(t=>t== (int)MenuFeatureEnum.ManageAndInviteUser))
                {
                    setting.MenuItems.Add(new Menu
                    {
                        IconClass = "menu-icon fas fa-user-alt",
                        Name = AppResource.GetResource("Users"),
                        IsActive = false,
                        RouteLink = "/user"
                    });
                }
                if (menuFeatures.Any(t => t == (int)MenuFeatureEnum.ManageAndInviteUser))
                {
                    setting.MenuItems.AddRange(new List<Menu>
                    {
                        new Menu
                        {
                             IconClass = "menu-icon fas fa-building",
                             Name = AppResource.GetResource("Company"),
                             IsActive = false,
                             RouteLink = "/company"
                        },
                        new Menu
                        {
                             IconClass = "menu-icon fas fa-portrait",
                             Name = AppResource.GetResource("Employees"),
                             IsActive = false,
                             RouteLink = "/employee"
                        },
                         new Menu
                        {
                             IconClass = "menu-icon fa fa fa-money",
                             Name = AppResource.GetResource("Currencies"),
                             IsActive = false,
                             RouteLink = "/currencies"
                        },
                        new Menu
                        {
                             IconClass = "menu-icon  fas fa-building",
                             Name = AppResource.GetResource("Locations"),
                             IsActive = false,
                             RouteLink = "/location"
                        },
                        new Menu
                        {
                             IconClass = "menu-icon fa fa-paper-plane",
                             Name = AppResource.GetResource("Invoice Setting"),
                             IsActive = false,
                             RouteLink = "/invoicesetting"
                        },
                    });
                    if (context.Organisation.FirstOrDefault(u => u.Id == organisationId).DeclareTax)
                    {
                        setting.MenuItems.Add(new Menu
                        {
                            IconClass = "menu-icon fa fa-percent",
                            Name = AppResource.GetResource("Taxes"),
                            IsActive = false,
                            RouteLink = "/taxrate"
                        });
                    }
                }
                result.Add(setting);
            }
            return result;
        }

        public Permission GetPermission(string userId, string organisationId)
        {
            using var context = Context();
            var menuFeatures = context.OrganisationUserMenuFeature.Where(u => u.OrganisationId == organisationId && u.UserId == userId)
                .Select(u => u.MenuFeatureId).ToList();
            return new Permission
            {
                ApprovaPayPurchaseSale = menuFeatures.Any(u => u == (int)MenuFeatureEnum.ApprovaPayPurchaseSale),
                ManageAndInviteUser = menuFeatures.Any(u => u == (int)MenuFeatureEnum.ManageAndInviteUser),
                ManageOrganisactionSetting = menuFeatures.Any(u => u == (int)MenuFeatureEnum.ManageOrganisactionSetting),
                Payroll = menuFeatures.Any(u => u == (int)MenuFeatureEnum.Payroll),
                ReadPurchaseSale = menuFeatures.Any(u => u == (int)MenuFeatureEnum.ReadPurchaseSale),
                ReadWritePurchaseSale = menuFeatures.Any(u => u == (int)MenuFeatureEnum.ReadWritePurchaseSale),
                Report = menuFeatures.Any(u => u == (int)MenuFeatureEnum.Report),
            };
        }
    }
}
