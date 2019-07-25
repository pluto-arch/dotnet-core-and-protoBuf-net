using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using DataModel;
using ProtoBuf;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000/api/values/") })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
                HttpResponseMessage response = null;
                response = client.GetAsync("GetString").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result2 = response.Content.ReadAsStreamAsync().Result;
                    var adsad = Serializer.Deserialize<TestResponseDto>(result2);
                    Console.WriteLine("{0} test success!", "11");
                }
                else
                {
                    var message = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("{0} ({1})\n message: {2} ", (int)response.StatusCode, response.ReasonPhrase, message);
                }
            }

            Console.ReadKey();
        }
    }
}
