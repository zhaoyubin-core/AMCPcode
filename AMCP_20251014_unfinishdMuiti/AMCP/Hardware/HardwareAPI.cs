using System;
using Advantech.Adam;
using Advantech.Common;
using NationalInstruments.VisaNS;
//using NationalInstruments.Common;

namespace AMCP
{
    public class ADAM
    {
        private int m_iCom, AI_iAddr, AO_iAddr, m_iChTotal;
        public bool m_bStart;
        private byte[] m_byRange;
        private Adam4000Config m_adamConfig;
        private Adam4000Type m_Adam4000Type;
        private AdamCom adamCom;

        public ADAM()
        {
            m_iCom = 3;     // using COM3
            AI_iAddr = 2;	// IN ，the slave address is 1
            AO_iAddr = 1;   //通道out
            m_bStart = false;
            m_Adam4000Type = Adam4000Type.Adam4017P; // the sample is for ADAM-4017P
            m_iChTotal = AnalogInput.GetChannelTotal(m_Adam4000Type);
            m_byRange = new byte[m_iChTotal];
        }

        public void SetPortNum(int portNum)
        {
            this.m_iCom = portNum;
        }

        public bool IsOpen()
        {
            if (adamCom != null)
            {
                return adamCom.IsOpen;
            }
            else
            {
                return false;
            }
        }

        public bool Connect()
        {
            adamCom = new AdamCom(m_iCom);
            adamCom.Checksum = false; // disbale checksum
            if (adamCom.OpenComPort())
            {
                // set COM port state, 9600,N,8,1
                adamCom.SetComPortState(Baudrate.Baud_9600, Databits.Eight, Advantech.Common.Parity.None, Stopbits.One);
                // set COM port timeout
                adamCom.SetComPortTimeout(500, 500, 0, 500, 0);
                // get module config
                if (!adamCom.Configuration(AI_iAddr).GetModuleConfig(out m_adamConfig))
                {
                    m_bStart = false;
                    adamCom.CloseComPort();
                    return false;
                }
                //
                //
                m_bStart = true; // starting flag
            }
            else
            {
                m_bStart = false;
                adamCom.CloseComPort();
            }
            return m_bStart;
        }

        public bool Disconnect()
        {
            return adamCom.CloseComPort();
        }

        public float[] GetAIValues()
        {
            float[] fVals;
            Adam4000_ChannelStatus[] status;

            if (adamCom.AnalogInput(AI_iAddr).GetValues(8, out fVals, out status))
            {
                return fVals;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定通道的模拟输入值
        /// </summary>
        /// <param name="chNo">通道号</param>
        /// <returns>电压值</returns>
        public double GetAIValue(int chNo)
        {
            float fVal;
            Adam4000_ChannelStatus status;

            if (adamCom.AnalogInput(AI_iAddr).GetValue(chNo, out fVal, out status))
            {
                if (status == Adam4000_ChannelStatus.Normal)
                {
                    return fVal;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取指定通道的模拟输出值
        /// </summary>
        /// <param name="chNo">通道号</param>
        /// <returns>电压值</returns>
        public double GetAOValue(int chNo)
        {
            float fValue;
            string szFormat;

            if (adamCom.AnalogOutput(AO_iAddr).GetCurrentValue(chNo, out fValue))
            {
                fValue = (float)Math.Round(fValue, 2);
                return fValue;
                //szFormat = AnalogOutput.GetFloatFormat(m_Adam4000Type, m_byRange[chNo]);
                //i_txtCh.Text = fValue.ToString(szFormat) + " " + AnalogOutput.GetUnitName(m_Adam4000Type, m_byRange[chNo]);
            }
            else
            {
                return 0;
            }
        }

        public void SetAOValue(int chNo, double value)
        {
            try
            {
                float fValue = (float)value;
               if (adamCom.AnalogOutput(AO_iAddr).SetCurrentValue(chNo, (float)value))
                {
                    //System.Threading.Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class CVisaOpt
    {
        private MessageBasedSession mbSession;
        private ResourceManager mRes = null;              //资源管理

        public static string[] ResourceArray = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="strRes"></param>
        /// <returns></returns>
        ///
        public CVisaOpt()
        {
        }

        /// <summary>
        /// 静态函数，查找仪器资源
        /// </summary>
        /// <param name="strRes"></param>
        /// <returns></returns>
        public string[] FindResource(string strRes)
        {
            //string[] VisaRes = new string[1];
            try
            {
                mRes = null;
                mRes = ResourceManager.GetLocalManager();
                if (mRes == null)
                {
                    //throw new Exception("本机未安装Visa的.Net支持库！");
                }
                ResourceArray = mRes.FindResources(strRes);

                //mRes.Open();
            }
            catch (System.ArgumentException)
            {
                ResourceArray = new string[1];
                ResourceArray[0] = "未能找到可用资源!";
            }
            return ResourceArray;
        }

        /// <summary>
        /// 打开资源
        /// </summary>
        /// <param name="strResourceName"></param>
        public void OpenResource(string strResourceName)
        {
            //若资源名称为空，则直接返回
            if (strResourceName != null)
            {
                try
                {

                    mRes = ResourceManager.GetLocalManager();
                    mbSession = (MessageBasedSession)mRes.Open(strResourceName);
                    //此资源的超时属性
                    //setOutTime(5000);
                    mbSession.Timeout = 2000;
                }
                catch (NationalInstruments.VisaNS.VisaException e)
                {
                    // Global.LogAdd(e.Message);
                }
                catch (Exception exp)
                {
                    //Global.LogAdd(exp.Message);
                    //throw new Exception("VisaCtrl-VisaOpen\n" + exp.Message);
                }
            }
        }


        /// <summary>
        /// 写命令函数
        /// </summary>
        /// <param name="strCommand"></param>
        public void Write(string strCommand)
        {
            try
            {
                if (mbSession != null)
                {
                    mbSession.Write(strCommand);
                }
            }
            catch (NationalInstruments.VisaNS.VisaException e)
            {
                //Global.LogAdd(e.Message);
            }
            catch (Exception exp)
            {
                throw new Exception("VisaCtrl-VisaOpen\n" + exp.Message);
            }
        }


        /// <summary>
        /// 检测是否有返回消息
        /// </summary>
        /// <returns></returns>
        public bool IsMessageAvailable()
        {
            try
            {
                //return (mbSession.ReadStatusByte() == StatusByteFlags.MessageAvailable);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 读取返回值函数
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            try
            {
                if (mbSession != null)
                {
                    return mbSession.ReadString();
                }
            }
            catch (NationalInstruments.VisaNS.VisaException)
            {
                return "NULL";
            }
            return Convert.ToString(0);
        }


        /// <summary>
        /// 设置超时时间
        /// </summary>
        /// <param name="time">MS</param>
        public void SetOutTime(int time)
        {
            mbSession.Timeout = time;
        }

        /// <summary>
        /// 释放会话
        /// </summary>
        public void Release()
        {
            if (mbSession != null)
            {
                mbSession.Dispose();
            }
        }
    }


}
