using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProducerCSW
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (RunTestsIsTrue())
                RunTests();
            else
            {
                var hasMessageToSend = true;
                while (hasMessageToSend)
                {
                    var order = AskForParameters();
                    ProcessMessage(order);

                    Console.WriteLine("Has Message to send? (Y/N)");
                    if (Console.ReadLine().ToUpper() == "Y")
                        continue;
                    else
                        break;
                }
            }
            Console.ReadLine();
        }
        private static void RunTests()
        {
            Random rand = new Random();

            var listCountry = new List<string>();
            listCountry.Add("Brasil");
            listCountry.Add("Portugal");
            listCountry.Add("Espanha");
            listCountry.Add("Chile");
            listCountry.Add("Argentina");

            Console.WriteLine("Type the number of messages to send:");
            var total= Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < total; i++)
            {
                var order = new OrderMessage();
                order.Id = i;
                order.MerchantId = i;
                order.Amount = 100;
                order.CountryDestination = listCountry[rand.Next(listCountry.Count)];
                order.CountryOrigin= listCountry[rand.Next(listCountry.Count)];
                order.NumberOfItems = rand.Next(100);

                ProcessMessage(order);
            }
        }

        private static bool RunTestsIsTrue()
        {
            Console.WriteLine("Run Tests? (Y/N)");
            return (Console.ReadLine().ToUpper()== "Y");
        }

        private static OrderMessage AskForParameters()
        {
            var order = new OrderMessage();

            Console.WriteLine("Type the order ID:");
            order.Id = Helper.TryParse<int>(Console.ReadLine());
            Console.WriteLine("Type the Merchant ID:");
            order.MerchantId = Helper.TryParse<int>(Console.ReadLine());
            Console.WriteLine("Type the Country Origin:");
            order.CountryOrigin = Helper.TryParse<string>(Console.ReadLine());
            Console.WriteLine("Type the Country Destination:");
            order.CountryDestination = Helper.TryParse<string>(Console.ReadLine());
            Console.WriteLine("Type the Amount:");
            order.Amount = Helper.TryParse<decimal>(Console.ReadLine());
            Console.WriteLine("Type the Number of Items:");
            order.NumberOfItems = Helper.TryParse<int>(Console.ReadLine());

            return order;

        }

        private static async Task ProcessMessage(OrderMessage order)
        {
            var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                var orderSerialized = JsonConvert.SerializeObject(order);
                if (order.CountryDestination.Contains("Portugal"))
                {
                    p.BeginProduce("csw-topic-portugal", new Message<Null, string> { Value = orderSerialized }, handler);
                }
                else if (order.CountryDestination.Contains("Espanha"))
                {
                    p.BeginProduce("csw-topic-espanha", new Message<Null, string> { Value = orderSerialized }, handler);
                }
                else
                {
                    p.BeginProduce("csw-topic", new Message<Null, string> { Value = orderSerialized }, handler);
                }
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
