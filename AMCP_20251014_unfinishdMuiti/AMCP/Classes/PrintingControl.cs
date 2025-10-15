using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCP
{
    interface PrintingControl
    {
        // 打印工作模式（0：空闲状态；1：正在打印；2：暂停打印）
        int workMode { get; set; }

        /// <summary>
        /// 执行打印过程
        /// </summary>
        /// <param name="modelData">模型数据类对象</param>
        void Run();

        /// <summary>
        /// 暂停打印
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复打印
        /// </summary>
        void Resume();

        /// <summary>
        /// 中止打印
        /// </summary>
        void Stop();

        bool OpenCommEthernet(string Address);

        void CloseComm();


    }
}
