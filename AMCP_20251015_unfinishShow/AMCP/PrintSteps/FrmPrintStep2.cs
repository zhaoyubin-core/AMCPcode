using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace AMCP
{
    public partial class FrmPrintStep2 : Form
    {
        //FrmMain GV;
        //string strCSV = " ";
        //string str = " ";
        System.Windows.Forms.CheckBox[] arr_chkPrintPos;
        public FrmPrintStep2()
        {
            InitializeComponent();
            this.tabControl1.SelectedIndex = 0;
            arr_chkPrintPos = new System.Windows.Forms.CheckBox[] { chkPrintPos1, chkPrintPos2};
            txtGcodeFileName.Text = GV.PathFileName;
            //string str1 = Application.StartupPath + "\\Log\\" + "Conlog_" + Path.GetFileName(GV.PathFileName);
            //string str2 = Application.StartupPath + "\\Log\\" + "Monlog_" + Path.GetFileName(GV.PathFileName);
            //GV.ControlLogFileName = str1;
            //GV.MonitorLogFileName = str2;
            // 添加初始数据行
            // 清除所有现有行
            dataGridRotateSpeed.Rows.Clear();
            // 添加4行非空数据（行号1~4，转速默认60）
            for (int i = 0; i < 4; i++)
            {
                // 直接添加数据，避免空行
                dataGridRotateSpeed.Rows.Add(new object[] { i + 1, 60 });
            }

            //新增AB工位列表
            lvFilesA.Enter += ListView_Enter;
            lvFilesB.Enter += ListView_Enter;
            lvFilesA.Enter += ListView_Leave;
            lvFilesB.Enter += ListView_Leave;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //GV.MsgShow(lblNotice, "参数保存成功。", timer1, 5000);
        }

        private void btnGenerateCSV_Click(object sender, EventArgs e)
        {
            int cubeStructType = 1;
            string strCubeStruct = "ABAB";
            if (rdbCubeStruct1.Checked)
            {
                cubeStructType = 1;
                strCubeStruct = "ABAB";
            }
            else if (rdbCubeStruct2.Checked)
            {
                cubeStructType = 2;
                strCubeStruct = "AABB";
            }
            else if (rdbCubeStruct3.Checked)
            {
                cubeStructType = 3;
                strCubeStruct = "FCT";
            }
            try
            {
                int countLayer = Convert.ToInt32(nmbLayerCount.Value);
                double velo = Convert.ToDouble(nmbSetPrintingSpeed.Value);//打印速度
                double xLen = Convert.ToDouble(nmbWidthInX.Value);
                double yLen = Convert.ToDouble(nmbLengthInY.Value);
                double dZ = Convert.ToDouble(nmbLayerThickness.Value);
                double fs = Convert.ToDouble(nmbFilamentSpacing.Value);
                double liftHeight = Convert.ToDouble(nmbLiftHeight.Value);
                double[] rotSpeed = GetDataAsDoubleArray();//螺杆阀转速
                double timeTotal = getPrintingSeconds(velo, xLen, yLen, fs, countLayer);
                string strCSV = "";
                string str;
                int ipm_total = 0;
                double offsetRate = 0.5; // FCT结构偏移量（线间距占比）
                double offset = 0;
                for (int layer = 0; layer < countLayer; layer++)
                {
                    int Axis;
                    switch (cubeStructType)
                    {
                        case 1: // ABAB型
                            Axis = (int)(layer % 2);
                            offset = 0;
                            break;
                        case 2: // AABB型
                            Axis = (int)(((int)(layer / 2)) % 2);
                            offset = 0;
                            break;
                        case 3: // FCT型
                            Axis = (int)(layer % 2);
                            offset = (((int)(layer / 2)) % 2) * offsetRate;
                            break;
                        default:
                            Axis = (int)(layer % 2);
                            offset = 0;
                            break;
                    }
                    str = dZ.ToString() + ",";          // dZ
                    str += liftHeight.ToString() + ","; // liftHeight
                    str += Axis.ToString() + ",";        // Axis
                    str += "0,";                        // Min: start coordinate of Line segment 
                    str += (Axis == 0 ? xLen : yLen).ToString() + ",";        // Max: end coordinate of Line segment in line direction 
                    double pmax = (Axis == 0 ? yLen : xLen);
                    double ipm = Math.Ceiling(pmax / fs);  // max index of points
                    // 该层第一条线段坐标
                    double p = 0;
                    str += (p.ToString() + ",");
                    // 该层中间所有线段坐标
                    for (int ip = 1; ip <= ipm + 1; ip++)
                    {
                        p = (ip - offset) * fs;
                        if (p >= pmax) // 如果达到或超出边界，则停止扩张
                        {
                            p = pmax;
                            str += (p.ToString() + ",");
                            ipm = ip;
                            break;
                        }
                        str += (p.ToString() + ",");
                        ipm = ip;
                    }
                    // 该层最后一条线段坐标
                    if (ipm > ipm_total)
                    {
                        ipm_total = (int)ipm;
                    }
                    //新增，在每个坐标点结束后，添加转速数据
                    str += rotSpeed[layer].ToString() + ",";
                    strCSV += str + "\r\n";
                }
                //写抬头
                str = "layerThickness, liftHeight, Axis, Min, Max, "; //p1, p2, p3, ...\r\n";
                for (int i = 0; i <= ipm_total; i++)
                {
                    str += ("p" + (i + 1).ToString() + ", ");
                }
                str += "RotaeSpeed, ";
                str += "\r\n";
                strCSV = str + strCSV;
                saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog1.DefaultExt = "csv";
                saveFileDialog1.FileName = strCubeStruct + "-" + xLen.ToString() + "x" + yLen.ToString() + "x" + dZ.ToString() + "x" + fs.ToString()
                    + "x" + countLayer.ToString() + "-" + DateTime.Now.ToString("yyyyMMdd");
                DialogResult dlgRslt;
                dlgRslt = saveFileDialog1.ShowDialog();
                if (dlgRslt == DialogResult.OK)
                {
                    string fileName = saveFileDialog1.FileName;
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    if (GV.WriteTextFile(fileName, strCSV))
                    {
                        txtGcodeFileName.Text = fileName;
                        if (DialogResult.OK == MessageBox.Show("数据已保存到：" + fileName + "\n\n点击确定打开文件。点击取消关闭此对话。", "提示", MessageBoxButtons.OKCancel))
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(fileName);
                            }
                            catch (Exception ee)
                            {
                                MessageBox.Show("打开失败！");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        double[] RotateSpeed;
        //获取设置的转速
        public double[] GetDataAsDoubleArray()
        {
            // 创建与行数相同的数组
            double[] dataArray = new double[dataGridRotateSpeed.Rows.Count];

            for (int i = 0; i < dataGridRotateSpeed.Rows.Count; i++)
            {
                // 跳过新行（如果有）
                if (dataGridRotateSpeed.Rows[i].IsNewRow) continue;

                // 获取第二列的值
                object cellValue = dataGridRotateSpeed.Rows[i].Cells["转速"].Value;

                // 转换为double
                if (cellValue != null && double.TryParse(cellValue.ToString(), out double value))
                {
                    dataArray[i] = value;
                }
                else
                {
                    // 转换失败时使用默认值或抛出异常
                    dataArray[i] = 0.0; // 或者 throw new FormatException($"第{i+1}行数据格式错误");
                }
            }
            return dataArray;
        }

        /// <summary>
        /// 估算打印耗时
        /// </summary>
        /// <param name="velocity">打印速度</param>
        /// <param name="xLen">长度</param>
        /// <param name="yLen">宽度</param>
        /// <param name="delta">线间距</param>
        /// <param name="countLayer">层数</param>
        /// <returns></returns>
        private double getPrintingSeconds(double velocity, double xLen, double yLen, double delta, int countLayer)
        {
            double vx = velocity;
            double vy = velocity;
            double dx = delta;
            double dy = delta;
            int countX = (int)(xLen / dx) + 1;
            int countY = (int)(yLen / dy) + 1;
            double timeX = xLen / vx + 0.7;   // 完成一次加减速所需时间：0.7s
            double timeY = yLen / vy + 0.7;
            double timeLayer1 = timeX * countY + timeY + 0.7 * countY;
            double avgTimePerLine = timeLayer1 / countY;
            double timeLayer2 = timeY * countX + timeX;
            int countLayer2 = (int)countLayer / 2; // 偶数层层数
            int countlayer1 = countLayer - countLayer2; // 奇数层层数

            double timeTotal = timeLayer1 * countLayer2 + timeLayer2 * countlayer1;

            return timeTotal;
        }
        //估算打印耗时
        private void btnPrintingTime_Click(object sender, EventArgs e)
        {
            try
            {
                double countLayer = Convert.ToDouble(nmbLayerCount.Value);
                double velo = Convert.ToDouble(nmbSetPrintingSpeed.Value);
                double xLen = Convert.ToDouble(nmbWidthInX.Value);
                double yLen = Convert.ToDouble(nmbLengthInY.Value);
                int cLayer = Convert.ToInt32(nmbLayerCount.Value);
                double dx = Convert.ToDouble(nmbFilamentSpacing.Value);//线间距
                double timeTotal = getPrintingSeconds(velo, xLen, yLen, dx, cLayer);//总时间以秒计
                lblLeftTimeInfo.Text = "预计时间：" + ((int)(timeTotal / 3600)).ToString("00") + " hr, "
                    + ((int)((timeTotal % 3600) / 60)).ToString("00") + " min, "
                    + ((int)(timeTotal % 60)).ToString("00") + " sec";

            }
            catch (Exception ex)
            {
            }
        }

        private void btnBrowseGcodeFiles_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV files (*.csv); GCode files (*.gcode)|*.gcode;*.csv"; //"CSV files (*.csv)|*.csv|GCode files (*.gcode)|*.gcode";
            openFileDialog1.DefaultExt = "csv";
            openFileDialog1.FileName = "";
            DialogResult dlgRslt;
            dlgRslt = openFileDialog1.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                txtGcodeFileName.Text = openFileDialog1.FileName;//显示文件名
                // 根据文件扩展名调用不同的处理方法
                string extension = Path.GetExtension(openFileDialog1.FileName).ToLower();
                if (extension == ".csv")
                {
                    //读取转速列显示转速
                    //List<string> tempRot = new List<string>();
                    //tempRot = ReadLastColumnRotaeSpeed(openFileDialog1.FileName);
                    LoadRotateSpeedToDataGridView(openFileDialog1.FileName);
                    //读取层数，线间距等用于估算时间
                    LoadPrintData2EstimateTime(txtGcodeFileName.Text);
                }
            }
        }

        private void nmbSetPrintingSpeed_ValueChanged(object sender, EventArgs e)
        {
            GV.printingSpeedSet = (double)(nmbSetPrintingSpeed.Value);
            if (GV.frmPrintStep3 != null)
            {
                GV.frmPrintStep3.UpdatePrintingSpeed();
            }
        }

        /// <summary>
        /// 同步速度设置
        /// </summary>
        public void UpdatePrintingSpeed()
        {
            decimal newSpeed = (decimal)GV.printingSpeedSet;
            if (nmbSetPrintingSpeed.Value != newSpeed)
            {
                nmbSetPrintingSpeed.Value = newSpeed;
            }
        }

        public bool ValidityCheck()
        {
            if (File.Exists(txtGcodeFileName.Text))
            {
                lblFileError.Hide();
                return true;
            }
            else
            {
                GV.MsgShow(lblFileError, "路径文件加载失败，请检查确认！", timer1, 5000);
                return false;
            }
        }


        private void btnPathPreview_Click(object sender, EventArgs e)
        {
            GV.StopImmediately();
            string extName = System.IO.Path.GetExtension(GV.PathFileName);
            if (extName == ".csv")
            {            
                GV.ExecuteCustomedCube(GV.PathFileName, GV.PrintMode);
            }
            else if (extName == ".gcode")
            {
                GV.ExecuteGcode(GV.PathFileName, GV.PrintMode);
            }
            //新增逗号分割符路径显示
            else if (extName == ".path")
            {

            }

            GV.frmPathPreview.LoadPlannedPath();

            GV.frmPathPreview.EnableConfirmPrinting(false);
            GV.frmPathPreview.Show();
            GV.frmPathPreview.Activate();
        }

        private void txtGcodeFileName_TextChanged(object sender, EventArgs e)
        {
            if (ValidityCheck())//文件名变化
            {
                GV.PathFileName = txtGcodeFileName.Text;
                //string str1 = Application.StartupPath + "\\Log\\" + "Conlog_" + Path.GetFileName(GV.PathFileName);
                //string str2 = Application.StartupPath + "\\Log\\" + "Monlog_" + Path.GetFileName(GV.PathFileName);
                //GV.ControlLogFileName = str1;
                //GV.MonitorLogFileName = str2;
                //GV.StartEndPrinting(false);
                GV.MsgShow(lblFileError, "路径文件加载成功。", timer1, 3000, Color.FromArgb(0, 192, 0));
                btnPathPreview.Enabled = true;
            }
            else
            {
                GV.MsgShow(lblFileError, "路径文件加载失败，请检查确认。", timer1, 3000, Color.Red);
                btnPathPreview.Enabled = false;
            }
        }
        //直线段
        private void btnCreateLineGcode_Click(object sender, EventArgs e)
        {
            //string strGcode = "";
            //if (radioButton1.Checked) // 相对直线运动
            //{
            //    double len = 0;
            //    double angle = 0;
            //    double xt = 0;
            //    double yt = 0;
            //    double theta;
            //    len = (double)nmudLineLength.Value;
            //    angle = (double)nmudLineAngle.Value;
            //    theta = angle * Math.PI / 180;
            //    xt = len * Math.Cos(theta);
            //    yt = len * Math.Sin(theta);
            //    //strGcode += "G91" + "\n";
            //    strGcode += "G0 X0 Y0\r\n";
            //    strGcode += "G1 X" + xt.ToString() + " Y" + yt.ToString() + "\r\n";
            //    saveFileDialog1.FileName = "相对直线运动-" + len.ToString() + "mm-" + angle.ToString() + "deg";
            //}
            //else // 绝对直线运动
            //{
            //    double x0, y0, z0;
            //    double x1, y1, z1;
            //    x0 = (double)nmudX0.Value;
            //    y0 = (double)nmudY0.Value;
            //    z0 = (double)nmudZ0.Value;
            //    x1 = (double)nmudX1.Value;
            //    y1 = (double)nmudY1.Value;
            //    z1 = (double)nmudZ1.Value;

            //    //strGcode += "G90" + "\n";
            //    strGcode += "G0 X" + x0.ToString() + " Y" + y0.ToString() + " Z" + z0.ToString() + "\r\n";
            //    strGcode += "G1 X" + x1.ToString() + " Y" + y1.ToString() + " Z" + z1.ToString() + "\r\n";
            //    saveFileDialog1.FileName = "绝对直线运动(" + x0.ToString("0.###") + "," + y0.ToString("0.###") + "," + z0.ToString("0.###") + ")-("
            //        + x1.ToString("0.###") + "," + y1.ToString("0.###") + "," + z1.ToString("0.###") + ")";
            //}
            //saveFileDialog1.Filter = "gcode files (*.gcode)|*.gcode";
            //saveFileDialog1.DefaultExt = "gcode";
            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    if (GV.WriteTextFile(saveFileDialog1.FileName, strGcode))
            //    {
            //        GV.MsgShow(lblNotice, "文件保存成功。", timer1, 5000);
            //        txtGcodeFileName.Text = saveFileDialog1.FileName;
            //    }
            //}
        }

        private void label12_MouseMove(object sender, MouseEventArgs e)
        {
            (sender as Control).ForeColor = Color.Blue;
        }

        private void label12_MouseLeave(object sender, EventArgs e)
        {
            (sender as Control).ForeColor = Color.Black;
        }

        private void label12_Click(object sender, EventArgs e)
        {
            //nmudX0.Value = (decimal)GV.PrintingObj.Status.fPosX;
            //nmudY0.Value = (decimal)GV.PrintingObj.Status.fPosY;
            //nmudZ0.Value = (decimal)GV.PrintingObj.Status.fPosZ;
        }

        private void label22_Click(object sender, EventArgs e)
        {
            //nmudX1.Value = (decimal)GV.PrintingObj.Status.fPosX;
            //nmudY1.Value = (decimal)GV.PrintingObj.Status.fPosY;
            //nmudZ1.Value = (decimal)GV.PrintingObj.Status.fPosZ;
        }

        private void label22_MouseMove(object sender, MouseEventArgs e)
        {
            (sender as Control).ForeColor = Color.Blue;
        }

        private void label22_MouseLeave(object sender, EventArgs e)
        {
            (sender as Control).ForeColor = Color.Black;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //double xLen = (double)numericUpDown1.Value;
            //double yLen = (double)numericUpDown2.Value;
            //double dLen = (double)numericUpDown3.Value;

            //string strGcode = "";

            //for (double y = 0; y <= yLen; y += dLen)
            //{
            //    strGcode += "G0 X0 Y" + y.ToString() + "\r\n";
            //    strGcode += "G1 X" + xLen.ToString() + " Y" + y.ToString() + "\r\n";
            //}
            //saveFileDialog1.FileName = "折返式线-" + xLen.ToString() + "x" + yLen.ToString() + "-" + dLen.ToString();
            //saveFileDialog1.Filter = "gcode files (*.gcode)|*.gcode";
            //saveFileDialog1.DefaultExt = "gcode";
            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    if (GV.WriteTextFile(saveFileDialog1.FileName, strGcode))
            //    {
            //        GV.MsgShow(lblNotice, "文件保存成功。", timer1, 5000);
            //        txtGcodeFileName.Text = saveFileDialog1.FileName;
            //    }
            //}
        }

        /// <summary>
        /// 移动到目标位置（先提针、平移、再下针）
        /// </summary>
        /// <param name="x">目标X</param>
        /// <param name="y">目标Y</param>
        /// <param name="z">目标Z</param>
        /// <param name="NoticeInfo">达到目标位置的通知消息</param>
        /// <returns>估计所需运动时间</returns>
        private int Move2ReadyPos(double x, double y, double z, string NoticeInfo)
        {
            if (GV.CheckClearCommands() != ClearResult.Needless) return 0;

            double vZup = 30;       // Z轴提针速度(mm/s)
            double vZdown1 = 10;    // Z轴第一阶段下针速度(mm/s)
            double vZdown2 = 2;     // Z轴第二阶段下针速度(mm/s)
            double vXY = 30;        // XY轴移动速度(mm/s)
            double dNear = 3;       // 接近减速距离(mm)
            // 估计运动至起始点的时间：
            double timeEstimate = 0;

            GV.PrintingObj.qDisplayInfo("Notice", "Moving");

            // 分4步移动到目标位置：
            //// 第1步：快速将Z轴升至提针位置
            //GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, vZup, 0);
            //GV.PrintingObj.qWaitMoveEnd();
            //timeEstimate += (Math.Abs(GV.PrintingObj.Status.fPosZ - GV.Z_TOP) / vZup);  // 耗时估算

            // 第2步：将XY轴移动到目标位置
            GV.PrintingObj.qMoveXYTo(x, y, vXY, 0, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Sqrt(Math.Pow(GV.PrintingObj.Status.fPosX - x, 2) + Math.Pow(GV.PrintingObj.Status.fPosY - y, 2)) / vXY);  // 耗时估算
            // 第3步：较慢速将Z轴降至接近目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, z - dNear, vZdown1, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Abs(z - dNear) / vZdown1);  // 耗时估算
            // 第4步：非常慢速将Z轴降至目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, z, vZdown2, 0);
            GV.PrintingObj.qWaitMoveEnd();
            GV.PrintingObj.qDisplayInfo("Notice", NoticeInfo);
            timeEstimate += (5 / vZdown2);  // 耗时估算

            timeEstimate += 2;
            return (int)(timeEstimate * 1000);
        }

        //private void btnGenHex_Click(object sender, EventArgs e)
        //{
        //    double L = Convert.ToDouble(nmbLength.Value);
        //    double nn = Convert.ToInt16(nmbRow.Value);
        //    int mm = Convert.ToInt16(nmbColumn.Value);
        //    double hLayer = Convert.ToDouble(everylayerhegiht.Value);
        //    double YLength = 3 * L * (nn / 2) + 0.5 * L;    //(nn % 2==0?0.5:2) * L; // 偶数末尾加0.5L，奇数末尾加2L
        //    double offset = (double)(nmudOffset.Value);

        //    saveFileDialog1.FileName = "HEX-" + L.ToString() + "-" + nn.ToString() + "x" + mm.ToString() + "x" + hLayer.ToString()
        //        + "x" + nmudLayer.Value.ToString() + "-" + DateTime.Now.ToString("yyyyMMdd");
        //    saveFileDialog1.Filter = "GCode Files (*.gcode)|*.gcode";
        //    saveFileDialog1.DefaultExt = "gcode";
        //    DialogResult dlgRslt;
        //    dlgRslt = saveFileDialog1.ShowDialog();
        //    string bFile = "";
        //    if (dlgRslt == DialogResult.OK)
        //    {
        //        bFile = saveFileDialog1.FileName;
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    int layerCount = Convert.ToInt16(nmudLayer.Value);
        //    double taketiphigh = 2;
        //    double ddx = L / 2;
        //    double ddy = 0.886 * L;
        //    int step = 1;
        //    string strGcode = "";
        //    int ii;
        //    int ll;

        //    double x0 = 0, y0 = 0, z0 = 0;
        //    double x, y, z;
        //    double xLeft, xRight, yTop, yBottom;
        //    x = x0; y = y0; z = z0;
        //    if (chkSecondPath.Checked)
        //    {
        //        #region SecondPath
        //        strGcode += GetG0(step++, true, x, true, y, false, 0);
        //        strGcode += GetG0(step++, false, 0, false, 0, true, z);

        //        strGcode += ";Layer count: " + layerCount.ToString() + "\r\n";
        //        for (int iLayer = 0; iLayer < layerCount; iLayer++)  //层数
        //        {
        //            strGcode += ";LAYER:" + iLayer.ToString() + "\r\n";
        //            if (OffsetHEX.Checked)
        //            {
        //                // 奇偶层错位打印
        //                if (iLayer % 2 == 0)
        //                {
        //                    #region 奇数层
        //                    for (int i = 0; i < 2 * mm; i++)  // X 方向个数
        //                    {

        //                        for (int j = 0; j < nn; j++) // Y方向个数
        //                        {

        //                            if (j % 2 == 0)//第偶数个的画线方式（第一个作为编号0）
        //                            {
        //                                x += (i % 2 != 0 ? Math.Pow(-1, nn) : 1) * L * Math.Sqrt(3) / 2;
        //                                y += Math.Pow(-1, i) * L / 2;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);


        //                                //根据处于哪个纵行进行划线(初始位置的方向） Math.Pow(-1, nn) 根据nn的情况而定，返回来的第一条线有差别，
        //                                y += Math.Pow(-1, i) * L;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                x += -(i % 2 != 0 ? Math.Pow(-1, nn) : 1) * L * Math.Sqrt(3) / 2;
        //                                y += Math.Pow(-1, i) * L / 2;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                //X方向与上一条相反

        //                                if (i % 2 == 0 & j == nn - 1) //i%2==0 标示从上往下走
        //                                {
        //                                    x += L * Math.Sqrt(3);
        //                                    strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                }
        //                            }
        //                            else//第奇数个的画线方式
        //                            {
        //                                y += Math.Pow(-1, i) * L;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                if (j == nn - 1)
        //                                {
        //                                    x += L * Math.Sqrt(3) / 2;
        //                                    y += Math.Pow(-1, i) * L / 2;
        //                                    strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                }
        //                            }
        //                        }
        //                        x += offset;
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region 偶数层
        //                    for (int i = 0; i < (2 * mm + 1); i++)  // X 方向个数
        //                    {

        //                        if (i == 0)
        //                        {
        //                            x += -L * Math.Sqrt(3) / 2;
        //                            strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                        }
        //                        for (int j = 0; j < nn; j++) // Y方向个数
        //                        {
        //                            if (j % 2 == 0)//第偶数个的画线方式（第一行作为编号0）
        //                            {
        //                                x += (i % 2 != 0 ? Math.Pow(-1, nn) : 1) * L * Math.Sqrt(3) / 2;
        //                                y += Math.Pow(-1, i) * L / 2;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                //根据处于哪个纵行进行划线(初始位置的方向） Math.Pow(-1, nn) 根据nn的情况而定，返回来的第一条线有差别，
        //                                y += Math.Pow(-1, i) * L;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                x += -(i % 2 != 0 ? Math.Pow(-1, nn) : 1) * L * Math.Sqrt(3) / 2;
        //                                y += Math.Pow(-1, i) * L / 2;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                //X方向与上一条相反
        //                                if (i % 2 == 0 & j == nn - 1 & i != 2 * mm) //i%2==0 标示从上往下走
        //                                {
        //                                    x += L * Math.Sqrt(3);
        //                                    strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                }
        //                            }
        //                            else//第奇数行的画线方式
        //                            {
        //                                y += Math.Pow(-1, i) * L;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                if (j == nn - 1)
        //                                {
        //                                    x += L * Math.Sqrt(3) / 2;
        //                                    y += Math.Pow(-1, i) * L / 2;
        //                                    strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                                }
        //                            }
        //                        }
        //                        x += offset;
        //                    }
        //                    #endregion
        //                }
        //            }
        //            else
        //            {
        //                #region 奇偶层不错位
        //                for (int i = 0; i < 2 * mm; i++)  // X 方向个数
        //                {
        //                    for (int j = 0; j < nn; j++) // Y方向个数
        //                    {
        //                        if (j % 2 == 0)//第偶数个的画线方式（第一个作为编号0）
        //                        {
        //                            xLeft = Math.Pow(-1, nn) * L * Math.Sqrt(3) / 2;
        //                            xRight = Math.Pow(-1, nn) * L * Math.Sqrt(3) / 2;
        //                            x += ((i % 2 != 0 ? Math.Pow(-1, nn) : 1) * L * Math.Sqrt(3) / 2 - offset * 0.5);
        //                            y += Math.Pow(-1, i) * L / 2;
        //                            strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                            //根据处于哪个纵行进行划线(初始位置的方向） Math.Pow(-1, nn) 根据nn的情况而定，返回来的第一条线有差别
        //                            y += Math.Pow(-1, i) * L;
        //                            strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                            x += (-(i % 2 != 0 ? Math.Pow(-1, nn) : 1) * L * Math.Sqrt(3) / 2) + offset * 0.5;
        //                            y += Math.Pow(-1, i) * L / 2;
        //                            strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                            //X方向与上一条相反
        //                            if (i % 2 == 0 & j == nn - 1) //i%2==0 标示从上往下走
        //                            {
        //                                x += L * Math.Sqrt(3);
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                            }
        //                        }
        //                        else//第奇数个的画线方式
        //                        {
        //                            y += Math.Pow(-1, i) * L;
        //                            strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                            if (j == nn - 1)
        //                            {
        //                                x += L * Math.Sqrt(3) / 2;
        //                                y += Math.Pow(-1, i) * L / 2;
        //                                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                            }
        //                        }
        //                    }
        //                    x += offset;
        //                }
        //                #endregion
        //            }

        //            // 封边
        //            if (nmudWallCycle.Value > 0)
        //            {
        //                for (int iWall = 1; iWall <= nmudWallCycle.Value; iWall++)
        //                {
        //                    if (iWall == 1)
        //                    {
        //                        // Z轴升高（提升一些以避免针头刮到已打印部分）
        //                        z += taketiphigh;
        //                        strGcode += GetG0(step++, false, 0, false, 0, true, z);

        //                        // 回到下一层零位
        //                        x = x0 - offset * iWall;
        //                        y = y0 - offset * iWall;
        //                        strGcode += GetG0(step++, true, x, true, y, false, 0);

        //                        // Z轴降低
        //                        z += -taketiphigh;
        //                        strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    }
        //                    else
        //                    {
        //                        // 回到下一层零位
        //                        x = x0 - offset * iWall;
        //                        y = y0 - offset * iWall;
        //                        strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                    }

        //                    //str1 += MuCmd_01_INC_LINE_V1(step++, true, L * Math.Sqrt(3) * mm, false, L * Math.Sqrt(3) / 2, false, -2);
        //                    x += (L * Math.Sqrt(3)) * mm + offset * (2 * mm - 0.5 + 2 * iWall);
        //                    //y += L * Math.Sqrt(3) / 2;
        //                    strGcode += GetG1(step++, true, x, true, y, false, 0);

        //                    //str1 += MuCmd_01_INC_LINE_V1(step++, false, L * Math.Sqrt(3) * nn, true, L * (0.5 + 3 * nn), false, -2);
        //                    //x += L * Math.Sqrt(3) * nn;
        //                    y += (1.5 * nn + 0.5) * L + offset * iWall * 2;
        //                    strGcode += GetG1(step++, true, x, true, y, false, 0);

        //                    //str1 += MuCmd_01_INC_LINE_V1(step++, true, -L * Math.Sqrt(3) * mm, false, L * Math.Sqrt(3) / 2, false, -2);
        //                    x -= (L * Math.Sqrt(3)) * mm + offset * (2 * mm - 0.5 + 2 * iWall);
        //                    //y += L * Math.Sqrt(3) / 2;
        //                    strGcode += GetG1(step++, true, x, true, y, false, 0);

        //                    //str1 += MuCmd_01_INC_LINE_V1(step++, false, L * Math.Sqrt(3) * nn, true, -L * (0.5 + 3 * nn), false, -2);
        //                    //x += L * Math.Sqrt(3) * nn;
        //                    y -= (1.5 * nn + 0.5) * L + offset * iWall * 2;
        //                    strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                }
        //            }




        //            //x += L * Math.Sqrt(3) * mm;

        //            //strGcode += GetG1(step++, true, x, false, 0, false, 0);
        //            //y += YLength;

        //            //strGcode += GetG1(step++, false, 0, true, y, false, -2);

        //            //if (nn % 2 == 0)
        //            //{
        //            //    x += -L * Math.Sqrt(3) * mm;
        //            //    strGcode += GetG1(step++, true, x, false, 0, false, -2);

        //            //}
        //            //else
        //            //{
        //            //    //for (int i = 0; i < outPorts.Length; i++)
        //            //    //{
        //            //    //    str1 += MuCmd_12_Out(step++, outPorts[i], 0);
        //            //    //}
        //            //    z -= taketiphigh;
        //            //    strGcode += GetG0(step++, false, 0, false, 0, true, -taketiphigh);
        //            //    x += -L * Math.Sqrt(3) * mm;
        //            //    strGcode += GetG0(step++, true, x, false, 0, false, 0);
        //            //    z += taketiphigh;
        //            //    strGcode += GetG0(step++, false, 0, false, 0, true, taketiphigh);
        //            //    //for (int i = 0; i < outPorts.Length; i++)
        //            //    //{
        //            //    //    strGcode += MuCmd_12_Out(step++, outPorts[i], 1);
        //            //    //}
        //            //}
        //            //y += -YLength;
        //            //strGcode += GetG1(step++, false, 0, true, y, false, 0);


        //            ////for (int i = 0; i < outPorts.Length; i++)
        //            ////{
        //            ////    str1 += MuCmd_12_Out(step++, outPorts[i], 0);
        //            ////}

        //            if (iLayer != layerCount - 1)
        //            {
        //                // Z轴升高（提升一些以避免针头刮到已打印部分）
        //                z += taketiphigh;
        //                strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                // 回到原点
        //                x = x0;
        //                y = y0;
        //                strGcode += GetG0(step++, true, x, true, y, false, 0);
        //                // Z轴降低到下一层开始位置
        //                z += -taketiphigh + hLayer;
        //                strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //            }
        //            else
        //            {
        //                z += 20;
        //                strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                //str1 += MuCmd_11_Reset(step++);
        //            }
        //        }
        //        #endregion//方法1
        //    }
        //    else
        //    {
        //        #region Not SencondPath
        //        strGcode += ";Layer count: " + layerCount.ToString() + "\r\n";
        //        for (int iLayer = 0; iLayer < layerCount; iLayer++)  //层数
        //        {
        //            strGcode += ";LAYER:" + iLayer.ToString() + "\r\n";
        //            strGcode += GetG0(step++, true, x, true, y, false, 0);
        //            strGcode += GetG0(step++, false, 0, false, 0, true, z);

        //            for (ll = 1; ll <= (mm + 1); ll++)
        //            {
        //                if (ll % 2 == 0)
        //                {
        //                    for (ii = 1; ii <= nn; ii++)
        //                    {
        //                        x += ddx;
        //                        y += ddy;
        //                        strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                        x += -ddx;
        //                        y += ddy;
        //                        strGcode += GetG1(step++, true, x, true, y, false, 0);

        //                    }
        //                    if (ll < (mm + 1))
        //                    {
        //                        z += -taketiphigh;
        //                        strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                        x += L + 2 * ddx;
        //                        y += -2 * nn * ddy;
        //                        strGcode += GetG0(step++, true, x, true, y, false, 0);
        //                        z += taketiphigh;
        //                        strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    }
        //                }
        //                else
        //                {
        //                    for (ii = 1; ii <= nn; ii++)
        //                    {
        //                        x += -ddx;
        //                        y += ddy;
        //                        strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                        x += ddx;
        //                        y += ddy;
        //                        strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                    }
        //                    if (ll < (mm + 1))
        //                    {
        //                        z += -taketiphigh;
        //                        strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                        x += L;
        //                        y += -2 * nn * ddy;
        //                        strGcode += GetG0(step++, true, x, true, y, false, 0);
        //                        z += taketiphigh;
        //                        strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    }
        //                }
        //            }
        //            z += -taketiphigh;
        //            strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //            x = x0;
        //            y = y0;
        //            strGcode += GetG0(step++, true, x, true, y, false, 0);
        //            z += taketiphigh;
        //            strGcode += GetG0(step++, false, 0, false, 0, true, z);

        //            int jj = 1, zz = 1;
        //            for (jj = 1; jj <= nn; jj++)
        //            {
        //                zz = 1;
        //                for (ii = 1; ii <= mm; ii++)
        //                {
        //                    x += L;
        //                    strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                    z += -taketiphigh;
        //                    strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    x += ddx;
        //                    y += zz * ddy;
        //                    strGcode += GetG0(step++, true, x, true, y, false, 0);
        //                    z += taketiphigh;
        //                    strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    zz = -zz;
        //                }
        //                if (zz == -1)
        //                {
        //                    z += -taketiphigh;
        //                    strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    x += -mm * (L + ddx);
        //                    y += ddy;
        //                    strGcode += GetG0(step++, true, x, true, y, false, 0);
        //                    z += taketiphigh;
        //                    strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                }
        //                else if (zz == 1)
        //                {
        //                    z += -taketiphigh;
        //                    strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //                    x += -mm * (L + ddx);
        //                    y += 2 * ddy;
        //                    strGcode += GetG0(step++, true, x, true, y, false, 0);
        //                    z += taketiphigh;
        //                    strGcode += GetG0(step++, false, x0, false, y0, true, z);
        //                }
        //                else
        //                {

        //                }
        //            }

        //            if (Convert.ToBoolean(mm % 2)) { zz = mm / 2 + 1; } else { zz = mm / 2; }
        //            for (ii = 1; ii <= zz; ii++)
        //            {
        //                x += L;
        //                strGcode += GetG1(step++, true, x, true, y, false, 0);
        //                z += -taketiphigh;
        //                strGcode += GetG0(step++, false, x0, false, y0, true, z);
        //                x += 2 * ddx + L;
        //                //y += zz * ddy;
        //                strGcode += GetG1(step++, true, x, true, y, false, z0);
        //                z += taketiphigh;
        //                strGcode += GetG0(step++, false, x0, false, y0, true, z);
        //            }

        //            z += -taketiphigh - hLayer;
        //            strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //            x = x0;
        //            y = y0;
        //            strGcode += GetG0(step++, true, x, true, y, false, 0);
        //            z += taketiphigh;
        //            strGcode += GetG0(step++, false, 0, false, 0, true, z);
        //        }

        //        strGcode += "G0 Z0";
        //        #endregion //方法2
        //    }

        //    string fileName = bFile;
        //    //string strBegin = GV.ReadTextFile("begin.muf");
        //    //string strEnd = GV.ReadTextFile("end.muf");

        //    if (File.Exists(fileName))
        //    {
        //        File.Delete(fileName);
        //    }
        //    if (GV.WriteTextFile(fileName, strGcode))
        //    {
        //        txtGcodeFileName.Text = fileName;
        //        if (DialogResult.OK == MessageBox.Show("GCode文件已保存到：" + fileName + "\n\n点击确定打开文件。点击取消关闭此对话。", "提示", MessageBoxButtons.OKCancel))
        //        {
        //            try
        //            {
        //                System.Diagnostics.Process.Start(fileName);
        //            }
        //            catch (Exception ee)
        //            {
        //                MessageBox.Show("打开失败！");
        //            }
        //        }
        //    }
        //}


        private string GetG0(int step, bool flagX, double valueX, bool flagY, double valueY, bool flagZ, double valueZ)
        {
            string strGcode;
            strGcode = "G0";
            strGcode += flagX ? " X" + valueX.ToString("0.0000") : "";
            strGcode += flagY ? " Y" + valueY.ToString("0.0000") : "";
            strGcode += flagZ ? " Z" + valueZ.ToString("0.0000") : "";
            strGcode += "\r\n";
            return strGcode;
        }


        private string GetG1(int step, bool flagX, double valueX, bool flagY, double valueY, bool flagZ, double valueZ)
        {
            string strGcode;
            strGcode = "G1";
            strGcode += flagX ? " X" + valueX.ToString("0.0000") : "";
            strGcode += flagY ? " Y" + valueY.ToString("0.0000") : "";
            strGcode += flagZ ? " Z" + valueZ.ToString("0.0000") : "";
            strGcode += "\r\n";
            return strGcode;
        }

        private void rdbCubeStruct1_CheckedChanged(object sender, EventArgs e)
        {
            picStruct1.Show();
            picStruct2.Hide();
            picStruct3.Hide();
        }

        private void rdbCubeStruct2_CheckedChanged(object sender, EventArgs e)
        {
            picStruct1.Hide();
            picStruct2.Show();
            picStruct3.Hide();
        }

        private void rdbCubeStruct3_CheckedChanged(object sender, EventArgs e)
        {
            picStruct1.Hide();
            picStruct2.Hide();
            picStruct3.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //double A = (double)numericUpDown9.Value;
            //double T = (double)numericUpDown8.Value;
            //double L = (double)numericUpDown7.Value;
            //double w = T / (2 * Math.PI);
            //double xStep = 0.1;
            ////w = T/(2*pi); % 2*pi*w = T
            ////xx =  4*pi*w:0.01:20*pi*w;
            ////yy = A * sin(w*xx);


            //string strGcode = "";

            //strGcode += "G0 X0 Y0" + "\r\n";
            //for (double x = xStep; x <= L; x += xStep)
            //{
            //    strGcode += "G1 X" + x.ToString() + " Y" + (A * Math.Sin(w * x) + A / 2).ToString() + "\r\n";
            //}
            //saveFileDialog1.FileName = "波浪线-A" + A.ToString() + "T" + T.ToString() + "L" + L.ToString();
            //saveFileDialog1.Filter = "gcode files (*.gcode)|*.gcode";
            //saveFileDialog1.DefaultExt = "gcode";
            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    if (GV.WriteTextFile(saveFileDialog1.FileName, strGcode))
            //    {
            //        GV.MsgShow(lblNotice, "文件保存成功。", timer1, 5000);
            //        txtGcodeFileName.Text = saveFileDialog1.FileName;
            //    }
            //}
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GV.frmBasicTest.Show();
            GV.frmBasicTest.Activate();
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GV.PrintMode = checkBox1.Checked ? 1 : 0;
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                GV.PrintMode = 1;
            }
            else
            {
                GV.PrintMode = 0;
            }
            label66.Text = "PrintMode: " + GV.PrintMode.ToString();
        }
        //打印类型
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                GV.PrintMode = 2;//勾选螺杆打印
            }
            else
            {
                GV.PrintMode = 0;
            }
            label66.Text = "PrintMode: " + GV.PrintMode.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                try
                {
                    //if (GV.frmCamera.bInitialized)
                    //{
                    //    GV.frmCamera.CameraConn();
                    //    GV.photoEnabled = true;
                    //}
                    //else
                    //{
                    //    MessageBox.Show("摄像头模组初始化失败，请检查摄像头连接是否正常。", "错误提示");
                    //    checkBox3.Checked = false;
                    //    GV.photoEnabled = false;
                    //}
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            GV.noStopExtrude = checkBox4.Checked;
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            GV.lenAdvanced = (int)numericUpDown10.Value;
        }

        private void nmbLength_ValueChanged(object sender, EventArgs e)
        {
            //double L = Convert.ToDouble(nmbLength.Value);
            //double nn = Convert.ToInt16(nmbRow.Value);
            //int mm = Convert.ToInt16(nmbColumn.Value);
            //double hLayer = Convert.ToDouble(everylayerhegiht.Value);
            //double YLength = 3 * L * (nn / 2) + 0.5 * L;    //(nn % 2==0?0.5:2) * L; // 偶数末尾加0.5L，奇数末尾加2L
            //int layerCount = (int)(nmudLayer.Value);
            //double offset = (double)(nmudOffset.Value);
            //int wallCount = (int)nmudWallCycle.Value;
            //double xLen = (L * Math.Sqrt(3)) * mm + offset * (2 * mm - 0.5 + 2 * wallCount);
            //double yLen = (1.5 * nn + 0.5) * L + offset * wallCount * 2;
            //double zLen = hLayer * layerCount;
            //label72.Text = "长宽高（mm）：" + xLen.ToString("0.000") + " * " + yLen.ToString("0.000") + " * " + zLen.ToString("0.000");
        }
        //蜂鸣器关闭
        private void chkWarnOn_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWarnOn.Checked)
            {
                GV.Scilence = true;
                // GV.PrintingObj.Extrude(5, 0);
            }
            else
            {
                GV.Scilence = false;
            }
        }

        private void nmbLayerCount_ValueChanged(object sender, EventArgs e)
        {
            // 获取当前设置的层数
            int layerCount = (int)nmbLayerCount.Value;
            //// 防止负值
            //if (layerCount < 0)
            //{
            //    layerCount = 0;
            //    nmbLayerCount.Value = 0;
            //}
            // 暂停布局逻辑以提高性能
            dataGridRotateSpeed.SuspendLayout();

            try
            {
                // 清除所有现有行
                dataGridRotateSpeed.Rows.Clear();

                // 添加新行（带有序号）
                for (int i = 0; i < layerCount; i++)
                {
                    // 添加新行
                    int rowIndex = dataGridRotateSpeed.Rows.Add();

                    // 设置序号列（第一列）
                    dataGridRotateSpeed.Rows[rowIndex].Cells["序号"].Value = i + 1;

                    // 初始化数据列（第二列）
                    dataGridRotateSpeed.Rows[rowIndex].Cells["转速"].Value = 60;
                }
                dataGridRotateSpeed.AllowUserToAddRows = false;//禁止添加新行
            }
            finally
            {
                // 恢复布局逻辑
                dataGridRotateSpeed.ResumeLayout();
            }
        }
        //读取已有文件转速
        public static List<string> ReadLastColumnRotaeSpeed(string csvFilePath)
        {
            var rotateSpeedData = new List<string>();

            try
            {
                // 读取所有行
                var lines = File.ReadAllLines(csvFilePath);
                if (lines.Length == 0)
                    return rotateSpeedData;

                // 获取标题行
                var headers = lines[0].Split(',');
                if (headers.Length == 0)
                    return rotateSpeedData;

                // 获取最后一列索引和列名
                int lastColIndex = headers.Length - 2;
                string lastColName = headers[lastColIndex].Trim().ToLower();

                // 检查列名是否包含"rotate"或"speed"
                if (lastColName.Contains("rotate") || lastColName.Contains("speed"))
                {
                    // 处理数据行（跳过标题行）
                    for (int i = 1; i < lines.Length; i++)
                    {
                        var columns = lines[i].Split(',');
                        if (columns.Length > lastColIndex)
                        {
                            rotateSpeedData.Add(columns[lastColIndex]);
                        }
                    }
                    // GV.layerRotation = GV.frmPrintStep2.chkLayerRotation.Checked;
                    GV.setLayerRotation = true;
                }
                else GV.setLayerRotation = false;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"错误：文件 {csvFilePath} 未找到");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取CSV文件时出错: {ex.Message}");
            }
            return rotateSpeedData;
        }
        // 读取转速数据并显示到DataGridView
        private void LoadRotateSpeedToDataGridView(string filePath)
        {
            try
            {
                // 读取CSV文件获取转速数据
                List<string> tempRot = ReadLastColumnRotaeSpeed(filePath);

                // 确保DataGridView有足够的行
                if (dataGridRotateSpeed.Rows.Count < tempRot.Count)
                {
                    dataGridRotateSpeed.Rows.Add(tempRot.Count - dataGridRotateSpeed.Rows.Count);
                }
                // 查找列索引
                int rotateSpeedColumnIndex = -1;
                int serialNumberColumnIndex = -1;

                foreach (DataGridViewColumn column in dataGridRotateSpeed.Columns)
                {
                    // 查找转速列
                    if (column.HeaderText.Contains("转速") || column.Name.Contains("RotateSpeed"))
                    {
                        rotateSpeedColumnIndex = column.Index;
                    }
                    // 查找序号列
                    else if (column.HeaderText.Contains("序号") || column.Name.Contains("SerialNumber"))
                    {
                        serialNumberColumnIndex = column.Index;
                    }
                    // 如果两个列都找到了，可以提前退出循环
                    if (rotateSpeedColumnIndex != -1 && serialNumberColumnIndex != -1)
                        break;
                }
                // 填充数据到DataGridView
                for (int i = 0; i < tempRot.Count; i++)
                {
                    // 添加序号（从1开始）
                    if (serialNumberColumnIndex != -1)
                    {
                        dataGridRotateSpeed.Rows[i].Cells[serialNumberColumnIndex].Value = i + 1;
                    }
                    // 添加转速数据
                    if (rotateSpeedColumnIndex != -1)
                    {
                        if (double.TryParse(tempRot[i], out double rotateSpeed))
                        {
                            dataGridRotateSpeed.Rows[i].Cells[rotateSpeedColumnIndex].Value = rotateSpeed;
                        }
                        else
                        {
                            dataGridRotateSpeed.Rows[i].Cells[rotateSpeedColumnIndex].Value = "无效数据";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载转速数据失败: {ex.Message}", "错误",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadPrintData2EstimateTime(string filepath)
        {
            double timeTotal = 0;
            try
            {
                //逐层估算时间，并计算总时间
                string[][] matrixP = CsvFileReader.Read(filepath);
                int cLayer = matrixP.Length;//层数
                for (int layer = 0; layer < cLayer; layer++)
                {
                    double velo = GV.printingSpeedSet;//打印速度
                    int count = matrixP[layer].Length;//一共有多少列
                    double lengthX = Convert.ToDouble(matrixP[layer][4]) - Convert.ToDouble(matrixP[layer][3]);//单根线条长度
                    int maxP = GV.setLayerRotation ? 2 : 1;//是否含有rotaespeed
                    double lengthY = Convert.ToDouble(matrixP[layer][count - maxP]) - Convert.ToDouble(matrixP[layer][5]);//延某方向扩展距离                                                                                                           
                    double dz = Convert.ToDouble(matrixP[layer][0]);//抬针高度
                    double liftHeight = Convert.ToDouble(matrixP[layer][1]);
                    double dx = Convert.ToDouble(matrixP[layer][7]) - Convert.ToDouble(matrixP[layer][6]);//线间距
                    int countLines = (int)(lengthY / dx) + 1;//记录线条数
                    double timeSingle = lengthY / velo + 0.7;
                    double timeLayer = timeSingle * countLines + timeSingle + GV.timeCloseExtrudeInAdvance / 1000 + (int)(dz / GV.printingSpeedSet);
                    timeTotal += timeLayer;
                }
                lblLeftTimeInfo.Text = "预计时间：" + ((int)(timeTotal / 3600)).ToString("00") + " hr, "
                    + ((int)((timeTotal % 3600) / 60)).ToString("00") + " min, "
                    + ((int)(timeTotal % 60)).ToString("00") + " sec";
            }
            catch (Exception ex)
            {

            }
        }
        private void btnSetRotSpeed_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 获取转速数据和目标文件名
                double[] tempRot = GetDataAsDoubleArray();
                string fileName = txtGcodeFileName.Text;
                // 2. 验证文件是否存在
                if (!File.Exists(fileName))
                {
                    MessageBox.Show("指定的文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // 3. 读取文件所有行
                var lines = File.ReadAllLines(fileName).ToList();

                // 处理标题行
                var headers = lines[0].Split(',');
                int rotateSpeedColIndex = headers.Length - 2; // 最后一列
                // 查找Rotate Speed列（不区分大小写）
                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i].Trim().Equals("Rotate Speed", StringComparison.OrdinalIgnoreCase))
                    {
                        rotateSpeedColIndex = i;
                        break;
                    }
                }
                // 6.确保数据行数与转速数据匹配
                //if (lines.Count - 1 < tempRot.Length)
                //{
                //    MessageBox.Show("转速数据行数超过文件数据行数！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //}

                // 7. 更新转速数据
                for (int i = 0; i < tempRot.Length; i++)
                {
                    if (i + 1 >= lines.Count) break; // 防止超出文件行数

                    var columns = lines[i + 1].Split(',').ToList(); // +1跳过标题行

                    // 确保列数足够
                    while (columns.Count <= rotateSpeedColIndex)
                    {
                        columns.Add(""); // 补充空列
                    }

                    // 更新转速值
                    columns[rotateSpeedColIndex] = tempRot[i].ToString(CultureInfo.InvariantCulture);

                    // 重新组合行
                    lines[i + 1] = string.Join(",", columns);
                }
                // 8. 保存为新文件
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV文件 (*.csv)|*.csv|文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveFileDialog.Title = "保存转速数据文件";
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(fileName) + "_with_rotatespeed" + Path.GetExtension(fileName);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
                saveFileDialog.OverwritePrompt = true; // 覆盖前提示
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string newFileName = saveFileDialog.FileName;

                    try
                    {
                        // 保存到新文件
                        File.WriteAllLines(newFileName, lines);

                        MessageBox.Show($"转速数据已成功保存到新文件：\n{newFileName}",
                                       "保存成功",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存文件时出错：{ex.Message}",
                                       "错误",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存转速数据时出错：{ex.Message}", "错误",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void chkLayerRotation_CheckedChanged(object sender, EventArgs e)
        {
            bool isLayerRotationChecked = chkLayerRotation.Checked;
            GV.layerRotation = isLayerRotationChecked;
            //GV.timeRotation = !chkLayerRotation.Checked;

            if (isLayerRotationChecked)
            {
                chkTimeRotation.Checked = false;
                GV.frmRotaryValveCtrl.Hide();
                panelLayerRotation.Enabled = true;
                panelLayerRotation.Visible = true;
            }
            else
            {
                //chkTimeRotation.Checked = false;
                //GV.frmRotaryValveCtrl.Show();
                panelLayerRotation.Enabled = false;
                panelLayerRotation.Visible = false;
            }
        }

        private void chkTimeRotation_CheckedChanged(object sender, EventArgs e)
        {
            bool isTimeRotationChecked = chkTimeRotation.Checked;
            GV.timeRotation = isTimeRotationChecked;

            //GV.timeRotation = !chkLayerRotation.Checked;    
            if (isTimeRotationChecked)
            {
                panelLayerRotation.Visible = false;
                chkLayerRotation.Checked = false;
                GV.frmRotaryValveCtrl.Show();
                GV.frmRotaryValveCtrl.Activate();
            }
            else
            {
                GV.frmRotaryValveCtrl.Hide();
            }
        }
        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        // 存储已选择的A工位文件路径
        private List<string> listAItems = new List<string>();
        // 存储已选择的B工位文件路径
        private List<string> listBItems = new List<string>();

        // 记录最后获得焦点的ListView
        private ListView lastFocusedListView;
        private bool select2Lists = false;//双工位时为同时选择，耦合运动
        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            //检查
            if (lastFocusedListView == null)
            {
                MessageBox.Show("请先选择一个工位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int maxCount = 5;//最多一次性打印5个样品
            int canAddCount;
            int addCount;
            openFileDialog2.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog2.Multiselect = true;
            openFileDialog2.Title = "选择CSV文件";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                //同时选择A和B工位
                if (select2Lists)
                {
                    // 获取选中的文件列表
                    string[] selectedDoubleFiles = openFileDialog2.FileNames;
                    // 同时添加到A和B
                    // 计算可以添加的文件数量（考虑两个列表的当前状态）
                    int availableSlotsA = maxCount - listAItems.Count;
                    int availableSlotsB = maxCount - listBItems.Count;
                    canAddCount = Math.Min(selectedDoubleFiles.Length, Math.Min(availableSlotsA, availableSlotsB));

                    if (canAddCount <= 0)
                    {
                        MessageBox.Show("列表已满，无法添加更多文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // 确保两个列表长度相同，如果不同则用空字符串补齐
                    while (listAItems.Count < listBItems.Count)
                    {
                        listAItems.Add(""); // 添加空项而不是null，便于区分
                    }
                    while (listBItems.Count < listAItems.Count)
                    {
                        listBItems.Add(""); // 添加空项而不是null，便于区分
                    }
                    //同步添加
                    for (int index = 0; index < selectedDoubleFiles.Length; index++)
                    {
                        listAItems.Add(selectedDoubleFiles[index]);
                        listBItems.Add(selectedDoubleFiles[index]);
                        //GV.listPrintPosA[index] = true;
                        //GV.listPrintPosB[index] = true;
                        //AddListItem();
                    }
                    UpdateListViews(lvFilesA, listAItems, lvFilesB, listBItems);
                    //UpdateListViews(lvFilesB, listBItems, lvFilesB, listAItems);
                }
                else//单独选择A或B工位
                {
                    List<string> targetList = lastFocusedListView == lvFilesA ? listAItems : listBItems;//确认目标工位
                    List<string> otherList = lastFocusedListView == lvFilesA ? listBItems : listAItems;
                    string[] selectedSingleFiles = openFileDialog2.FileNames;

                    addCount = Math.Min(maxCount - targetList.Count, selectedSingleFiles.Length);
                    if (addCount <= 0)
                    {
                        MessageBox.Show("该列表最多只能添加5个文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    for (int index = 0; index < addCount; index++)
                    {
                        targetList.Add(selectedSingleFiles[index]);
                        // 在另一个列表的对应位置添加空项
                        if (otherList.Count < targetList.Count)
                        {
                            otherList.Add(""); // 添加空项保持对齐
                        }

                        //if (lastFocusedListView == lvFilesA)
                        //    //GV.listPrintPosA[index] = true;
                        //    AddListItem(true, false);
                        //else
                        //    AddListItem(false, true);
                        //GV.listPrintPosB[index] = true;
                    }
                    //if (lastFocusedListView == lvFilesA)
                    //    UpdateListA();
                    //else
                    //    UpdateListB();

                    // 确保两个列表完全对齐
                    while (targetList.Count > otherList.Count)
                    {
                        otherList.Add("");
                    }
                    UpdateListViews(lvFilesA, listAItems, lvFilesB, listBItems);
                }
                //更新文件计数
                UpdateListItems(listAItems, listBItems);
            }
        }


        private void btnRemoveSelectedA_Click(object sender, EventArgs e)
        {
            if (lastFocusedListView == null || lastFocusedListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择一个工位并选中要移除的文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<string> targetList = lastFocusedListView == lvFilesA ? listAItems : listBItems;

            // 从后往前删除，避免索引变化问题
            for (int i = lastFocusedListView.SelectedItems.Count - 1; i >= 0; i--)
            {
                ListViewItem item = lastFocusedListView.SelectedItems[i];
                targetList.Remove(item.Tag as string);
            }

            //if (lastFocusedListView == lvFilesA)
            //    UpdateListA();
            //else
            //    UpdateListB();
            UpdateListViews(lvFilesA, listAItems, lvFilesB, listBItems);
        }

        //更新AB工位文件选择状态
        private void ListView_Enter(object sender, EventArgs e)
        {
            // 记录最后获得焦点的ListView
            lastFocusedListView = sender as ListView;
            //按钮使能
            btnRemoveSelected.Enabled = true;
            btnSelectFiles.Enabled = true;
            // 根据最后获得焦点的ListView更新按钮状态
            if (lastFocusedListView == lvFilesA)
            {
                btnSelectFiles.Text = "添加到列表A";
                btnRemoveSelected.Text = "从列表A移除";
                lvFilesA.Font = new Font(lvFilesA.Font, FontStyle.Bold);
                lvFilesB.Font = new Font(lvFilesB.Font, FontStyle.Regular);
            }
            else if (lastFocusedListView == lvFilesB)
            {
                btnSelectFiles.Text = "添加到列表B";
                btnRemoveSelected.Text = "从列表B移除";
                lvFilesB.Font = new Font(lvFilesB.Font, FontStyle.Bold);
                lvFilesA.Font = new Font(lvFilesA.Font, FontStyle.Regular);
            }
            this.Invalidate(); // 触发重绘边框
        }

        // 定义焦点状态的颜色
        private readonly Color FocusedBackColor = Color.LightGray;  // 浅灰色背景
        private readonly Color FocusedBorderColor = Color.DodgerBlue;             // 蓝色边框
        private readonly Color NormalBackColor = SystemColors.Window;              // 默认背景色
        private readonly Color NormalBorderColor = SystemColors.ControlDark;        // 默认边框色
        private void ListView_Leave(object sender, EventArgs e)
        {
            // 更新背景色
            lvFilesA.BackColor = lvFilesA.Focused ? FocusedBackColor : NormalBackColor;
            lvFilesB.BackColor = lvFilesB.Focused ? FocusedBackColor : NormalBackColor;
            this.Invalidate(); // 触发重绘边框
        }
        private void UpdateListA()
        {
            lvFilesA.Items.Clear();
            int maxCount = Math.Max(listAItems.Count, listBItems.Count);

            for (int i = 0; i < maxCount; i++)
            {
                if (i < listAItems.Count)
                {
                    string filePath = listAItems[i];
                    AddFileToListView(lvFilesA, filePath, i + 1);
                }
                else
                {
                    // 添加空行
                    ListViewItem emptyItem = new ListViewItem((i + 1).ToString());
                    emptyItem.SubItems.Add("--");
                    emptyItem.SubItems.Add("--");
                    emptyItem.Tag = null;
                    lvFilesA.Items.Add(emptyItem);
                }
            }

            lblFileCount.Text = $"已选择文件数 A:{listAItems.Count} 个  B:{listBItems.Count} 个";
        }

        private void UpdateListB()
        {
            lvFilesB.Items.Clear();
            int maxCount = Math.Max(listAItems.Count, listBItems.Count);

            for (int i = 0; i < maxCount; i++)
            {
                if (i < listBItems.Count)
                {
                    string filePath = listBItems[i];
                    AddFileToListView(lvFilesB, filePath, i + 1);
                }
                else
                {
                    // 添加空行
                    ListViewItem emptyItem = new ListViewItem((i + 1).ToString());
                    emptyItem.SubItems.Add("--");
                    emptyItem.SubItems.Add("--");
                    emptyItem.Tag = null;
                    lvFilesB.Items.Add(emptyItem);
                }
            }

            lblFileCount.Text = $"已选择文件数 A:{listAItems.Count} 个  B:{listBItems.Count} 个";
        }
        private void UpdateListViews(ListView listViewA, List<string> itemsA, ListView listViewB, List<string> itemsB)
        {
            // 清空两个ListView
            listViewA.Items.Clear();
            listViewB.Items.Clear();

            // 获取最大行数
            int maxCount = Math.Max(itemsA.Count, itemsB.Count);

            for (int i = 0; i < maxCount; i++)
            {
                // 处理ListViewA
                if (i < itemsA.Count && !string.IsNullOrEmpty(itemsA[i]))
                {
                    string filePath = itemsA[i];
                    AddFileToListView(listViewA, filePath, i + 1);
                }
                else
                {
                    // 为ListViewA添加空行
                    ListViewItem emptyItemA = new ListViewItem((i + 1).ToString());
                    emptyItemA.SubItems.Add("--");
                    emptyItemA.SubItems.Add("--");
                    emptyItemA.Tag = null;
                    listViewA.Items.Add(emptyItemA);
                }

                // 处理ListViewB
                if (i < itemsB.Count && !string.IsNullOrEmpty(itemsB[i]))
                {
                    string filePath = itemsB[i];
                    AddFileToListView(listViewB, filePath, i + 1);
                }
                else
                {
                    // 为ListViewB添加空行
                    ListViewItem emptyItemB = new ListViewItem((i + 1).ToString());
                    emptyItemB.SubItems.Add("--");
                    emptyItemB.SubItems.Add("--");
                    emptyItemB.Tag = null;
                    listViewB.Items.Add(emptyItemB);
                }
            }

            // 更新文件计数显示
            lblFileCount.Text = $"已选择文件数 A:{itemsA.Count} 个  B:{itemsB.Count} 个";
        }


        private void AddFileToListView(ListView listView, string filePath, int index)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            ListViewItem item = new ListViewItem(index.ToString());
            item.SubItems.Add(fileInfo.Name);
            item.SubItems.Add(fileInfo.DirectoryName);
            //item.SubItems.Add((fileInfo.Length / 1024).ToString("N0") + " KB");
            item.Tag = filePath;

            listView.Items.Add(item);
        }

        //添加列表项
        void AddListItem()
        {

        }
        private void chkSelectBoth_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectBoth.Checked)
            {
                GV.arrPrintPosSelected[0] = true;
                GV.arrPrintPosSelected[1] = true;
                select2Lists = true;
            }
            else
            {
                select2Lists = false;
                GV.arrPrintPosSelected[0] = false;
                GV.arrPrintPosSelected[1] = false;
            }
        }

        private void btnMultiPathPreview_Click(object sender, EventArgs e)
        {
            //检查
            //UpdateListItems(listAItems, listBItems);
            GV.ExecuteCustomedMultiCubes(listAItems, listBItems, GV.PrintMode);

            //打开预览界面并展示
            GV.frmPathPreview.LoadPlannedPath();

            GV.frmPathPreview.EnableConfirmPrinting(false);
            GV.frmPathPreview.Show();
            GV.frmPathPreview.Activate();
        }
        public void SetCheckState(System.Windows.Forms.CheckBox chk)
        {
            switch (chk.Name)
            {
                case "chkPrintPos1":
                    GV.arrPrintPosSelected[0] = chk.Checked;
                    //txtPosHeight1.Enabled = chk.Checked;
                    //txtRotateSpeed1.Enabled = chk.Checked;
                    chkPrintPos1.Checked = chk.Checked;
                    //txtPosHeight.Text = txtPosHeight1.Text;
                    //txtRotateSpeed.Text = txtRotateSpeed1.Text;
                    break;
                case "chkPrintPos2":
                    GV.arrPrintPosSelected[1] = chk.Checked;
                    //txtPosHeight2.Enabled = chk.Checked;
                    //txtRotateSpeed2.Enabled = chk.Checked;
                    chkPrintPos2.Checked = chk.Checked;
                    //txtPosHeight.Text = txtPosHeight2.Text;
                    //txtRotateSpeed.Text = txtRotateSpeed2.Text;
                    break;
                //case "chkPrintPos3":
                //    GV.arrPrintPosSelected[2] = chk.Checked;
                //    txtPosHeight3.Enabled = chk.Checked;
                //    txtRotateSpeed3.Enabled = chk.Checked;
                //    chkPrintPos3.Checked = chk.Checked;
                //    //txtPosHeight.Text = txtPosHeight3.Text;
                //    //txtRotateSpeed.Text = txtRotateSpeed3.Text;
                //    break;
                //case "chkPrintPos4":
                //    GV.arrPrintPosSelected[3] = chk.Checked;
                //    txtPosHeight4.Enabled = chk.Checked;
                //    txtRotateSpeed4.Enabled = chk.Checked;
                //    chkPrintPos4.Checked = chk.Checked;
                //    //txtPosHeight.Text = txtPosHeight4.Text;
                //    //txtRotateSpeed.Text = txtRotateSpeed4.Text;
                //    break;
                //case "chkPrintPos5":
                //    GV.arrPrintPosSelected[4] = chk.Checked;
                //    txtPosHeight5.Enabled = chk.Checked;
                //    txtRotateSpeed5.Enabled = chk.Checked;
                //    chkPrintPos5.Checked = chk.Checked;
                //    //txtPosHeight.Text = txtPosHeight5.Text;
                //    //txtRotateSpeed.Text = txtRotateSpeed5.Text;
                //    break;
                //case "chkPrintPos6":
                //    GV.arrPrintPosSelected[5] = chk.Checked;
                //    txtPosHeight6.Enabled = chk.Checked;
                //    txtRotateSpeed6.Enabled = chk.Checked;
                //    chkPrintPos6.Checked = chk.Checked;
                //    //txtPosHeight.Text = txtPosHeight6.Text;
                //    //txtRotateSpeed.Text = txtRotateSpeed6.Text;
                //    break;
            }

        }
        private void UpdateListItems(List<string> ItemsA = null, List<string> ItemsB = null)
        {
            // 确保目标列表有足够的容量           
            while (GV.listPathFileNameA.Count < ItemsA.Count)
            {
                GV.listPathFileNameA.Add(""); // 或者使用默认值
                GV.listAirPressureA.Add(GV.extrudePressValueA);
                GV.listRotateSpeedA.Add(GV.speedRotaryValueA);
                GV.listTargetZ.Add(GV.Z_BOTTOM);
                GV.listPrintPosA.Add(true);
            }

            for (int i = 0; i < ItemsA.Count; i++)
            {
                GV.listPathFileNameA[i] = ItemsA[i];
            }
            //B工位赋值
            while (GV.listPathFileNameB.Count < ItemsB.Count)
            {

                GV.listPathFileNameB.Add(""); // 或者使用默认值
                GV.listAirPressureB.Add(GV.extrudePressValueB);
                GV.listRotateSpeedB.Add(GV.speedRotaryValueB);
               // GV.listTargetZ.Add(GV.Z_BOTTOM);
                GV.listPrintPosB.Add(true);
            }
            for (int j = 0; j < ItemsB.Count; j++)
            {
                GV.listPathFileNameB[j] = ItemsB[j];
            }
        }


        private void chkTransPort_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTransPort.Checked)
            {
                GV.basalTransport = true;
            }
            else
            {
                GV.basalTransport = false;
            }
        }

        private void chkNozzleClean_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNozzleClean.Checked)
            {
                GV.nozzleClean = true;
            }
            else
            {
                GV.nozzleClean = false;
            }
        }
        //更新list状态
        private void lvFiles_Leave(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            string str = ctrl.Name;
            int a = lvFilesA.Items.Count;
            int b = lvFilesB.Items.Count;
           GV.ContiniousPrintCount =Math.Max(a, b);
            if (GV.ContiniousPrintCount > 0)
            {
                // 初始化列表（如果为空）
            
                GV.listTargetZ.Clear();
                GV.listPrintSpeed.Clear();

                GV.listAirPressureA.Clear();
                GV.listAirPressureB.Clear();
                GV.listRotateSpeedA.Clear();
                GV.listRotateSpeedB.Clear();
                GV.listSlightZa.Clear();
                GV.listSlightZb.Clear();

                double targetZ = GV.Z_BOTTOM;
                double speed = GV.vSet;
                double extrudePressure = GV.extrudePressValueA;
                double rotateValve = GV.speedRotaryValueA;
                double adjustZ = GV.Za_BOTTOM;
                
                for (int count = 0; count < Math.Max(a, b); count++)
                {
                    //先设置默认值,共用主z轴，下针位置，打印速度；分别设置气压，螺杆，小z轴
                   
                    GV.listTargetZ.Add(targetZ);
                    GV.listPrintSpeed.Add(speed);
                }
                switch (str)
                {
                    case "lvFilesA":

                        GV.frmPrintStep4.SetCheckState(0, a > 0);
                        for (int i = 0; i < a; i++)
                        {
                            GV.listAirPressureA.Add(extrudePressure);
                            GV.listRotateSpeedA.Add(rotateValve);
                            GV.listSlightZa.Add(adjustZ);
                        }
                        break;
                    case "lvFilesB":
                        GV.frmPrintStep4.SetCheckState(1, b > 0);
                        for (int j = 0; j < b; j++)
                        {
                            GV.listAirPressureB.Add(extrudePressure);
                            GV.listRotateSpeedB.Add(rotateValve);
                            GV.listSlightZb.Add(adjustZ);
                        }
                        break ;
                }              
            }
        }
    }
}
