using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ProducerCSW
{
    class Program
    {
        public static void Main(string[] args)
        {
            var hasMessageToSend = true;
            while (hasMessageToSend)
            {
                var order = AskForParameters();
                ProcessMessage(args, order);

                Console.WriteLine("Has Message to send? (Y/N)");
                if (Console.ReadLine() == "Y")
                    continue;
                else
                    break;
            }
        }

        private static Order AskForParameters()
        {
            var order = new Order();

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

        private static async Task ProcessMessage(string[] args, Order order)
        {
            var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                var orderSerialized = JsonConvert.SerializeObject(order);
                p.BeginProduce("csw-topic", new Message<Null, string> { Value = orderSerialized }, handler);
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
