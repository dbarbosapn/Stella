using System.Collections.Generic;

namespace Stella.Models
{
    public class AccumulatedRecord
    {
        public List<string> Ids = new List<string>();
        public string CustomerId { get; set; }
        public string ServiceName { get; set; }
        public int Count {
            get {
                return Ids.Count;
            }
        }

        public AccumulatedRecord(string customerId, string serviceName)
        {
            CustomerId = customerId;
            ServiceName = serviceName;
        }
    }
}
