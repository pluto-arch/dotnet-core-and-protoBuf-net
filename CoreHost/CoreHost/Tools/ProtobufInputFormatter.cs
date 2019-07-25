using System;
using System.Reflection;
using System.Threading.Tasks;
using DataModel;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ProtoBuf.Meta;

namespace CoreHost.Tools
{
    public class ProtobufInputFormatter : InputFormatter
    {
        private static Lazy<RuntimeTypeModel> model = new Lazy<RuntimeTypeModel>(CreateTypeModel);

        public static RuntimeTypeModel Model
        {
            get { return model.Value; }
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var type = context.ModelType;
            var request = context.HttpContext.Request;
            MediaTypeHeaderValue requestContentType = null;
            MediaTypeHeaderValue.TryParse(request.ContentType, out requestContentType);
            object result = Model.Deserialize(context.HttpContext.Request.Body, null, type);
            return InputFormatterResult.SuccessAsync(result);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            return true;
        }


        private static RuntimeTypeModel CreateTypeModel()
        {
            var typeModel = TypeModel.Create();
            typeModel.UseImplicitZeroDefaults = false;
            Assembly ass = Assembly.GetAssembly(typeof(IBaseDto));
            foreach (var type in ass.GetTypes())
            {
                if (type != typeof(IBaseDto) && (type.GetInterface(nameof(IBaseDto)).Name == nameof(IBaseDto)))
                {
                    typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(type);
                }
            }
            typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(TestResponseDto));
            return typeModel;
        }
    }
}