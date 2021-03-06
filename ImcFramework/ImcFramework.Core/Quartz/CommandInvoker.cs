﻿using ImcFramework.Commands;
using ImcFramework.Data;
using ImcFramework.Ioc;
using ImcFramework.Reflection;
using ImcFramework.WcfInterface;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImcFramework.Core.Quartz
{
    public class CommandInvoker: ICommandInvoker
    {
        private IScheduleProvider scheduleProvider;
        public CommandInvoker(IScheduleProvider scheduleProvider)
        {
            this.scheduleProvider = scheduleProvider;
        }

        private static Dictionary<ECommand, dynamic> dict = new Dictionary<ECommand, dynamic>();

        public TOutput Invoke<TOutput>(FunctionSwitch functionSwitch)
        {
            if (!dict.ContainsKey(functionSwitch.Command))
            {
                var type = GetCommandClass(functionSwitch.Command);
                dynamic instance = Activator.CreateInstance(type, new object[] { scheduleProvider.Schedule });
                dict[functionSwitch.Command] = instance;
            }

            dynamic ret = dict[functionSwitch.Command].Execute(functionSwitch);

            return (TOutput)ret;
        }

        public static Type GetCommandClass(ECommand command)
        {
            ITypeFinder typeFinder = new TypeFinder();

            var types = typeFinder.FindAll().Where(zw => zw.Name == command.ToString() + "Command");
            return types.FirstOrDefault();
        }
    }
}
