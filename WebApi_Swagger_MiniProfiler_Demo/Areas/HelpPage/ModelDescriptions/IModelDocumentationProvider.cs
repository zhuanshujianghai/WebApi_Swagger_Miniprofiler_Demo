using System;
using System.Reflection;

namespace WebApi_Swagger_MiniProfiler_Demo.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}