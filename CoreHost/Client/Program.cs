using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetworkTools;
using Newtonsoft.Json;
using ProtoBuf;
using Serilog;

namespace Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {

            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);
            Log.Information($"client启动");
            var service = ConfigureServices(configuration);
            var http = service.GetService<IHttpServiceHelper>();
            if (http!=null)
            {
               var response= await http.SendGetAsync<TestResponseDto, TestResponseDto>(new TestResponseDto
                {
                    Name = "this ",
                    FirstName = "is ",
                    LastName = "get "
                }, "GetString");

               

               Log.Information(JsonConvert.SerializeObject(response));
            }

            return 1;
            Console.ReadKey();
        }


        public static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services=new ServiceCollection();
            services.AddHttpClient();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<IHttpServiceHelper, HttpServiceHelper>();
            return services.BuildServiceProvider();
        }


        /// <summary>
        /// 进行配置
        /// </summary>
        /// <returns></returns>
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables();
            //GetValue  配置扩展  允许将配置转化为强类型  
            //            if (config.GetValue<bool>("UseVault", false))  //将UseVault 配置值转化为bool 强类型
            //            {
            //                
            //            }

            return builder.Build();
        }

        /// <summary>
        /// 配置serilog
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss.FFF} {Level}] {Message} ({SourceContext:l}){NewLine}{Exception}";
            var log = new LoggerConfiguration();
            log.MinimumLevel.Verbose();
            log.Enrich.WithProperty("ApplicationContext", "Client");
            log.Enrich.FromLogContext();
            log.WriteTo.Console();
            log.WriteTo.File(Path.Combine("logs", @"client.log"), rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate); // 写到文件，每天一个，最小记录级别是Debug，文件格式是 yyyyMMdd.log
            log.ReadFrom.Configuration(configuration); //serilog的 appsetting.json 支持
            return log.CreateLogger();
        }
    }
}
