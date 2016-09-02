using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ServiceDemo
{
    public class WSManager
    {
        //public static object CallWebService(string webServiceAsmx, string serviceName, string methodName, object[] args = null)
        //{
        //    WebClient client = new WebClient();

        //    Stream stream = client.OpenRead(webServiceAsmx + "?wsdl");
        //    //读取wsdl描述文件
        //    ServiceDescription description = ServiceDescription.Read(stream);
        //    ServiceDescriptionImporter importer = new ServiceDescriptionImporter();

        //    importer.ProtocolName = "Soap12";
        //    importer.AddServiceDescription(description, null, null);
        //    importer.Style = ServiceDescriptionImportStyle.Client;
        //    importer.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;

        //}

        //[PermissionSetAttribute(SecurityAction.Demand, Name = "Full Trust")]
        /// <summary>
        /// 自动生成代理类，输出到控制台
        /// </summary>
        /// <param name="webServiceAsmx"></param>
        public static void Run(string webServiceAsmx)
        {
            WebClient client = new WebClient();

            Stream stream = client.OpenRead(webServiceAsmx + "?wsdl");
            // Get a WSDL file describing a service.
            ServiceDescription description = ServiceDescription.Read(stream);

            // Initialize a service description importer.
            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
            importer.ProtocolName = "Soap12";  // Use SOAP 1.2.
            importer.AddServiceDescription(description, null, null);

            // Report on the service descriptions.
            Console.WriteLine("Importing {0} service descriptions with {1} associated schemas.",
                              importer.ServiceDescriptions.Count, importer.Schemas.Count);

            // Generate a proxy client.
            importer.Style = ServiceDescriptionImportStyle.Client;

            // Generate properties to represent primitive values.
            importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            // Initialize a Code-DOM tree into which we will import the service.
            CodeNamespace nmspace = new CodeNamespace();
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(nmspace);

            // Import the service into the Code-DOM tree. This creates proxy code
            // that uses the service.
            ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit);

            if (warning == 0)
            {
                // Generate and print the proxy code in C#.
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                provider.GenerateCodeFromCompileUnit(unit, Console.Out, new CodeGeneratorOptions());
            }
            else
            {
                // Print an error message.
                Console.WriteLine(warning);
            }
        }

        static object Call(string url, string servername, string methodname, params object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";

            try
            { //获取WSDL 
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.ProtocolName = "Soap";
                sdi.Style = ServiceDescriptionImportStyle.Client;	//生成客户端代理						
                sdi.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;
                sdi.AddServiceDescription(sd, null, null);//添加WSDL文档

                //@namespace = string.Format(@namespace, sd.Services[0].Name);
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码 
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                DiscoveryClientProtocol dcp = new DiscoveryClientProtocol();
                dcp.DiscoverAny(url);
                dcp.ResolveAll();
                foreach (object osd in dcp.Documents.Values)
                {
                    if (osd is ServiceDescription) sdi.AddServiceDescription((ServiceDescription)osd, null, null); ;
                    if (osd is XmlSchema) sdi.Schemas.Add((XmlSchema)osd);
                }
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();
                //设定编译参数 
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                //编译代理类 
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (!cr.Errors.HasErrors)
                {
                    Assembly asm = cr.CompiledAssembly;
                    Type t = asm.GetType(@namespace + "." + servername); // 如果在前面为代理类添加了命名空间，此处需要将命名空间添加到类型前面。

                    object o = Activator.CreateInstance(t);
                    MethodInfo method = t.GetMethod(methodname);
                    return method.Invoke(o, args);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }
    }
}
