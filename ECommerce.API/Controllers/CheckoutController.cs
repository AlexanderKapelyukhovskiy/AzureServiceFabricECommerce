using ECommerce.API.Model;
using ECommerce.CheckoutService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private static readonly Random rnd = new Random(DateTime.UtcNow.Second);

        private ICheckoutService GetService()
        {
            long key = LongRandom();
            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            return proxyFactory.CreateNonIServiceProxy<ICheckoutService>(
                new Uri("fabric:/ECommerce/ECommerce.CheckoutService"),
                new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(key));
        }

        private long LongRandom()
        {
            byte[] buf = new byte[8];
            rnd.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return longRand;
        }

        [Route("{userId}")]
        public async Task<ApiCheckoutSummary> CheckoutAsync(string userId)
        {
            var summary = await GetService().CheckoutAsync(userId);
            return ToApiCheckout(summary);
        }

        [Route("history/{userId}")]
        public async Task<IEnumerable<ApiCheckoutSummary>> GetHistoryAsync(string userId)
        {
            var history = await GetService().GetOrderHistoryAsync(userId);

            return history.Select(ToApiCheckout);
        }

        private ApiCheckoutSummary ToApiCheckout(CheckoutSummary model)
        {
            return new ApiCheckoutSummary
            {
                Products = model.Products.Select(
                    p => new ApiCheckoutProduct
                    {
                        ProductId = p.Product.Id,
                        ProductName = p.Product.Name,
                        Price = p.Price,
                        Quantity = p.Quantity
                    }).ToList(),
                Date = model.Date,
                TotalPrice = model.TotalPrice
            };
        }
    }
}
