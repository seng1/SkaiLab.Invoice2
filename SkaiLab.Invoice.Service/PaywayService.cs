using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using SkaiLab.Invoice.Models.Payway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SkaiLab.Invoice.Service
{
    public class PaywayService:PaymentService,IPaywayService
    {
        public PaywayService(IDataContext context) : base(context)
        {

        }

        public async Task ProcessPayAsync(RequestComplete requestComplete)
        {
            using var context = Context();
            var payment = context.UserPaymentPayway.FirstOrDefault(u => u.TransactionId == requestComplete.TransactionId);
            TransactionRequest transactionRequest = new TransactionRequest
            {
                Date = CurrentCambodiaTime.ToString("yyyy-MM-dd"),
                Status= "APPROVED"
            };
            var transactionHeader = await GetTransactionsAsync(transactionRequest);
            var transaction = transactionHeader.Transactions.FirstOrDefault(u => u.OrderId == requestComplete.TransactionId);
            if (transaction == null)
            {
                throw new Exception("Cannot request to get transaction");
            }
            if (Math.Round(transaction.TotalAmount,0) !=Math.Round(payment.IdNavigation.TotalIncludeTax,0))
            {
                throw new Exception("User try to hack amount");
            }
            payment.IdNavigation.IsPaid = true;
            var userPlan = context.UserPlan.FirstOrDefault(u => u.UserId == payment.IdNavigation.UserId);
            if (userPlan.Expire == null || userPlan.IsTrail)
            {
                userPlan.Expire = payment.IdNavigation.SubscriptionTypeId == 1 ? CurrentCambodiaTime.AddMonths(1) : CurrentCambodiaTime.AddYears(1);
            }
            else
            {

                userPlan.Expire = payment.IdNavigation.SubscriptionTypeId == 1 ? userPlan.Expire.Value.AddMonths(1) : userPlan.Expire.Value.AddYears(1);
            }
            userPlan.IsTrail = false;
            payment.IdNavigation.UserPaymentInvoice = new Dal.Models.UserPaymentInvoice
            {
                Date=CurrentCambodiaTime,
                Year=CurrentCambodiaTime.Year,
                Number=GenerateInvoiceNumber(context,payment.IdNavigation.IsTax,CurrentCambodiaTime)
            };
            payment.IdNavigation.UserPaymentPayWayDetail = new Dal.Models.UserPaymentPayWayDetail
            {
                Apv= transaction.Apv,
                Amount= transaction.TotalAmount,
                Date= transaction.Date,
                PaidBy= transaction.Name,
                PaymentType= transaction.PaymentType,
                Phone= transaction.Phone,
                SourceOfFund=transaction.SourceOfFund
            };
            context.SaveChanges();
        }
        public async Task<CheckTransaction> CheckTransactionAsync(RequestComplete request)
        {

            RequestCheckTransaction requestCheckTransaction = new RequestCheckTransaction();
            requestCheckTransaction.Hash =Models.Utils.SHA512_ComputeHash($"{Option.PayWay.MerchantId}{request.TransactionId}", Option.PayWay.ApiKey);
            requestCheckTransaction.TransactionId = request.TransactionId;
            var endPoint = $"{Option.PayWay.ApiUrl}check/transaction/";
            var response = await CallApiAsync(JsonConvert.SerializeObject(requestCheckTransaction), endPoint);
            return JsonConvert.DeserializeObject<CheckTransaction>(response);
        }

        public async Task<TransactionHeader> GetTransactionsAsync(TransactionRequest request)
        {
            request.Hash = Models.Utils.SHA512_ComputeHash($"{Option.PayWay.MerchantId}{request.Date}", Option.PayWay.ApiKey);
            var endPoint = $"{Option.PayWay.ApiUrl}get/transactions/";
            var response = await CallApiAsync(JsonConvert.SerializeObject(request), endPoint);
            return JsonConvert.DeserializeObject<TransactionHeader>(response);
        }
        async Task<string> CallApiAsync(string jsonBody, string urlEndPoint)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    //client.DefaultRequestHeaders.Referrer = new Uri(baseUrl);

                    var dicData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBody);
                    var content = new FormUrlEncodedContent(dicData.AsEnumerable());

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, urlEndPoint) { Content = content };

                    var respondMessage = await client.SendAsync(requestMessage);

                    respondMessage.EnsureSuccessStatusCode();

                    var respondString = await respondMessage.Content.ReadAsStringAsync();

                    if (respondMessage.IsSuccessStatusCode)
                        return respondString;

                    return $"{(int)respondMessage.StatusCode}-{respondMessage.ReasonPhrase}";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
    }
}
