using System;
using System.Reflection;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ProtoBuf.Meta;

namespace CoreHost.Tools
{
    public class ProtobufOutputFormatter : OutputFormatter
    {
        private static Lazy<RuntimeTypeModel> model = new Lazy<RuntimeTypeModel>(CreateTypeModel);

        public string ContentType { get; private set; }

        public static RuntimeTypeModel Model
        {
            get { return model.Value; }
        }

        public ProtobufOutputFormatter()
        {
            ContentType = "application/x-protobuf";
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-protobuf"));

            //SupportedEncodings.Add(Encoding.GetEncoding("utf-8"));
        }

        private static RuntimeTypeModel CreateTypeModel()
        {
            var typeModel = TypeModel.Create();
            typeModel.UseImplicitZeroDefaults = false;
            Assembly ass = Assembly.GetAssembly(typeof(IBaseDto));
            foreach (var type in ass.GetTypes())
            {
                if (type!=typeof(IBaseDto) &&(type.GetInterface(nameof(IBaseDto)).Name == nameof(IBaseDto)))
                {
                    typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(type);
                }
            }
//            typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(TestResponseDto));
            return typeModel;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;

            Model.Serialize(response.Body, context.Object);
            return Task.FromResult(response);
        }
    }
}