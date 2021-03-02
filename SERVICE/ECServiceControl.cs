using log4net;
using Quartz;
using Quartz.Impl;
using SleckEc2KdInterface.JOB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace SleckEc2KdInterface.SERVICE
{
    public class EcServiceControl : ServiceControl, ServiceSuspend, ServiceShutdown

    {
        //测试
        private static string Q_Interval = ConfigurationManager.AppSettings["Q_Interval"].ToString();
        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IScheduler scheduler;
        public async Task Init()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

           

            //job方法一
            var job = JobBuilder.Create<EcJob>()
                                                .StoreDurably(true)
                                                .Build();

            var trigger = TriggerBuilder.Create()
                                                   .StartAt(DateTimeOffset.Now.AddSeconds(1))//开始执行时间
                                                   .EndAt(DateTimeOffset.Now.AddDays(1500))//执行结束时间
                                                   .WithCronSchedule("00 00 23 * * ?").Build();//每天23:00:00分执行

            //var trigger = TriggerBuilder.Create()
            //                                    .StartAt(DateTimeOffset.Now.AddSeconds(1))//开始执行时间
            //                                    .EndAt(DateTimeOffset.Now.AddDays(1500))//执行结束时间
            //                                    .WithSimpleSchedule(x => x
            //                                                            .WithIntervalInMinutes(int.Parse(Q_Interval))//30分钟钟执行一次
            //                                                            .RepeatForever()//重复执行
            //                                                            .Build()).Build();




            await scheduler.ScheduleJob(job, trigger);
            //[start 会让 调度线程 启动，从jobstore中获取快要执行的trigger，然后获取trigger关联的job进行执行。]
            await scheduler.Start();


        }

        public EcServiceControl()
        {

            Init().GetAwaiter().GetResult();


        }
        bool ServiceControl.Start(HostControl hostControl)
        {

            scheduler.Start();
            Logger.InfoFormat("服务已开启，PID：{0}，TID：{1}，调用 ServiceControl.Start(HostControl hostControl) 方法", GetProcessId(), GetThreadId());

            return true;

        }



        bool ServiceControl.Stop(HostControl hostControl)
        {


            scheduler.Shutdown(false);
            Logger.InfoFormat("服务已停止，PID：{0}，TID：{1}，调用 ServiceControl.Stop(HostControl hostControl) 方法", GetProcessId(), GetThreadId());

            return true;

        }



        bool ServiceSuspend.Pause(HostControl hostControl)
        {

            scheduler.PauseAll();
            Logger.InfoFormat("服务已暂停，PID：{0}，TID：{1}，调用 ServiceControl.Pause(HostControl hostControl) 方法", GetProcessId(), GetThreadId());

            return true;

        }



        bool ServiceSuspend.Continue(HostControl hostControl)
        {

            scheduler.ResumeAll();
            Logger.InfoFormat("服务继续运行，PID：{0}，TID：{1}，调用 ServiceControl.Continue(HostControl hostControl) 方法", GetProcessId(), GetThreadId());

            return true;

        }



        void ServiceShutdown.Shutdown(HostControl hostControl)
        {

            scheduler.Shutdown(false);
            Logger.InfoFormat("服务已关闭，PID：{0}，TID：{1}，调用 ServiceShutdown.Shutdown(HostControl hostControl) 方法", GetProcessId(), GetThreadId());

        }

        public static int GetProcessId()
        {

            return System.Diagnostics.Process.GetCurrentProcess().Id;

        }





        public static int GetThreadId()

        {

            return System.Threading.Thread.CurrentThread.ManagedThreadId;

        }

    }
}
