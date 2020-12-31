using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SkaiLab.Invoice.Dal.Models
{
    public partial class InvoiceContext : DbContext
    {
        public InvoiceContext()
        {
        }

        public InvoiceContext(DbContextOptions<InvoiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Bill> Bill { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerTransaction> CustomerTransaction { get; set; }
        public virtual DbSet<CustomerTransactionAttachment> CustomerTransactionAttachment { get; set; }
        public virtual DbSet<CustomerTransactionItem> CustomerTransactionItem { get; set; }
        public virtual DbSet<CustomerTransactionItemProduct> CustomerTransactionItemProduct { get; set; }
        public virtual DbSet<DeviceCodes> DeviceCodes { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<ExchangeRate> ExchangeRate { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<ExpenseAttachentFile> ExpenseAttachentFile { get; set; }
        public virtual DbSet<ExpenseItem> ExpenseItem { get; set; }
        public virtual DbSet<ExpenseProductItem> ExpenseProductItem { get; set; }
        public virtual DbSet<ExpenseStatus> ExpenseStatus { get; set; }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceQuote> InvoiceQuote { get; set; }
        public virtual DbSet<InvoiceStatus> InvoiceStatus { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<MaritalStatus> MaritalStatus { get; set; }
        public virtual DbSet<MenuFeature> MenuFeature { get; set; }
        public virtual DbSet<Organisation> Organisation { get; set; }
        public virtual DbSet<OrganisationBaseCurrency> OrganisationBaseCurrency { get; set; }
        public virtual DbSet<OrganisationCurrency> OrganisationCurrency { get; set; }
        public virtual DbSet<OrganisationInvitingUser> OrganisationInvitingUser { get; set; }
        public virtual DbSet<OrganisationInvitingUserMenuFeature> OrganisationInvitingUserMenuFeature { get; set; }
        public virtual DbSet<OrganisationInvoiceSetting> OrganisationInvoiceSetting { get; set; }
        public virtual DbSet<OrganisationType> OrganisationType { get; set; }
        public virtual DbSet<OrganisationUser> OrganisationUser { get; set; }
        public virtual DbSet<OrganisationUserMenuFeature> OrganisationUserMenuFeature { get; set; }
        public virtual DbSet<PayrollEmployee> PayrollEmployee { get; set; }
        public virtual DbSet<PayrollEmployeeNoneTax> PayrollEmployeeNoneTax { get; set; }
        public virtual DbSet<PayrollEmployeeTax> PayrollEmployeeTax { get; set; }
        public virtual DbSet<PayrollMonth> PayrollMonth { get; set; }
        public virtual DbSet<PayrollMonthTaxSalary> PayrollMonthTaxSalary { get; set; }
        public virtual DbSet<PayrollMonthTaxSalaryRange> PayrollMonthTaxSalaryRange { get; set; }
        public virtual DbSet<PersistedGrants> PersistedGrants { get; set; }
        public virtual DbSet<Plan> Plan { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductInventory> ProductInventory { get; set; }
        public virtual DbSet<ProductInventoryBalance> ProductInventoryBalance { get; set; }
        public virtual DbSet<ProductInventoryHistory> ProductInventoryHistory { get; set; }
        public virtual DbSet<ProductInventoryHistoryIn> ProductInventoryHistoryIn { get; set; }
        public virtual DbSet<ProductInventoryHistoryOut> ProductInventoryHistoryOut { get; set; }
        public virtual DbSet<ProductPurchaseInformation> ProductPurchaseInformation { get; set; }
        public virtual DbSet<ProductSaleInformation> ProductSaleInformation { get; set; }
        public virtual DbSet<ProjectPlan> ProjectPlan { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public virtual DbSet<Quote> Quote { get; set; }
        public virtual DbSet<QuoteAttachment> QuoteAttachment { get; set; }
        public virtual DbSet<QuoteItem> QuoteItem { get; set; }
        public virtual DbSet<QuoteStatus> QuoteStatus { get; set; }
        public virtual DbSet<SalaryType> SalaryType { get; set; }
        public virtual DbSet<Tax> Tax { get; set; }
        public virtual DbSet<TaxComponent> TaxComponent { get; set; }
        public virtual DbSet<TaxSalary> TaxSalary { get; set; }
        public virtual DbSet<TaxSalaryRange> TaxSalaryRange { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<VendorExpense> VendorExpense { get; set; }
        public virtual DbSet<WorkingOrganisation> WorkingOrganisation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=SENG-DELL\\SQLDB;Database=Invoice;user=sa;password=12345");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Bill)
                    .HasForeignKey<Bill>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bill__Id__37703C52");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.ContactName).HasMaxLength(150);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Alpha2Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Alpha3Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NameKh).HasMaxLength(200);

                entity.Property(e => e.Nationality)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NationalityKh).HasMaxLength(200);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Symbole).HasMaxLength(100);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.BusinessRegistrationNumber).HasMaxLength(100);

                entity.Property(e => e.ContactId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LegalName).HasMaxLength(200);

                entity.Property(e => e.LocalLegalName).HasMaxLength(200);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TaxNumber).HasMaxLength(100);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__Contac__2BFE89A6");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__Curren__2B0A656D");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__Organi__2A164134");
            });

            modelBuilder.Entity<CustomerTransaction>(entity =>
            {
                entity.HasIndex(e => new { e.OrganisationId, e.Number })
                    .HasName("UQ__Customer__A5A95CC40668054F")
                    .IsUnique();

                entity.Property(e => e.BaseCurrencyExchangeRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(250);

                entity.Property(e => e.Number).HasMaxLength(100);

                entity.Property(e => e.OrganisationId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaidBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PaidDate).HasColumnType("datetime");

                entity.Property(e => e.RefNo).HasMaxLength(100);

                entity.Property(e => e.TaxCurrencyExchangeRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalIncludeTax).HasColumnType("money");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.CustomerTransaction)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerT__Curre__3D2915A8");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerTransaction)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerT__Custo__4B422AD5");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.CustomerTransaction)
                    .HasForeignKey(d => d.OrganisationId)
                    .HasConstraintName("FK__CustomerT__Organ__3F115E1A");
            });

            modelBuilder.Entity<CustomerTransactionAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(200);

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.CustomerTransaction)
                    .WithMany(p => p.CustomerTransactionAttachment)
                    .HasForeignKey(d => d.CustomerTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerT__Custo__57A801BA");
            });

            modelBuilder.Entity<CustomerTransactionItem>(entity =>
            {
                entity.Property(e => e.LineTotal).HasColumnType("money");

                entity.Property(e => e.LineTotalIncludeTax).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.CustomerTransaction)
                    .WithMany(p => p.CustomerTransactionItem)
                    .HasForeignKey(d => d.CustomerTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerT__Custo__4F12BBB9");

                entity.HasOne(d => d.Tax)
                    .WithMany(p => p.CustomerTransactionItem)
                    .HasForeignKey(d => d.TaxId)
                    .HasConstraintName("FK__CustomerT__TaxId__5006DFF2");
            });

            modelBuilder.Entity<CustomerTransactionItemProduct>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.CustomerTransactionItemProduct)
                    .HasForeignKey<CustomerTransactionItemProduct>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerTran__Id__52E34C9D");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.CustomerTransactionItemProduct)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK__CustomerT__Locat__54CB950F");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CustomerTransactionItemProduct)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerT__Produ__53D770D6");
            });

            modelBuilder.Entity<DeviceCodes>(entity =>
            {
                entity.HasKey(e => e.UserCode);

                entity.Property(e => e.UserCode).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.DeviceCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SubjectId).HasMaxLength(200);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.DocumentUrl).HasMaxLength(200);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.IdorPassportNumber)
                    .HasColumnName("IDOrPassportNumber")
                    .HasMaxLength(100);

                entity.Property(e => e.JobTitle).HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber).HasMaxLength(100);

                entity.Property(e => e.Salary).HasColumnType("decimal(18, 6)");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__Countr__45BE5BA9");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.GenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__Gender__26CFC035");

                entity.HasOne(d => d.MaritalStatus)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.MaritalStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__Marita__27C3E46E");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__Organi__29AC2CE0");

                entity.HasOne(d => d.SalaryType)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.SalaryTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Employee__Salary__28B808A7");
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasKey(e => new { e.FromOrganisationCurrencyId, e.ToOrganisationCurrencyId });

                entity.Property(e => e.ExchangeRate1)
                    .HasColumnName("ExchangeRate")
                    .HasColumnType("decimal(18, 6)");

                entity.HasOne(d => d.FromOrganisationCurrency)
                    .WithMany(p => p.ExchangeRateFromOrganisationCurrency)
                    .HasForeignKey(d => d.FromOrganisationCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExchangeR__FromO__1332DBDC");

                entity.HasOne(d => d.ToOrganisationCurrency)
                    .WithMany(p => p.ExchangeRateToOrganisationCurrency)
                    .HasForeignKey(d => d.ToOrganisationCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExchangeR__ToOrg__14270015");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasIndex(e => new { e.OrganisationId, e.Number })
                    .HasName("UQ__Expense__A5A95CC48490AB4D")
                    .IsUnique();

                entity.Property(e => e.ApprovedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovedDate).HasColumnType("datetime");

                entity.Property(e => e.BaseCurrencyExchangeRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.BilledBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BilledDate).HasColumnType("datetime");

                entity.Property(e => e.CloseDate).HasColumnType("datetime");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.Property(e => e.DeliveryDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RefNo).HasMaxLength(100);

                entity.Property(e => e.TaxCurrencyExchangeRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalIncludeTax).HasColumnType("money");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Expense__Currenc__4C6B5938");

                entity.HasOne(d => d.ExpenseStatus)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.ExpenseStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Expense__Expense__4D5F7D71");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Expense__Organis__4E53A1AA");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Expense__VendorI__4F47C5E3");
            });

            modelBuilder.Entity<ExpenseAttachentFile>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(200);

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Expense)
                    .WithMany(p => p.ExpenseAttachentFile)
                    .HasForeignKey(d => d.ExpenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExpenseAt__Expen__503BEA1C");
            });

            modelBuilder.Entity<ExpenseItem>(entity =>
            {
                entity.Property(e => e.LineTotal).HasColumnType("money");

                entity.Property(e => e.LineTotalIncludeTax).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Expense)
                    .WithMany(p => p.ExpenseItem)
                    .HasForeignKey(d => d.ExpenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExpenseIt__Expen__00DF2177");

                entity.HasOne(d => d.Tax)
                    .WithMany(p => p.ExpenseItem)
                    .HasForeignKey(d => d.TaxId)
                    .HasConstraintName("FK__ExpenseIt__TaxId__01D345B0");
            });

            modelBuilder.Entity<ExpenseProductItem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.ExpenseProductItem)
                    .HasForeignKey<ExpenseProductItem>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExpenseProdu__Id__04AFB25B");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.ExpenseProductItem)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK__ExpensePr__Locat__531856C7");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ExpenseProductItem)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExpensePr__Produ__540C7B00");
            });

            modelBuilder.Entity<ExpenseStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Invoice)
                    .HasForeignKey<Invoice>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__Id__55F4C372");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Invoice__StatusI__56E8E7AB");
            });

            modelBuilder.Entity<InvoiceQuote>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.InvoiceQuote)
                    .HasForeignKey<InvoiceQuote>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InvoiceQuote__Id__58D1301D");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.InvoiceQuote)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InvoiceQu__Quote__57DD0BE4");
            });

            modelBuilder.Entity<InvoiceStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasIndex(e => new { e.OrganisationId, e.Name })
                    .HasName("UQ__Location__15141E92B2AD2DC1")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Location)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Location__Organi__59C55456");
            });

            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<MenuFeature>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NameKh).HasMaxLength(500);
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BussinessRegistrationNumber).HasMaxLength(100);

                entity.Property(e => e.ContactId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.LegalLocalName).HasMaxLength(250);

                entity.Property(e => e.LegalName).HasMaxLength(250);

                entity.Property(e => e.LineBusiness).HasMaxLength(250);

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.TaxNumber).HasMaxLength(100);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Organisation)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK__Organisat__Conta__5AB9788F");

                entity.HasOne(d => d.OrganisationType)
                    .WithMany(p => p.Organisation)
                    .HasForeignKey(d => d.OrganisationTypeId)
                    .HasConstraintName("FK__Organisat__Organ__5BAD9CC8");
            });

            modelBuilder.Entity<OrganisationBaseCurrency>(entity =>
            {
                entity.HasKey(e => e.OrganisationId)
                    .HasName("PK__Organisa__722346DCF9CABD58");

                entity.Property(e => e.OrganisationId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.BaseCurrency)
                    .WithMany(p => p.OrganisationBaseCurrencyBaseCurrency)
                    .HasForeignKey(d => d.BaseCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__BaseC__5CA1C101");

                entity.HasOne(d => d.Organisation)
                    .WithOne(p => p.OrganisationBaseCurrency)
                    .HasForeignKey<OrganisationBaseCurrency>(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Organ__5D95E53A");

                entity.HasOne(d => d.TaxCurrency)
                    .WithMany(p => p.OrganisationBaseCurrencyTaxCurrency)
                    .HasForeignKey(d => d.TaxCurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__TaxCu__5E8A0973");
            });

            modelBuilder.Entity<OrganisationCurrency>(entity =>
            {
                entity.HasIndex(e => new { e.OrganisationId, e.CurrencyId })
                    .HasName("UQ__Organisa__636736726785E5C2")
                    .IsUnique();

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.OrganisationCurrency)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Curre__5F7E2DAC");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationCurrency)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Organ__607251E5");
            });

            modelBuilder.Entity<OrganisationInvitingUser>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.Email })
                    .HasName("PK__Organisa__38BE568F77C61C76");

                entity.Property(e => e.OrganisationId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.ExpireToken).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.RoleName).HasMaxLength(100);

                entity.Property(e => e.Token)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationInvitingUser)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Organ__6166761E");
            });

            modelBuilder.Entity<OrganisationInvitingUserMenuFeature>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.Email, e.MenuFeatureId })
                    .HasName("PK__Organisa__0040F186A6A995E6");

                entity.Property(e => e.OrganisationId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.MenuFeature)
                    .WithMany(p => p.OrganisationInvitingUserMenuFeature)
                    .HasForeignKey(d => d.MenuFeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__MenuF__625A9A57");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationInvitingUserMenuFeature)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Organ__634EBE90");
            });

            modelBuilder.Entity<OrganisationInvoiceSetting>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.OrganisationInvoiceSetting)
                    .HasForeignKey<OrganisationInvoiceSetting>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisation__Id__6442E2C9");
            });

            modelBuilder.Entity<OrganisationType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<OrganisationUser>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.UserId })
                    .HasName("PK__Organisa__A35BCA1848D8BF51");

                entity.Property(e => e.OrganisationId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RoleName).HasMaxLength(200);

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationUser)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Organ__65370702");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OrganisationUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__UserI__662B2B3B");
            });

            modelBuilder.Entity<OrganisationUserMenuFeature>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.UserId, e.MenuFeatureId })
                    .HasName("PK__Organisa__9BA56D1122BDDA71");

                entity.Property(e => e.OrganisationId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.MenuFeature)
                    .WithMany(p => p.OrganisationUserMenuFeature)
                    .HasForeignKey(d => d.MenuFeatureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__MenuF__671F4F74");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationUserMenuFeature)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__Organ__681373AD");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OrganisationUserMenuFeature)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Organisat__UserI__690797E6");
            });

            modelBuilder.Entity<PayrollEmployee>(entity =>
            {
                entity.HasIndex(e => new { e.EmployeeId, e.PayrollMonthId })
                    .HasName("UQ__PayrollE__B712A6542E3A6429")
                    .IsUnique();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.PayrollEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollEm__Emplo__69FBBC1F");

                entity.HasOne(d => d.PayrollMonth)
                    .WithMany(p => p.PayrollEmployee)
                    .HasForeignKey(d => d.PayrollMonthId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollEm__Payro__6AEFE058");
            });

            modelBuilder.Entity<PayrollEmployeeNoneTax>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.OtherBenefit).HasColumnType("money");

                entity.Property(e => e.Salary).HasColumnType("money");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.PayrollEmployeeNoneTax)
                    .HasForeignKey<PayrollEmployeeNoneTax>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollEmplo__Id__6BE40491");
            });

            modelBuilder.Entity<PayrollEmployeeTax>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.OtherBenefit).HasColumnType("money");

                entity.Property(e => e.OtherBenefitTaxDeduct).HasColumnType("money");

                entity.Property(e => e.Salary).HasColumnType("money");

                entity.Property(e => e.SalaryTax).HasColumnType("money");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.PayrollEmployeeTax)
                    .HasForeignKey<PayrollEmployeeTax>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollEmplo__Id__6CD828CA");
            });

            modelBuilder.Entity<PayrollMonth>(entity =>
            {
                entity.HasIndex(e => new { e.OrganisationId, e.Month })
                    .HasName("UQ__PayrollM__5D8A0CCBC739B4D3")
                    .IsUnique();

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Month)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.PayrollMonth)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollMo__Organ__6DCC4D03");
            });

            modelBuilder.Entity<PayrollMonthTaxSalary>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AdditionalBenefits).HasColumnType("money");

                entity.Property(e => e.ChildOrSpouseAmount).HasColumnType("money");

                entity.Property(e => e.NoneResidentRate).HasColumnType("money");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.PayrollMonthTaxSalary)
                    .HasForeignKey<PayrollMonthTaxSalary>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollMonth__Id__6EC0713C");
            });

            modelBuilder.Entity<PayrollMonthTaxSalaryRange>(entity =>
            {
                entity.Property(e => e.FromAmount).HasColumnType("money");

                entity.Property(e => e.ToAmount).HasColumnType("money");

                entity.HasOne(d => d.PayrollMonthTaxNavigation)
                    .WithMany(p => p.PayrollMonthTaxSalaryRange)
                    .HasForeignKey(d => d.PayrollMonthTax)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayrollMo__Payro__6FB49575");
            });

            modelBuilder.Entity<PersistedGrants>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.Property(e => e.Key).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.SubjectId).HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);

                entity.HasOne(d => d.ProjectPlan)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.ProjectPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Plan__ProjectPla__308E3499");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => new { e.OrganisationId, e.Code })
                    .HasName("UQ__Product__08068377B7C65DC8")
                    .IsUnique();

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__Organis__70A8B9AE");
            });

            modelBuilder.Entity<ProductInventory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.DefaultLocation)
                    .WithMany(p => p.ProductInventory)
                    .HasForeignKey(d => d.DefaultLocationId)
                    .HasConstraintName("FK__ProductIn__Defau__719CDDE7");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.ProductInventory)
                    .HasForeignKey<ProductInventory>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductInven__Id__72910220");
            });

            modelBuilder.Entity<ProductInventoryBalance>(entity =>
            {
                entity.HasIndex(e => new { e.ProductId, e.LocationId })
                    .HasName("UQ__ProductI__DA732C85CFEFEE9E")
                    .IsUnique();

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.ProductInventoryBalance)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIn__Locat__73852659");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductInventoryBalance)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIn__Produ__74794A92");
            });

            modelBuilder.Entity<ProductInventoryHistory>(entity =>
            {
                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.RefNo).HasMaxLength(100);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 6)");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.ProductInventoryHistory)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIn__Locat__756D6ECB");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductInventoryHistory)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIn__Produ__76619304");
            });

            modelBuilder.Entity<ProductInventoryHistoryIn>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.ProductInventoryHistoryIn)
                    .HasForeignKey<ProductInventoryHistoryIn>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductInven__Id__7755B73D");
            });

            modelBuilder.Entity<ProductInventoryHistoryOut>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.ProductInventoryHistoryOut)
                    .HasForeignKey<ProductInventoryHistoryOut>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductInven__Id__793DFFAF");

                entity.HasOne(d => d.InventoryIn)
                    .WithMany(p => p.ProductInventoryHistoryOut)
                    .HasForeignKey(d => d.InventoryInId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIn__Inven__7849DB76");
            });

            modelBuilder.Entity<ProductPurchaseInformation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Title)
                    .HasMaxLength(500)
                    .IsFixedLength();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.ProductPurchaseInformation)
                    .HasForeignKey<ProductPurchaseInformation>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductPurch__Id__3B40CD36");

                entity.HasOne(d => d.Tax)
                    .WithMany(p => p.ProductPurchaseInformation)
                    .HasForeignKey(d => d.TaxId)
                    .HasConstraintName("FK__ProductPu__TaxId__3C34F16F");
            });

            modelBuilder.Entity<ProductSaleInformation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.ProductSaleInformation)
                    .HasForeignKey<ProductSaleInformation>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductSaleI__Id__3F115E1A");

                entity.HasOne(d => d.Tax)
                    .WithMany(p => p.ProductSaleInformation)
                    .HasForeignKey(d => d.TaxId)
                    .HasConstraintName("FK__ProductSa__TaxId__40058253");
            });

            modelBuilder.Entity<ProjectPlan>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.PurchaseOrder)
                    .HasForeignKey<PurchaseOrder>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PurchaseOrde__Id__7E02B4CC");
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.Property(e => e.AcceptedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AcceptedDate).HasColumnType("datetime");

                entity.Property(e => e.BaseCurrencyExchangeRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DeclinedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DeclinedDate).HasColumnType("datetime");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.InvoicedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InvoicedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RefNo).HasMaxLength(100);

                entity.Property(e => e.TaxCurrencyExchangeRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalIncludeTax).HasColumnType("money");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Quote)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quote__CurrencyI__7EF6D905");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Quote)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quote__CustomerI__22401542");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Quote)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quote__Organisat__00DF2177");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Quote)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quote__StatusId__01D345B0");
            });

            modelBuilder.Entity<QuoteAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(200);

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteAttachment)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuoteAtta__Quote__02C769E9");
            });

            modelBuilder.Entity<QuoteItem>(entity =>
            {
                entity.Property(e => e.LineTotal).HasColumnType("money");

                entity.Property(e => e.LineTotalIncludeTax).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.QuoteItem)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK__QuoteItem__Locat__2AD55B43");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.QuoteItem)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuoteItem__Produ__29E1370A");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteItem)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuoteItem__Quote__27F8EE98");

                entity.HasOne(d => d.Tax)
                    .WithMany(p => p.QuoteItem)
                    .HasForeignKey(d => d.TaxId)
                    .HasConstraintName("FK__QuoteItem__TaxId__28ED12D1");
            });

            modelBuilder.Entity<QuoteStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<SalaryType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameKh).HasMaxLength(100);
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Tax)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Tax__Organisatio__078C1F06");
            });

            modelBuilder.Entity<TaxComponent>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Rate).HasColumnType("money");

                entity.HasOne(d => d.Tax)
                    .WithMany(p => p.TaxComponent)
                    .HasForeignKey(d => d.TaxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaxCompon__TaxId__0880433F");
            });

            modelBuilder.Entity<TaxSalary>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AdditionalBenefits).HasColumnType("money");

                entity.Property(e => e.ChildOrSpouseAmount).HasColumnType("money");

                entity.Property(e => e.NoneResidentRate).HasColumnType("money");
            });

            modelBuilder.Entity<TaxSalaryRange>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FromAmount).HasColumnType("money");

                entity.Property(e => e.ToAmount).HasColumnType("money");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.Property(e => e.BusinessRegistrationNumber).HasMaxLength(100);

                entity.Property(e => e.ContactId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LegalName).HasMaxLength(100);

                entity.Property(e => e.LocalLegalName).HasMaxLength(200);

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TaxNumber).HasMaxLength(100);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.Vendor)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__ContactI__09746778");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Vendor)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__Currency__208CD6FA");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.Vendor)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__Organisa__2180FB33");
            });

            modelBuilder.Entity<VendorExpense>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.VendorExpense)
                    .HasForeignKey<VendorExpense>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__VendorExpens__Id__0D44F85C");
            });

            modelBuilder.Entity<WorkingOrganisation>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__WorkingO__1788CC4CC376231C");

                entity.Property(e => e.OrganisationId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.WorkingOrganisation)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkingOr__Organ__0E391C95");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.WorkingOrganisation)
                    .HasForeignKey<WorkingOrganisation>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WorkingOr__UserI__0F2D40CE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
