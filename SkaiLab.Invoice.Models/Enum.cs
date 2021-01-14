namespace SkaiLab.Invoice.Models
{
    public enum ExpenseStatusEnum
    {
        Delete=1,
        Draft=2,
        WaitingForApproval=3,
        Approved=4,
        Billed=5,
        OverDue=6
    }
    public enum QuoteEnum
    {
        Draft=1,
        Accepted=2,
        Declined=3,
        Invoiced=4,
        Delete=5,
        Expire = 100
    }
    public enum InvoiceStatusEnum
    {
        WaitingForPayment=1,
        Paid=2,
        OverDue = 1000
    }
    public enum CustomerCreditStatusEnum
    {
        Delete = 1,
        Draft = 2,
        WaitingForApproval = 3,
        Approved = 4,
        Paid = 5
    }
    public enum PrintDocumentTypeEnum
    {
        Quote=1,
        PurchaseOrder = 2,
        Bill=3,
        Expense = 4,
        Invoice = 5
    }
    public enum PurchaseOrderDateTypeFilter
    {
        All=0,
        Date=1,
        DeliveryDate=2
    }
    public enum BillDateTypeFilterEnum
    {
        All = 0,
        Date = 1,
        DueDate = 2
    }
    public enum QuoteFilterEnum
    {
        All = 0,
        Date = 1,
        Expire = 2
    }
    public enum InvoiceDateTypeFilterEnum
    {
        All = 0,
        Date = 1,
        DueDate = 2
    }
    public enum SalaryTypeEnum
    {
        Net=1,
        Cross=2
    }
    public enum DashboardPeriodEnum
    {
        Last30Day = 1,
        ThisMonth = 2,
        ThisQuarter = 3,
        ThisYear = 4,
        LastMonth = 5,
        LastQuater = 6,
        LastYear = 7
    }
    public enum DisplayColumnEnum
    {
        Month = 1,
        TotalOnly = 2,
        Organisation = 3
    }
    public enum MaritalStatusEnum
    {
        Married=1,
        Widowed=2,
        Separated=3,
        Divorced=4,
        Single=5
    }
    public enum MenuFeatureEnum
    {
        ReadWritePurchaseSale=1,
        ReadPurchaseSale = 2,
        ApprovaPayPurchaseSale = 3,
        ManageOrganisactionSetting=4,
        ManageAndInviteUser=5,
        Payroll=6,
        Report=7
    }
    public enum CreateCompanyResultEnum
    {
        Success=0,
        UserNoLicense=1,
        LicenseExpireOrIncomple=2,
        LimitCreateNumberOfOrganisation=3
    }
}
