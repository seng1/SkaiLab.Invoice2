import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import { BaseService } from './base-service';
import { InventoryHistoryFilter, ProfitAndLostSummaryFilter, ReportFilter } from '../models/report-filter';
import { ProfitAndLostSummary } from '../models/ProfitAndLostSummary';
import * as html2pdf from 'html2pdf.js';
import { ProfitAndLostDetail } from '../models/ProfitAndLostDetail';
import { CustomerBalanceDetail } from '../models/CustomerBalanceDetail';
import { CustomerBalanceSummary } from '../models/CustomerBalanceSummary';
import { Invoice } from '../models/customer-transaction';
import { ProductSaleSummary } from '../models/product-sale-summary';
import { ProductSaleDetail } from '../models/product-sale-detail-report';
import { Product } from '../models/product';
import { InventoryHistoryDetail } from '../models/inventoryHistory';
import { TaxMonthly } from '../models/tax-monthly';

@Injectable()
export class ReportService {
  constructor(private http: HttpClient, private baseService: BaseService) { }
  generatePdf(reportName: string, paper: string, orientation: string, htmlData: string) {
    const options = {
      pagebreak: { mode: 'avoid-all', before: '#page2el' },
      image: { type: 'jpeg', quality: 1 },
      html2canvas: {
        dpi: 192,
        scale:4,
        letterRendering: true,
        useCORS: true
      },
      filename: reportName,
      jsPDF: { unit: 'in',format: paper, orientation: orientation }
    }
    html2pdf().from(htmlData).set(options).toPdf().get('pdf').then(function (pdf) {
      var totalPages = pdf.internal.getNumberOfPages();
      for (var i = 1; i <= totalPages; i++) {
        pdf.setPage(i);
        pdf.setFontSize(10);
        pdf.setTextColor(150);
        pdf.text('Page ' + i + ' of ' + totalPages, pdf.internal.pageSize.getWidth() - 100, pdf.internal.pageSize.getHeight() - 30);
      }
    }).save();
  }
  getProfitAndLostSummary(filter: ProfitAndLostSummaryFilter): Observable<ProfitAndLostSummary> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetProfitAndLostSummary";
    return this.http.post<ProfitAndLostSummary>(url, filter);
  }
  getProfitAndLostDetail(filter: ReportFilter): Observable<ProfitAndLostDetail> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetProfitAndLostDetail";
    return this.http.post<ProfitAndLostDetail>(url, filter);
  }
  getCustomerBalanceDetail(filter: ReportFilter): Observable<CustomerBalanceDetail> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetCustomerBalanceDetail";
    return this.http.post<CustomerBalanceDetail>(url, filter);
  }
  getCustomerBalanceSummary(filter: ReportFilter): Observable<CustomerBalanceSummary[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetCustomerBalanceSummary";
    return this.http.post<CustomerBalanceSummary[]>(url, filter);
  }
  getCustomerInvoices(filter: ReportFilter): Observable<Invoice[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetCustomerInvoices";
    return this.http.post<Invoice[]>(url, filter);
  }
  getProductSaleSummaries(filter: ReportFilter): Observable<ProductSaleSummary[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetProductSaleSummaries";
    return this.http.post<ProductSaleSummary[]>(url, filter);
  }
  getProductSaleDetail(productId: any, filter: ReportFilter): Observable<ProductSaleDetail> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/getProductSaleDetail/" + productId;
    return this.http.post<ProductSaleDetail>(url, filter);
  }
  GetProductInventoriesBalance(searchText: string): Observable<Product[]> {
    let url = this.baseService.apiUrl + "/Report/GetProductInventoriesBalance?searchText=" + searchText;
    return this.http.get<Product[]>(url);
  }
  GetProductInventoryByLocation(productId: any): Observable<Product[]> {
    let url = this.baseService.apiUrl + "/Report/GetProductInventoryByLocation/" + productId;
    return this.http.get<Product[]>(url);
  }
  getInventoryHistories(productId: any, filter: InventoryHistoryFilter): Observable<InventoryHistoryDetail[]> {
    filter.fromDate = new Date(filter.fromDate);
    filter.toDate = new Date(filter.toDate);
    let url = this.baseService.apiUrl + "/Report/GetInventoryHistories/" + productId;
    return this.http.post<InventoryHistoryDetail[]>(url, filter);
  }
  getTaxMonthly(month: any): Observable<TaxMonthly> {
    let url = this.baseService.apiUrl + "/Report/GetTaxMonthly/" + month;
    return this.http.get<TaxMonthly>(url);
  }
  
}