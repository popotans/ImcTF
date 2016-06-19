﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImcFramework.WcfInterface;
using Quartz;
using System.Threading;
using Common.Logging;

namespace ImcFramework.Core.BuildInBusinessService
{
    public class ImcTFSampleService : MainBusinessBase
    {
        public override EServiceType ServiceType
        {
            get
            {
                return new EServiceType("ImcTFSample", "框架示例服务");
            }
        }

        public override void Execute(IJobExecutionContext context)
        {
            NotifyAndLog("开始！", LogLevel.Info);

            Thread.Sleep(2000);

            NotifyAndLog("结束！", LogLevel.Info);
        }
    }
}