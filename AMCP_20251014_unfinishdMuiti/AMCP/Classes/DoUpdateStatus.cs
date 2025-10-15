using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;


namespace AMCP
{
    public class DoUpdateStatus
    {
        BackgroundWorker bgWorker = new BackgroundWorker();

        public long BackgoundTotal { get; private set; }
        public bool CompletedNormally { get; private set; }

        // 构造函数
        public DoUpdateStatus()
        {
            // 设置Backgoundworker属性
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;

            // 把处理程序连接到BackgroundWorker对象
            bgWorker.DoWork += DoWork_Handler;
            bgWorker.ProgressChanged += ProgressChanged_Handler;
            bgWorker.RunWorkerCompleted += RunWorkerCompleted_Handler;

        }

        // 执行后台线程
        public void StartWorker()
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync();
            }
        }

        public void DoWork_Handler(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // 进行后台处理
            long total = 0;
            for (int i = 0; i < 100; i++)
            {
                // 每次都检查是否取消
                if (worker.CancellationPending)
                {
                    args.Cancel = true;
                    worker.ReportProgress(-1);
                    break;
                }
                else
                {
                    // 如果没有取消则继续处理
                    total += i;
                    worker.ReportProgress(i);
                    args.Result = i + 1;
                    Thread.Sleep(100);
                }
            }

            // 处理完毕，存储结果并输出。
            args.Result = -1;
        }

        // 处理后台线程的输入
        private void ProgressChanged_Handler(object sender, ProgressChangedEventArgs args)
        {
            BackgoundTotal = args.ProgressPercentage;

        }

        // 后台线程完成之后，保存结果。
        private void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs args)
        {
            CompletedNormally = !args.Cancelled;
            //BackgoundTotal = args.Cancelled ? 0 : (long)args.Result;
        }

        public void Cancel()
        {
            if (bgWorker.IsBusy)
                bgWorker.CancelAsync();
        }
    }
}
