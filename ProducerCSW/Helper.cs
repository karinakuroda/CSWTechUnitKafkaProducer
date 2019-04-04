using System;
using System.ComponentModel;
using System.Globalization;

namespace ProducerCSW
{
    public static class Helper
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
    }
}
