using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace CoreHost
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        
        public static string AppName = "";
        public static int Main(string[] args)
        {
            try
            {
                var configuration = GetConfiguration();
                AppName = configuration["AppName"];
                Log.Logger = CreateSerilogLogger(configuration);
                Log.Information("配置web主机 ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);
                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 创建web服务主机
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5000")
                .CaptureStartupErrors(false) //捕获启动异常
                .UseStartup<Startup>() //使用启动类
                .UseApplicationInsights() //分析web性能
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("wwwroot")
                .UseConfiguration(configuration)
                .UseSerilog()
                .Build();

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
            log.Enrich.WithProperty("ApplicationContext", AppName);
            log.Enrich.FromLogContext();
            log.WriteTo.Console();
            log.WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate); // 写到文件，每天一个，最小记录级别是Debug，文件格式是 yyyyMMdd.log
            log.ReadFrom.Configuration(configuration); //serilog的 appsetting.json 支持


            return log.CreateLogger();
        }


        /// <summary>
        /// 进行配置
        /// </summary>
        /// <returns></returns>
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false,true)
                .AddEnvironmentVariables();
            var config = builder.Build();
            //GetValue  配置扩展  允许将配置转化为强类型  
            //            if (config.GetValue<bool>("UseVault", false))  //将UseVault 配置值转化为bool 强类型
            //            {
            //                
            //            }

            return builder.Build();
        }
    }
}
