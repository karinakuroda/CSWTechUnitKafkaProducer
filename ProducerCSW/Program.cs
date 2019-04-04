using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;

namespace ProducerCSW
{
    public class Order
    {
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public string CountryOrigin { get; set; }
        public string CountryDestination { get; set; }
        public decimal Amount { get; set; }
        public int NumberOfItems { get; set; }

    }

    class Program
    {
        public static T TryParse<T>(string inValue)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid type!");
                throw new InvalidCastException();
            }
        }

        public static void Main(string[] args)
        {
            var order = new Order();

            Console.WriteLine("Type the order ID:");
            order.Id = TryParse<int>(Console.ReadLine());
            Console.WriteLine("Type the Merchant ID:");
            order.MerchantId = TryParse<int>(Console.ReadLine());
            Console.WriteLine("Type the Country Origin:");
            order.CountryOrigin = TryParse<string>(Console.ReadLine());
            Console.WriteLine("Type the Country Destination:");
            order.CountryDestination = TryParse<string>(Console.ReadLine());
            Console.WriteLine("Type the Amount:");
            order.Amount = TryParse<decimal>(Console.ReadLine());
            Console.WriteLine("Type the Number of Items:");
            order.NumberOfItems = TryParse<int>(Console.ReadLine());

            Process(args, order);
        }
        public static async Task Process(string[] args, Order order)
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
