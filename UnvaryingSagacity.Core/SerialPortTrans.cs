using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
 
namespace UnvaryingSagacity.Core
{
    ///// <summary>
    ///// 下位机接收到命令后数据后的回传数据类型
    ///// </summary>
    //public enum DataBlockTypeOfLoop_Display
    //{
    //    完成,
    //    回显,
    //    失败,
    //    重传数据,
    //    等待数据,
    //    待命中,
    //    正在执行命令,
    //}

    /// <summary>
    /// 上位机状态
    /// </summary>
    public enum ProcessStatus
    {
        未知,
        打开,
        监听,
        等待命令,
        命令发出,
        发送附加数据,
        等待执行结果,
        执行完成,
    }
    /// <summary>
    /// 接收到下位机的数据块后的回调
    /// </summary>
    /// <param name="data">数据块</param>
    /// <param name="length">数据块长度</param>
    /// <param name="hasdata">true还有数据块</param>
    public delegate void CallbackDataLoop_Display(object  sender, byte[] data, int length, ref ProcessStatus status);

    /// <summary>
    /// 接收到下位机的多数据块后的回调
    /// </summary>
    /// <param name="data">数据块</param>
    /// <param name="length">数据块长度</param>
    /// <param name="hasdata">true还有数据块</param>
    public delegate void CallbackDataLoop_Display_Multi(object sender, byte[] data, int length, bool hasBlock, ref ProcessStatus status);

    /// <summary>
    /// 状态改变回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="status"></param>
    public delegate void CallbackStatusChanged(object sender, ProcessStatus status);

    /// <summary>
    /// 通过System.IO.Ports.SerialPort.GetPortNames()获得所有可用的串口;
    /// </summary>
    public class SerialPortTrans:SampleClass ,IDisposable  
    {
        SerialPort _serialPort = new SerialPort();
        Thread _readThread;
        Thread _timerThread;
        ProcessStatus _status = ProcessStatus.未知;
        byte[] _reveicebuf = new byte[1024]; //预留1K接收缓存
        byte[] _debugbuf = new byte[0x10000];//调试,接收所有执行结果数据 
        int _reveice_index = 0;
        int _debug_index = 0;
        long _timeTick = 0;
        int _sec = 0;//定时器秒数

        public ProcessStatus Status
        {
            get { return _status; }
        }

        public CallbackDataLoop_Display DataLoop_DisplayCallback
        {
            get;
            set;
        }

        public CallbackDataLoop_Display_Multi DataLoop_Multi_DisplayCallback
        {
            get;
            set;
        }
        public CallbackStatusChanged StatusChangedCallback
        {
            get;
            set;
        }

        /// <summary>
        /// 检查串口是否有效, 检查需要关闭所有的串口
        /// </summary>
        /// <param name="portname"></param>
        /// <returns></returns>
        public static bool IsSerialPortVailde(string portname)
        {
            SerialPort serialPort = new SerialPort();
            bool b = false;

            try
            {
                serialPort.PortName = portname;
                serialPort.BaudRate = 115200;
                serialPort.Open();
                b = serialPort.IsOpen;
                serialPort.Close();
                return b;
            }
            catch
            {
                return b;
            }
        }

        public SerialPortTrans()
        {
            _timeTick = DateTime.Now.Ticks;
            _readThread = new Thread(Read);
            _readThread.Start();
            _timerThread = new Thread(Timetick);
            _timerThread.Start();
        }
        
        /// <summary>
        /// 固定115200的波特率
        /// </summary>
        /// <param name="portname">大写的串口名,如COM3</param>
        public void Open(string portname)
        {
            _reveice_index = 0;
            _debug_index = 0;
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            General.Delay_MS(100);
            _serialPort.PortName = portname;
            _serialPort.BaudRate = 115200;
            try
            {
                _serialPort.Open();
                if (_serialPort.IsOpen)
                {
                    _status = ProcessStatus.打开;
                }
                else
                {
                    _status = ProcessStatus.未知;
                }
            }
            catch (Exception ex)
            {
                _status = ProcessStatus.未知;
                System.Windows.Forms.MessageBox.Show(ex.Message); 
            }
        }

