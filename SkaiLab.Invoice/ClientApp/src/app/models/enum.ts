export enum ExepnseStatusEnum{
    Delete=1,
    Draft=2,
    WaitingForApproval=3,
    Approved=4,
    Billed=5,
    OverDue=6
}
export enum QuoteEnum
{
    Draft=1,
    Accepted=2,
    Declined=3,
    Invoiced=4,
    Delete=5,
    Expire = 100
}
export enum InvoiceStatusEnum{
    WaitingForPayment=1,
    Paid=2,
    OverDue = 1000
}
export enum PrintDocumentType{
    Quote=1,
    PurchaseOrder = 2,
    Bill=3,
    Expense=4,
    Invoice = 5
}
export enum PurchaseOrderDateTypeFilterEnum
{
    All=0,
    Date=1,
    DeliveryDate=2
}
export enum BillDateTypeFilterEnum
{
    All=0,
    Date=1,
    DueDate=2
}
export enum QuoteFilterEnum
{
    All=0,
    Date=1,
    Expire=2
}
export enum InvoiceDateTypeFilterEnum
{
    All=0,
    Date=1,
    DueDate=2
}
export enum DashboardPeriodEnum{
    Last30Day=1,
    ThisMonth=2,
    ThisQuarter=3,
    ThisYear=4,
    LastMonth=5,
    LastQuater=6,
    LastYear=7,
    Custom=8
}
export enum DisplayColumnEnum{
    Month=1,
    TotalOnly=2,
    Organisation=3
}