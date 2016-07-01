﻿using ImcFramework.WcfInterface;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Autofac.Integration.Wcf;
using ImcFramework.Core.LogModule;

namespace ImcFramework.Core.WcfService
{
    public class WcfServiceModule : ServiceModuleBase
    {
        public const string MODUEL_NAME = "WcfService_Module";

        private ILoggerPool loggerPool;
        public WcfServiceModule(ILoggerPoolFactory loggerPoolFactory)
        {
            this.loggerPool = loggerPoolFactory.GetLoggerPool(MODUEL_NAME);

            Logger = loggerPool.GetLogger("");
        }

        public ServiceHost ServiceHost { get; private set; }

        public override void Initialize()
        {
            //register the logger pool
            IocManager.Register<ILoggerPool>(loggerPool, MODUEL_NAME);
        }

        public override string Name
        {
            get
            {
                return MODUEL_NAME;
            }
        }

        public override void Start()
        {
            try
            {
                ServiceHost = new ServiceHost(typeof(ClientConnectorReal));

                //autofac DI
                ServiceHost.AddDependencyInjectionBehavior<IClientConnector>(IocManager.Container);

                //绑定服务行为
                ServiceMetadataBehavior behavior = ServiceHost.Description.Behaviors.
                    Find<ServiceMetadataBehavior>();

                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior();
                    behavior.HttpGetEnabled = true;
                    ServiceHost.Description.Behaviors.Add(behavior);
                }
                else
                {
                    behavior.HttpGetEnabled = true;
                }

                //启动事件
                ServiceHost.Opened += delegate
                {
                    Logger.Info("Host-->终结点为：" + ServiceHost.Description.Endpoints.FirstOrDefault().Address);
                    Logger.Info("Host-->服务启动，开始监听：" + ServiceHost.Description.ConfigurationName);
                };

                ServiceHost.Closed += delegate
                {
                    Logger.Info("Host-->终结点为：" + ServiceHost.Description.Endpoints.FirstOrDefault().Address);
                    Logger.Info("Host-->服务关闭，Close：" + ServiceHost.Description.ConfigurationName);
                };

                Thread th = new Thread(ServiceHost.Open);
                th.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        public override void Stop()
        {
            if (ServiceHost != null)
            {
                try
                {
                    ServiceHost.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
                finally
                {
                    ServiceHost.Abort();
                }
            }
        }
    }
}
