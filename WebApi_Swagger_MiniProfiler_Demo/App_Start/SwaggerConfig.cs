using System.Web.Http;
using WebActivatorEx;
using WebApi_Swagger_MiniProfiler_Demo;
using Swashbuckle.Application;
using System.IO;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApi_Swagger_MiniProfiler_Demo
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "WebApi_Swagger_MiniProfiler_Demo");
                        c.IncludeXmlComments(GetXmlCommentsPath("WebApi_Swagger_MiniProfiler_Demo"));
                        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                        c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                        c.DocumentFilter<InjectMiniProfiler>();
                    })
                .EnableSwaggerUi(c =>
                    {
                        string resourceName = thisAssembly.FullName.Substring(0, thisAssembly.FullName.IndexOf(",")) + ".Scripts.swaggerui.swagger_lang.js";
                        c.InjectJavaScript(thisAssembly, resourceName);

                        string resourceName2 = thisAssembly.FullName.Substring(0, thisAssembly.FullName.IndexOf(",")) + ".Scripts.swaggerui.SwaggerUiCustomization.js";
                        c.InjectJavaScript(thisAssembly, resourceName2);
                    });
        }
        private static string GetXmlCommentsPath(string subName)
        {
            return Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "bin\\").FirstOrDefault(n => n.ToLower().EndsWith(subName.ToLower() + ".xml"));
        }
    }
}
