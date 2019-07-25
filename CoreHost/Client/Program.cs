using System;
using System.Collections.Generic;
using System.IO;
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

                //GET
                response = client.GetAsync("GetString").Result;
                TestResponseDto res = null;
                if (response.IsSuccessStatusCode)
                {
                    var result2 = response.Content.ReadAsStreamAsync().Result;
                    var adsad = Serializer.Deserialize<TestResponseDto>(result2);
                    Console.WriteLine("{0} test success!", "11");
                }
                else
                {
                    var message = response.Content.ReadAsStreamAsync().Result;
                    res = Serializer.Deserialize<TestResponseDto>(message);
                    Console.WriteLine("{0} ({1})\n message: {2} ", (int)response.StatusCode, response.ReasonPhrase, message);
                }

                //POST
                using (var stream=new MemoryStream())
                {
                    Serializer.Serialize(stream, res);
                    var context = new StreamContent(stream);
                    response = client.PostAsync("Input", context).Result;
                }
            }

            Console.ReadKey();
        }
    }
}
