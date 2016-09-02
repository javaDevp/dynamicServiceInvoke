using System;
using System.Reflection;
using System.ServiceModel.Channels;

namespace ServiceDemo
{
    /// <summary>
    /// 绑定工厂
    /// </summary>
    public class BindingFactory
    {
        public static Binding CreateBinding(BindingTypes type)
        {
            try { 
                Binding bind;
                string typeName = Enum.GetName(typeof(BindingTypes), type);
                //Assembly assembly = Assembly.GetExecutingAssembly();
                if(type == BindingTypes.WebHttpBinding)
                {
                    //需要程序集在bin目录下
                    Type t = Type.GetType(string.Format("System.ServiceModel.{0},System.ServiceModel.Web", typeName));
                    bind = Activator.CreateInstance(t, true) as Binding;
                }
                else { 
                    Type t = Type.GetType(string.Format("System.ServiceModel.{0},System.ServiceModel", typeName));
                    bind = Activator.CreateInstance(t, true) as Binding;
                }
                return bind;
            }catch(Exception ex)
            {
                return null;
            }
        }
    }

    public enum BindingTypes
    {
        //CustomBinding,
        BasicHttpBinding,
        NetNamedPipeBinding,
        NetPeerTcpBinding,
        NetTcpBinding,
        UdpBinding,
        WebHttpBinding,
        WSDualHttpBinding,
        WSHttpBinding
    }
}