using System;
using System.Net.Http;

namespace NoOperationId
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var httpClient = new HttpClient())
            {
                var apiClient = new swaggerClient(httpClient);

                apiClient.PortsOfEntryAllAsync();               // gets all
                apiClient.PortsOfEntry2Async(Guid.Empty);       // gets one
                apiClient.PortsOfEntry3Async(Guid.Empty, null); // update
                apiClient.PortsOfEntryAsync(null);              // create
            }
        }
    }
}
