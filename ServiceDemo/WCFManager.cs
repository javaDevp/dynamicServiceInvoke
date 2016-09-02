using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDemo
{
    public class WCFManager<T> : ClientBase<T> where T: class
    {
        #region Constructors
        public WCFManager()
            :base()
        {

        }

        /// <summary>
        /// 通过配置节点名称初始化
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public WCFManager(string endpointConfigurationName)
            :base(endpointConfigurationName)
        {

        }

        /// <summary>
        /// 通过绑定&地址初始化
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public WCFManager(Binding binding, EndpointAddress remoteAddress)
            :base(binding, remoteAddress)
        {

        }

        /// <summary>
        /// 通过服务终结点初始化
        /// </summary>
        /// <param name="endpoint"></param>
        public WCFManager(ServiceEndpoint endpoint)
            : base(endpoint)
        {

        }

        /// <summary>
        /// 通过服务终结点初始化&回调上下文
        /// </summary>
        /// <param name="endpoint"></param>
        public WCFManager(InstanceContext callbackInstance, ServiceEndpoint endpoint)
            :base(callbackInstance, endpoint)
        {

        }

        #endregion

        public T GetProxy()
        {
            return Channel;
        }
    }

    public enum ChannelTypes
    {
        ChannelFactory,
        ConfigurationChannelFactory,
        DuplexChannelFactory,
        WebChannelFactory
    }
}
