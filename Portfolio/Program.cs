using BinanceExchange.API.Client;
using BinanceExchange.API.Models.Request;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using LogLevel = NLog.LogLevel;

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
            //Logging Configuration. 
            //Ensure that `nlog.config` is configured as you want, and is copied to output directory.
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            //This utilises the nlog.config from the build directory
            loggerFactory.ConfigureNLog("nlog.config");
            //For the sakes of this example we are outputting only fatal logs, debug being the lowest.
            LogManager.GlobalThreshold = LogLevel.Debug;
            var logger = LogManager.GetLogger("*");

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
                Logger = logger,
            });

            Console.WriteLine("Interacting with Binance...");

            // Test the Client
            await client.TestConnectivity();
            
            var accountInformation = await client.GetAccountInformation(receiveWindow: 3500); // Get accountinfo

            var balances = accountInformation.Balances.Where(x => x.Free + x.Locked > 0);

            List<BinanceExchange.API.Models.Response.OrderResponse> test = new List<BinanceExchange.API.Models.Response.OrderResponse>();

            var EosOrders = await client.GetAllOrders(new AllOrdersRequest()
            {
                Symbol = "EOSBTC"
            });


            var TrxOrders = await client.GetAllOrders(new AllOrdersRequest()
            {
                Symbol = "TRXBTC"
            });

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
        }
    }
}
