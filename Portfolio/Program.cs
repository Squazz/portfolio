using BinanceExchange.API.Client;
using BinanceExchange.API.Market;
using BinanceExchange.API.Models.Request;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Portfolio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AsyncCall().Wait();
        }

        private static async Task AsyncCall()
        {
            string ApiKey = "KEY";
            string SecretKey = "SECRET";

            Console.WriteLine("--------------------------");
            Console.WriteLine("BinanceExchange API - Tester");
            Console.WriteLine("--------------------------");

            //Initialise the general client client with config
            var client = new BinanceClient(new ClientConfiguration()
            {
                ApiKey = ApiKey,
                SecretKey = SecretKey,
                //Logger = logger,
            });

            Console.WriteLine("Interacting with Binance...");

            // Test the Client
            await client.TestConnectivity();
            
            var accountInformation = await client.GetAccountInformation(receiveWindow: 3500); // Get accountinfo

            var balances = accountInformation.Balances.Where(x => x.Free + x.Locked > 0);

            List<BinanceExchange.API.Models.Response.OrderResponse> test = new List<BinanceExchange.API.Models.Response.OrderResponse>();

            foreach (var balance in balances)
            {
                try
                {
                    var data = await client.GetAllOrders(new AllOrdersRequest()
                    {
                        Symbol = balance.Asset + "BTC"
                    });
                    test.AddRange(data);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            var stop = 0;
        }
    }
}
