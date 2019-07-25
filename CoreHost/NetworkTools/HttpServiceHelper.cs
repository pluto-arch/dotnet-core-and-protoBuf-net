using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ProtoBuf;

namespace NetworkTools
{
    /// <summary>
    /// 通讯帮助类
    /// </summary>
    public  class HttpServiceHelper : IHttpServiceHelper
    {

        public static string baseUrl { get; set; }
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpServiceHelper(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            baseUrl = config["ServerApiGetWay"];
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 发送httpGet 请求 使用protobuf序列化
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        /// <param name="apiMethod"></param>
        /// <returns></returns>
        public async Task<TResponse> SendGetAsync<TResponse, TRequest>(TRequest request,string apiMethod)
            where TResponse : class
            where TRequest : class
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress=new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
            var response = await httpClient.GetAsync(apiMethod);
            var responseStream = await response.Content.ReadAsStreamAsync();
            return Serializer.Deserialize<TResponse>(responseStream);
        }

        /// <summary>
        /// 发送httpost请求 使用protobuf序列化
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        /// <param name="apiMethod"></param>
        /// <returns></returns>
        public async Task<TResponse> SendPostAsync<TResponse, TRequest>(TRequest request, string apiMethod)
            where TResponse : class
            where TRequest : class
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
            using (var stream=new MemoryStream())
            {
                Serializer.Serialize(stream, request);
                var context = new StreamContent(stream);
                var response = await httpClient.PostAsync("Input", context);
                var responseStream = await response.Content.ReadAsStreamAsync();
                return Serializer.Deserialize<TResponse>(responseStream);
            }
            
        }
    }
}