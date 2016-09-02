using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WcfServices.Contract;

namespace ServiceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //协议
            ContractDescription cd = ContractDescription.GetContract(typeof(ICalculator));//new ContractDescription("ICalculator")
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.BasicHttpBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.CustomBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.NetNamedPipeBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.NetPeerTcpBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.NetTcpBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.UdpBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.WebHttpBinding);
            //Binding binding = BindingFactory.CreateBinding(BindingTypes.WSDualHttpBinding);
            //绑定
            Binding binding = BindingFactory.CreateBinding(BindingTypes.WSHttpBinding);
            //地址
            EndpointAddress ea = new EndpointAddress("http://localhost:12993/Service.svc");
            ServiceEndpoint se = new ServiceEndpoint(cd, binding, ea);

            using (ChannelFactory<ICalculator> factory = new ChannelFactory<ICalculator>(se))
            {
                ICalculator proxy = factory.CreateChannel();
                Console.WriteLine(proxy.Add(1, 2));
            }

            using(WCFManager<ICalculator> manager = new WCFManager<ICalculator>(se))
            {
                ICalculator proxy = manager.GetProxy();
                Console.WriteLine(proxy.Add(20, 34));
            }

            WSManager.Run("http://localhost:8088/PADWebService.asmx");
            Console.ReadKey();
        }
    }
}
