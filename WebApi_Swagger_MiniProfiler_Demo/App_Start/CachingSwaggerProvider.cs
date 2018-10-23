using Swashbuckle.Swagger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using WebGrease.Css.Extensions;

namespace WebApi_Swagger_MiniProfiler_Demo
{
    public class CachingSwaggerProvider : ISwaggerProvider
    {
        private static ConcurrentDictionary<string, SwaggerDocument> _cache =
            new ConcurrentDictionary<string, SwaggerDocument>();

        private readonly ISwaggerProvider _swaggerProvider;

        public CachingSwaggerProvider(ISwaggerProvider swaggerProvider)
        {
            _swaggerProvider = swaggerProvider;
        }

        public SwaggerDocument GetSwagger(string rootUrl, string apiVersion)
        {
            var cacheKey = string.Format("{0}_{1}", rootUrl, apiVersion);
            SwaggerDocument srcDoc = null;
            //只读取一次
            if (!_cache.TryGetValue(cacheKey, out srcDoc))
            {
                srcDoc = _swaggerProvider.GetSwagger(rootUrl, apiVersion);

                srcDoc.paths.ForEach(n =>
                {
                    SetOperation(n.Value.delete);
                    SetOperation(n.Value.get);
                    SetOperation(n.Value.head);
                    SetOperation(n.Value.options);
                    SetOperation(n.Value.patch);
                    SetOperation(n.Value.post);
                    SetOperation(n.Value.put);
                });

                srcDoc.vendorExtensions = new Dictionary<string, object> { { "ControllerDesc", GetControllerDesc() } };
                _cache.TryAdd(cacheKey, srcDoc);
            }
            return srcDoc;
        }

        private void SetOperation(Operation operation)
        {
            if (operation != null)
            {
                operation.tags[0] = operation.tags[0].Split('_').Last();

                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }

                if (operation.operationId.StartsWith("Import"))
                {
                    operation.parameters.Add(new Parameter()
                    {
                        @in = "formData",
                        name = "",
                        type = "file"
                    });
                    if (operation.consumes == null)
                    {
                        operation.consumes = new List<string>();
                    }
                    operation.consumes.Add("multipart/form-data");
                }
            }
        }

        /// <summary>
        /// 从API文档中读取控制器描述
        /// </summary>
        /// <returns>所有控制器描述</returns>
        public static ConcurrentDictionary<string, string> GetControllerDesc()
        {
            ConcurrentDictionary<string, string> controllerDescDict = new ConcurrentDictionary<string, string>();

            GetControllerDescByProject("SwaggerDemo", "Controller", controllerDescDict);
            GetControllerDescByProject("Application", "AppService", controllerDescDict);

            return controllerDescDict;
        }

        private static void GetControllerDescByProject(string subName, string endsWith, ConcurrentDictionary<string, string> controllerDescDict)
        {
            string xmlpath = Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "bin\\").FirstOrDefault(n => n.ToLower().EndsWith(subName.ToLower() + ".xml"));
            if (File.Exists(xmlpath))
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(xmlpath);
                string type = string.Empty, path = string.Empty, controllerName = string.Empty;

                string[] arrPath;
                int length = -1, cCount = endsWith.Length;
                XmlNode summaryNode = null;
                foreach (XmlNode node in xmldoc.SelectNodes("//member"))
                {
                    type = node.Attributes["name"].Value;
                    if (type.StartsWith("T:"))
                    {
                        //控制器
                        arrPath = type.Split('.');
                        length = arrPath.Length;
                        controllerName = arrPath[length - 1];
                        if (controllerName.EndsWith(endsWith))
                        {
                            //获取控制器注释
                            summaryNode = node.SelectSingleNode("summary");
                            string key = controllerName.StartsWith("I") ?
                                controllerName.Substring(1, 1).ToLower() + controllerName.Substring(2, controllerName.Length - 2 - cCount) :
                                controllerName.Remove(controllerName.Length - cCount, cCount);
                            if (summaryNode != null && !string.IsNullOrEmpty(summaryNode.InnerText) && !controllerDescDict.ContainsKey(key))
                            {
                                controllerDescDict.TryAdd(key, summaryNode.InnerText.Trim());
                            }
                        }
                    }
                }
            }
        }
    }
}