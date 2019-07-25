using System.Threading.Tasks;

namespace NetworkTools
{
    /// <summary>
    /// http 协议通信
    /// </summary>
    public interface IHttpServiceHelper
    {
        /// <summary>
        /// http get
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <param name="request">请求参数</param>
        /// <param name="apiMethod">接口方法名</param>
        /// <returns></returns>
        Task<TResponse> SendGetAsync<TResponse, TRequest>(TRequest request,string apiMethod) where TRequest : class where TResponse:class;

        /// <summary>
        /// http post
        /// </summary>
        /// <typeparam name="TResponse">响应类型</typeparam>
        /// <typeparam name="TRequest">请求类型</typeparam>
        /// <param name="request">请求参数</param>
        /// <param name="apiMethod">接口方法名</param>
        /// <returns></returns>
        Task<TResponse> SendPostAsync<TResponse, TRequest>(TRequest request, string apiMethod) where TRequest : class where TResponse : class;

    }
}