        public void Close()
        {
            _reveice_index = 0;
            _status = ProcessStatus.未知;
            _serialPort.Close();
        }

        /// <summary>
        /// 开始监听,每delay发出'>'
        /// 获得下位机应答'<'
        /// </summary>
        /// <param name="delay"></param>
        public void StartListen(int delay)
        {
            if (_status != ProcessStatus.未知 && _serialPort.IsOpen)
            {
                _status = ProcessStatus.监听;
                _serialPort.DiscardOutBuffer();
                _serialPort.DiscardInBuffer();
                _reveice_index = 0;
                _reveicebuf = new byte[1024];
                _timeTick = DateTime.Now.Ticks;
                _serialPort.Write(">>>>>>>>>>>>>>>>>");
                
                _sec = delay;
                _timerThread.Join (100);
                _readThread.Join(100);
            }
        }

        public void StopListen()
        {
            try
            {
                _serialPort.DiscardOutBuffer();
                _status = ProcessStatus.打开;
                _sec = 0;
                _reveice_index = 0;
                _reveicebuf = new byte[1024];
                _timerThread.Join(300);
                _readThread.Join(300);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 发送命令,等待下位机消息后,再执行SendAttachDataOfCommand()
        /// _status != ProcessStatus.未知, 有效
        /// 发送完成后,根据下位机的应答设置_status 为[发送附加数据]或[ 等待执行结果]
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(BusinessCommandOfSerialPort cmd)
        {
            if (_status == ProcessStatus.未知)
            {
                return;
            }
            if (_serialPort.IsOpen)
            {
                byte[] command = new byte[]{cmd.Assign_Main_ID ,cmd.Assign_Sub_ID ,
                                        cmd.AttachDataLength ,cmd.Loop_Display_Result ,
                                        cmd.Loop_Display_Process ,cmd.Loop_Display_Device ,
                                        cmd.Exec_AfterCompleted ,cmd.Timeout 
                                            };
                _status = ProcessStatus.命令发出;
                _reveice_index = 0;

                _serialPort.DiscardOutBuffer();
                _serialPort.DiscardInBuffer();
                Console.WriteLine("send cmd:");
                for (int i = 0; i < command.Length; i++)
                {
                    Console.Write(command[i].ToString());
                }
                Console.WriteLine();
                _serialPort.Write(command, 0, command.Length);
                //延迟
                //Core.General.Delay_MS(20);
            }
        }

        /// <summary>
        /// 封装data,以每块256字节发送命令附属数据
        /// [_status == ProcessStatus.发送附加数据] 时,有效
        /// 发送完成后,_status 被置为[ 等待执行结果]
        /// 至少有一个字节
        /// </summary>
        /// <param name="attachdata">实际要发送的命令附属数据</param>
        /// <param name="length">实际要发送的命令附属数据长度</param>
        public void SendAttachDataOfCommand(byte[] attachdata, long length)
        {
            if (_status != ProcessStatus.发送附加数据)
            {
                return;
            }
            long _index = 0;
            byte[] l = Core.General.GetHexBytes((int)length, 6);
            byte hasdata = 0;//没有数据了
            int blockCount = 0;
            do
            {
                long packLength = length - _index;
                if (packLength > 246)
                {
                    packLength = 246;
                    hasdata = 1;//还有数据
                }
                else
                    hasdata = 0;//没有数据了
                byte[] t = new byte[packLength + 10];
                t[0] = (byte)'.';
                t[1] = hasdata;
                Array.Copy(l, 0, t, 2, 6);
                Array.Copy(attachdata, _index, t, 8, packLength);
                t[packLength + 8] = (byte)'<';//最后2个字节
                t[packLength + 9] = (byte)'<';//最后2个字节
                if (packLength > 0)
                {
                    _serialPort.Write(t, 0, t.Length);
                    //int _pos = 0;
                    //Console.WriteLine("send:");
                    //for (int i = 0; i < t.Length; i++)
                    //{
                    //    Console.Write(t[i].ToString());
                    //    Console.Write(" ");
                    //    if (_pos >= 50)
                    //    {
                    //        Console.WriteLine("send:");
                    //        _pos = 0;
                    //    }
                    //}
                    //Console.WriteLine("send:");
                    //_pos = 0;
                    //for (int i = 0; i < t.Length; i++)
                    //{
                    //    Console.Write(Core.General.GetHexString(t[i]));
                    //    Console.Write(" ");
                    //    if (_pos >= 50)
                    //    {
                    //        Console.WriteLine("send:");
                    //        _pos = 0;
                    //    }
                    //}
                    General.Delay_MS(2500);  //大数据块必须要的延迟
                }
                blockCount++;
                _index += packLength;
            } while (hasdata > 0);
            _status = ProcessStatus.等待执行结果; 
        }

        void Read()
        {
            while (true )
            {
                bool dataVailde = false;
                if (_serialPort.IsOpen && _status != ProcessStatus.未知)
                {
                    try
                    {
                        //string test = Encoding.Default.GetString(_debugbuf, 0, _debug_index);
                        if (_serialPort.BytesToRead > 0)
                        {
                            byte[] buf = new byte[16];
                            //每次最多读16字节,防止干扰
                            int length = _serialPort.BytesToRead;
                            if (length > 16)
                            {
                                length = 16;
                            }
                            _serialPort.Read(buf, 0, length);
                            if ((_debug_index + length) > _debugbuf.Length)
                            {
                                _debug_index = 0;
                            }
                            Array.Copy(buf, 0, _debugbuf, _debug_index, length);
                            _debug_index += length;

                            //int _pos = 0;
                            //Console.WriteLine("接收:");
                            //for (int i = 0; i < buf.Length; i++)
                            //{
                            //    Console.Write(Core.General.GetHexString(buf[i]));
                            //    Console.Write(" ");
                            //    if (_pos >= 25)
                            //    {
                            //        Console.WriteLine("接收");
                            //        _pos = 0;
                            //    }
                            //}
                            if (_status == ProcessStatus.监听)
                            {
                                for (int i = 0; i < length; i++)
                                {
                                    if (buf[i] == (byte)'>')
                                    {
                                        _status = ProcessStatus.等待命令;
                                        _serialPort.DiscardInBuffer();
                                        break;
                                    }
                                }
                                if (StatusChangedCallback != null)
                                {
                                    StatusChangedCallback(this, _status);
                                }
                            }
                            else if (_status == ProcessStatus.命令发出)
                            {
                                for (int i = 0; i < length; i++)
                                {
                                    if (buf[i] == (byte)'.')
                                    {
                                        _status = ProcessStatus.发送附加数据;
                                        if ((i + 1) < length)
                                        {
                                            //把剩下的数据拷贝到缓存.
                                            Array.Copy(buf, i + 1, _reveicebuf, _reveice_index, length - i);
                                            _reveice_index += (length - i);
                                        }
                                        break;
                                    }
                                    else if (buf[i] == (byte)'<')
                                    {
                                        _status = ProcessStatus.等待执行结果;
                                        if ((i + 1) < length)
                                        {
                                            //把剩下的数据往前移动i,因为有的执行结果也跟着一起来了.
                                            byte[] tmp_buf = new byte[length - (i + 1)];

                                            Array.Copy(buf, i + 1, tmp_buf, 0, tmp_buf.Length);
                                            buf = new byte[16];
                                            Array.Copy(tmp_buf, buf, tmp_buf.Length);
                                            length = tmp_buf.Length;
                                        }
                                        else
                                        {
                                            //丢弃
                                            buf = new byte[16];
                                            length = 0;
                                        }
                                        break;
                                    }
                                }
                                if (StatusChangedCallback != null)
                                {
                                    StatusChangedCallback(this, _status);
                                }
                            }
                            if (_status == ProcessStatus.等待执行结果)
                            {
                                byte[] data = new byte[256];
                                dataVailde = false;
                                int data_length = 0;
                                bool hasnext = false;
                                for (int i = 0; i < length; i++)
                                {
                                    if (buf[i] == (byte)'<' || buf[i] == (byte)'.')
                                    {
                                        if (buf[i] == (byte)'.')
                                            hasnext = true;
                                        Array.Copy(buf, 0, _reveicebuf, _reveice_index, length);
                                        _reveice_index += i;
                                        //组合好一个块的数据,给回调data
                                        Array.Copy(_reveicebuf, data, _reveice_index);
                                        data_length = _reveice_index;
                                        //把剩下的数据拷贝到缓存.
                                        _reveice_index = length - (i + 1);
                                        if (_reveice_index > 0)
                                        {
                                            Array.Copy(buf, i + 1, _reveicebuf, 0, _reveice_index);
                                        }
                                        dataVailde = true;
                                        break;
                                    }
                                }
                                if (dataVailde)
                                {
                                    dataVailde = false;
                                    if (DataLoop_DisplayCallback != null)
                                    {
                                        DataLoop_DisplayCallback(this, data, data_length, ref _status);
                                    }
                                    else if (DataLoop_Multi_DisplayCallback != null)
                                    {
                                        DataLoop_Multi_DisplayCallback(this, data, data_length,hasnext ,ref _status);
                                    }
                                }
                                else
                                {
                                    Array.Copy(buf, 0, _reveicebuf, _reveice_index, length);
                                    _reveice_index += length;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _status = ProcessStatus.未知;
                        Console.WriteLine(ex.Message);
                    }
                }
                //if (_serialPort.IsOpen)
                    _readThread.Join(200);
                //else
                //    _readThread.Join(1000);
            }
        }

        void Timetick()
        {
            while (true)
            {
                if (_status == ProcessStatus.监听 && _sec > 0 && _serialPort.IsOpen)
                {
                    if ((DateTime.Now.Ticks - _timeTick) > (_sec * 1000000))
                    {
                        _serialPort.Write(">>>>>>>>>>>>>>>>>");
                        _timeTick = DateTime.Now.Ticks;
                    }
                }
                _timerThread.Join(500);
            }
        }

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            Close(); 
            _readThread.Abort();
            _timerThread.Abort(); 
        }

        #endregion
    }

    public class BusinessCommandOfSerialPort
    {
        /// <summary>
        /// 主任务号
        /// </summary>
        public byte Assign_Main_ID { get; set; }
        /// <summary>
        /// 次任务号
        /// </summary>
        public byte Assign_Sub_ID { get; set; }
        /// <summary>
        /// 有无附加数据(0x00 无数据;0x01小数据;0x02大数据)1024为标准,最大32K
        /// </summary>
        public byte AttachDataLength { get; set; }
        /// <summary>
        /// 执行结果要求回显
        /// </summary>
        public byte Loop_Display_Result { get; set; }
        /// <summary>
        /// 执行进程要求回显
        /// </summary>
        public byte Loop_Display_Process { get; set; }
        /// <summary>
        /// 回显设备,0x00 POS和上位机;0x01POS;0x02上位机
        /// </summary>
        public byte Loop_Display_Device { get; set; }
        /// <summary>
        /// 完成后处理:0=待命;1=关闭连接并回主界面;3=关机(保留)
        /// </summary>
        public byte Exec_AfterCompleted { get; set; }
        /// <summary>
        /// 超时秒数
        /// </summary>
        public byte Timeout { get; set; }


    }
}
