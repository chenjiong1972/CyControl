using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
namespace UnvaryingSagacity.Core.Net
{
    public class TransService
    {
        public const string SERVICESRESPONSE = "Yes.";
        public const string SERVICESASK = "UnvaryingSagacity.Net.Service?";
        public const string SERVICESWELLCOME = "Wellcome UnvaryingSagacity.Net.Service.";

        public enum EnumTransDataType
        {
            未知 = 0,
            测试=1,
            数据,
            命令,
            提示,
            请求,
            应答,
            校验,
            欢迎,
            心跳,
        }

        public static char TransTypeToChar(EnumTransDataType TransType)
        {
            char b = '0';
            switch (TransType)
            {
                case EnumTransDataType.测试:
                    b = '1';
                    break;
                case EnumTransDataType.数据:
                    b = '2';
                    break;
                case EnumTransDataType.命令:
                    b = '3';
                    break;
                case EnumTransDataType.提示:
                    b = '4';
                    break;
                case EnumTransDataType.请求:
                    b = '5';
                    break;
                case EnumTransDataType.应答:
                    b = '6';
                    break;
                case EnumTransDataType.校验:
                    b = '7';
                    break;
                case EnumTransDataType.欢迎:
                    b = '8';
                    break;
                case EnumTransDataType.心跳:
                    b = '9';
                    break;
                default:
                    break;
            }
            return b;
        }

        public static EnumTransDataType CharToTransType(char TransType)
        {
            EnumTransDataType tDataType = EnumTransDataType.未知;
            switch (TransType)
            {
                case '1':
                    tDataType = EnumTransDataType.测试;
                    break;
                case '2':
                    tDataType = EnumTransDataType.数据;
                    break;
                case '3':
                    tDataType = EnumTransDataType.命令;
                    break;
                case '4':
                    tDataType = EnumTransDataType.提示;
                    break;
                case '5':
                    tDataType = EnumTransDataType.请求;
                    break;
                case '6':
                    tDataType = EnumTransDataType.应答;
                    break;
                case '7':
                    tDataType = EnumTransDataType.校验;
                    break;
                case '8':
                    tDataType = EnumTransDataType.欢迎;
                    break;
                case '9':
                    tDataType = EnumTransDataType.心跳;
                    break;
                default:
                    break;
            }
            return tDataType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSource">通过Encoding.Default.GetBytes()来转换</param>
        /// <returns>可以用方法Encoding.ASCII.GetChars()转出为字符数组</returns>
        public static byte[] EncodeToBase64(string strSource)
        {
            ///第一个字节不用
            byte[] Base64 = new byte[65];
            //'    Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
            int MidAsc, BackRe;
            int NextRe = 0;
            int i, Length;
            int j = 0;
            if (strSource.Length <= 0)
            {
                return default(byte[]);
            }
            #region 为数组赋固定值 Base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
            for (i = 1; i < 63; i++)
            {
                if (i == 1 || i == 27 || i == 53 || j == 79)
                {
                    j = 0;
                }
                else
                {
                    j++;
                }
                if (i < 27)
                {
                    Base64[i] = (byte)(('A') + j);

                }
                else if (i < 53)
                {
                    Base64[i] = (byte)(('a') + j);
                }
                else if (i < 63)
                {
                    Base64[i] = (byte)(('0') + j);
                }
            }
            Base64[63] = 43;
            Base64[64] = 47;
            #endregion
            byte[] strEncode = Encoding.Default.GetBytes(strSource);// new byte[0];
            byte[] ByteData = new byte[0];

            j = 0;

            long k = 0;
            Length = strEncode.Length;
            int l = Length;// (Length + 2);
            int rm = l % 3;
            if (rm > 0)
                l = ((l / 3) + 1) * 4;
            else
                l = (l / 3) * 4;
            Array.Resize<byte>(ref  ByteData, l);
            try
            {
                k = 0;
                for (i = 0; i < Length; i += 3)
                {
                    MidAsc = strEncode[i];
                    NextRe = MidAsc & 3;
                    MidAsc = (MidAsc & 252) / 4;
                    ByteData[k] = Base64[MidAsc + 1];
                    k++;
                    if (i == (Length - 1))
                        break;
                    MidAsc = strEncode[i + (1)];
                    BackRe = NextRe * 16;
                    NextRe = MidAsc & 15;
                    MidAsc = (MidAsc & 240) / 16 | BackRe;
                    ByteData[k] = Base64[MidAsc + 1];
                    k++;
                    if (i == (Length - 2))
                        break;
                    MidAsc = strEncode[i + (2)];
                    BackRe = NextRe * 4;
                    NextRe = MidAsc & 63;
                    MidAsc = (MidAsc & 192) / 64 | BackRe;
                    ByteData[k] = Base64[MidAsc + 1];
                    ByteData[k + 1] = Base64[NextRe + 1];
                    k += 2;
                }
                if ((Length - i) == 1)
                {
                    NextRe = NextRe * 16;
                    ByteData[k] = Base64[NextRe + 1];
                    ByteData[k + 1] = 61;//"="
                    ByteData[k + 2] = 61;
                }
                else if ((Length - i) == 2)
                {
                    NextRe = NextRe * 4;
                    ByteData[k] = Base64[NextRe + 1];
                    ByteData[k + 1] = 61;
                }
                //char[] result = Encoding.ASCII.GetChars(ByteData);
                return ByteData;
            }
            catch (Exception ex)
            {
                return default(byte[]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">通过TCP传过来的都是ASC字符编码</param>
        /// <returns>可以用方法Encoding.*.GetChars()转出为字符数组或Encoding.*.GetBytes()转出为字节数组</returns>
        public static string UncodeFromBase64(char[] source)
        {
            byte BytePre = 0;
            byte ByteNext = 0;
            int j;
            int i;
            int Length;
            int MidAsc = 0;
            int lngAsc;
            int k;

            Length = source.Length;
            byte[] ByteData = new byte[(Length * 3) / 4];
            j = 1;
            k = 0;
            for (i = 0; i < Length; i++)
            {
                lngAsc = source[i];

                //以下是根据BASE64来计算前两位为零的编码
                if (lngAsc >= 48 && lngAsc <= 57)
                    MidAsc = lngAsc + 4;
                if (lngAsc >= 65 && lngAsc <= 90)
                    MidAsc = lngAsc - 65;
                if (lngAsc >= 97 && lngAsc <= 122)
                    MidAsc = lngAsc - 71;
                if (lngAsc == 43)
                    MidAsc = 62;
                if (lngAsc == 47)
                    MidAsc = 63;
                if (lngAsc == 61)
                    MidAsc = 32;

                if (j == 1)
                {
                    BytePre = (byte)(MidAsc * 4);
                    j = 2;
                }
                else if (j == 2)
                {
                    ByteNext = (byte)((MidAsc & 0x30) / 16);
                    ByteData[k] = (byte)(BytePre | ByteNext);
                    BytePre = (byte)((MidAsc & 15) * 16);
                    k++;
                    j = 3;
                }
                else if (j == 3)
                {
                    ByteNext = (byte)((MidAsc & 60) / 4);
                    ByteData[k] = (byte)(BytePre | ByteNext);
                    BytePre = (byte)((MidAsc & 3) * 64);
                    k++;
                    j = 4;
                }
                else
                {
                    ByteData[k] = (byte)(BytePre | MidAsc);
                    k++;
                    j = 1;
                }

            }
            string s = Encoding.Default.GetString(ByteData);
            return s;
        }

        static void SetLastError(StringBuilder sb, string Message)
        {
            sb.Remove(0, sb.Length);
            sb.Append(Message);
        }
        static void SetLastError(StringBuilder sb, string Message, bool Append)
        {
            if (Append)
                sb.Append(Message);
            else
            {
                SetLastError(sb, Message);
            }
            Console.WriteLine(Message);
        }

        /// <summary>
        /// sender is tcpClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Data"></param>
        public delegate void TransEventHandler(object sender, TransEventArgs Data);
        
        /// <summary>
        /// sender is tcpClient
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="charCount"></param>
        public delegate void ReceivingEventHandler(object sender, int charCount);

        public class TransEventArgs : EventArgs
        {
            private char[] data;
            private EnumTransDataType transType;

            public TransEventArgs(EnumTransDataType TransType, char[] Data)
            {
                data = Data;
                transType = TransType;
            }

            public char[] ToCharArrary()
            {
                return data;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(data);
                return sb.ToString();
            }

            public EnumTransDataType TransType
            {
                get { return transType; }
            }

            public int Length
            { get { return data.GetLength(0); } }
        }

        public class DataTansService
        {
            const int READ_BUFFER_SIZE = 1024 * 10;
            private TcpClient client;
            private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
            private char[] receiveBuff = new char[0];
            private int dataAvailableIndex = 0;
            private string key;
            private string hostName = "";
            private StringBuilder sbError = new StringBuilder();
            private int port = 0;
            private long lastTimeTick = 0;
            private bool available = false;

            public event TransEventHandler Received;
            public event ReceivingEventHandler Receiving;

            public DataTansService()
            {
            }

            /// <summary>
            /// HostName为远程主机的名字或IP地址
            /// </summary>
            /// <param name="Key"></param>
            /// <param name="HostName"></param>
            /// <param name="Port"></param>
            public DataTansService(string Key, string HostName, int Port)
            {
                key = Key;
                hostName = HostName;
                port = Port;
            }

            public Log Log { get; set; }

            public TcpClient tcpClient
            {
                get { return client; }
                set { client = value; }
            }
            /// <summary>
            /// HostName为远程主机的名字或IP地址
            /// </summary>
            public string HostName
            {
                get { return hostName; }
                set { hostName = value; }
            }

            public int Port
            {
                get { return port; }
                set { port = value; }
            }

            public string Key
            {
                get { return key; }
                set { key = value; }
            }

            public bool Available
            {
                get { return available; }
                set { available = value; }
            }

            public int LastSeconds
            {
                get
                {
                    if (lastTimeTick > 0)
                        return (int)((DateTime.Now.Ticks - lastTimeTick) / 10000000);//ticks的单位是100毫微秒
                    else
                        return 0;
                }
            }

            public string GetLastError()
            {
                return sbError.ToString();
            }

            public void Close()
            {
                if (client != null)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            SendData(EnumTransDataType.请求, "closed");
                            client.GetStream().Close(5000);
                        }
                        client.Close();
                        client.Client.Close(5000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        if (Log != null)
                            Log.Write(ex.Message);
                    }
                    finally
                    {
                        client.Client.Close(5000);
                        client = null;
                    }
                }
                dataAvailableIndex = 0;
            }

            /// <summary>
            /// 由hostName, port建立TcpClient
            /// </summary>
            /// <returns></returns>
            public bool CreateLink()
            {
                try
                {
                    return CreateLink(new TcpClient(hostName, port));
                }
                catch (Exception e)
                {
                    if (Log != null) 
                        Log.Write("CreateLink: " + e.Message);
                    return false;
                }
            }
            public bool CreateLink(TcpClient tcpClient)
            {
                if (dataAvailableIndex > 0)
                    dataAvailableIndex = 0;
                try
                {
                    if (client != null)
                    {
                        if (client.Connected)
                            client.Close();
                        client = null;
                    }
                    client = tcpClient;
                    client.NoDelay = true;
                    //client.Connect(); 这行代码是否需要, 必须进行实际测试

                    // 这里开始一个异步读取线程
                    // 数据保存到 readBuffer.
                    client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
                    return client.Connected;
                }
                catch (Exception e)
                {
                    SetLastError(sbError, e.Message);
                    return false;
                }
            }

            /// <summary>
            /// 自行编码
            /// </summary>
            /// <param name="TransType"></param>
            /// <param name="Data">最高位不能为1</param>
            public void SendData(EnumTransDataType TransType, char[] Data)
            {
                if (client == null)
                    return;
                if (!client.Connected)
                    return;
                //lock ensure that no other threads try to use the stream at the same time.
                lock (client.GetStream())
                {
                    StreamWriter streamWriter = new StreamWriter(client.GetStream());
                    streamWriter.AutoFlush = true;
                    char[] buff = new char[Data.Length + 2];

                    buff[0] = TransTypeToChar(TransType);
                    Array.Copy(Data, 0, buff, 1, Data.Length); 
                    buff[buff.Length - 1] = '.';//添加结尾标记
                    streamWriter.Write(buff);

                    //try
                    //{
                    //    streamWriter.Flush();// 立刻发送所有数据.
                    //}
                    //catch (FormatException ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //    if (Log != null)
                    //        Log.Write(ex.Message);
                    //}
                }
            }

            /// <summary>
            /// 采用Default和Base64编码发送字符串数据
            /// </summary>
            /// <param name="TransType"></param>
            /// <param name="Data"></param>
            public void SendData(EnumTransDataType TransType, string Data)
            {
                try
                {
                    if (client == null)
                        return;
                    if (!client.Connected)
                        return;
                    //lock ensure that no other threads try to use the stream at the same time.
                    lock (client.GetStream())
                    {
                        StreamWriter streamWriter = new StreamWriter(client.GetStream());
                        streamWriter.AutoFlush = true;
                        byte[] buff = Encoding.Default.GetBytes(Data);

                        char[] charBuff = new char[buff.Length * 2];
                        int i = Convert.ToBase64CharArray(buff, 0, buff.GetLength(0), charBuff, 1, Base64FormattingOptions.None);
                        charBuff[0] = TransTypeToChar(TransType);
                        charBuff[i + 1] = '.';//添加结尾标记
                        Array.Resize<char>(ref charBuff, i + 2);
                        streamWriter.Write(charBuff);

                        //try
                        //{
                        //    streamWriter.Flush();// 立刻发送所有数据.
                        //}
                        //catch (FormatException ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //    if (Log != null)
                        //        Log.Write(ex.Message);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (Log != null)
                        Log.Write(ex.Message);
                }
            }

            // 这是一个TcpClient.GetStream.Begin的回调方法.  
            // 开始与一个流的异步读.
            private void StreamReceiver(IAsyncResult ar)
            {
                int bytesRead;
                try
                {
                    lock (client.GetStream())
                    {
                        // 异步数据读入完成并且返回字节数
                        bytesRead = client.GetStream().EndRead(ar);
                        if (bytesRead < 1)
                            return;
                        else
                            lastTimeTick = System.DateTime.Now.Ticks;
                    }
                    char[] charBuff = Encoding.ASCII.GetChars(readBuffer, 0, bytesRead);
                    if (charBuff[charBuff.GetUpperBound(0)] == (char)13 || charBuff[charBuff.GetUpperBound(0)] =='.')
                    {
                        //一段数据结束
                        if (charBuff.Length > 1)
                        {
                            Append(charBuff);
                            if (Receiving != null)
                                Receiving(this, bytesRead);
                        }
                        try
                        {
                            //byte[] byteBuff = Convert.FromBase64CharArray(charBuff, 1, charBuff.Length - 2);//offset=1是因为传送类型在receiveBuff[0]
                            byte[] byteBuff = Convert.FromBase64CharArray(receiveBuff, 1, dataAvailableIndex-2);//offset=1是因为传送类型在receiveBuff[0]
                            char[] receivedChar = Encoding.Default.GetChars(byteBuff, 0, byteBuff.Length);
                            if (Received != null)
                                Received(this, new TransEventArgs(CharToTransType(charBuff[0]), receivedChar));
                            dataAvailableIndex = 0;
                        }
                        catch (Exception ex)
                        {
                            dataAvailableIndex = 0;
                            if (Log != null)
                                Log.Write(ex.Message); 
                            SetLastError(sbError, ex.Message); 
                        }
                    }
                    else
                    {
                        Append(charBuff);
                        if (Receiving != null)
                            Receiving(this, bytesRead);
                    }
                    if (client != null)
                    {
                        lock (client.GetStream())
                        {
                            // 开始一个异步读取到缓存
                            client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (Log != null)
                        SetLastError(sbError, e.Message);
                }
            }

            private void Append(char[] receiveChars)
            {
                int i = receiveBuff.GetLength(0);
                if (i < (dataAvailableIndex + receiveChars.GetLength(0)))
                {
                    Array.Resize<char>(ref receiveBuff, dataAvailableIndex + receiveChars.GetLength(0));
                }
                receiveChars.CopyTo(receiveBuff, dataAvailableIndex);
                dataAvailableIndex = dataAvailableIndex + receiveChars.GetLength(0);
            }
        }

        public class TransListener
        {
            private Core.ListDictionary<DataTansService> clientTables = new ListDictionary<DataTansService>();
            private TcpListener listener;
            private Thread listenerThread;
            private Thread connectionsThread;
            private IPAddress ip;
            private int port;
            private bool _enRefresh = false;
            
            public event TransEventHandler TransData;

            public bool MustVerify { get; set; }
            /// <summary>
            /// ip=IPAddress.GetAddressBytes() 是本地计算机需要侦听的IP
            /// IPAddress[] =  System.Net.Dns.GetHostAddresses( System.Net.Dns.GetHostName ())
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            public TransListener(byte[] ip, int port)
            {
                MustVerify = true;
                this.ip = new IPAddress(ip);
                this.port = port;
                listener = new TcpListener(this.ip, port);
                listenerThread = new Thread(new ThreadStart(DoListen));
                listenerThread.Start();
                connectionsThread = new Thread(new ThreadStart(RefreshConnectionsList));
                connectionsThread.Start();
            }

            public Core.ListDictionary<DataTansService> Clients
            {
                get { return clientTables; }
            }
            
            public Log Log { get; set; }

            public void Close()
            {
                try
                {
                    foreach (DataTansService dt in clientTables)
                    {
                        dt.Close();
                    }
                    clientTables.Clear(); 
                    listener.Stop();
                    listener = null;
                    listenerThread.Abort();
                    connectionsThread.Abort();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    if (Log != null)
                        Log.Write(ex.Message);
                }
            }

            private void DoListen()
            {
                try
                {
                    if (listener == null)
                        listener = new TcpListener(ip, port);
                    listener.Start();
                    while (true)
                    {
                        if (listener == null)
                            break;
                        if (listener.Pending())
                        {
                            _enRefresh = false;
                            DataTansService client = new DataTansService();
                            client.Key = Guid.NewGuid().ToString();
                            TcpClient tcpClient = listener.AcceptTcpClient();
                            if (client.CreateLink(tcpClient))
                            {
                                clientTables.Add(client.Key, client);
                                if (!MustVerify)
                                    client.Available = true;
                                Console.WriteLine("try Connect...{0},Count:{1}", client.Key, clientTables.Count.ToString());
                                clientTables[client.Key].Received += new TransEventHandler(client_Received);
                                Console.WriteLine("try Connect1");

                                ////StreamWriter streamWriter = new StreamWriter(clientTables[client.Key].tcpClient.GetStream());
                                ////Console.WriteLine("try Connect2");
                                //byte[] buff = Encoding.Default.GetBytes(client.Key);
                                //byte[] charBuff = new byte[buff.Length + 2];
                                //charBuff[0] = (byte)(TransTypeToChar(EnumTransDataType.校验));
                                ////char[] charBuff = new char[buff.Length * 2];
                                ////int i = Convert.ToBase64CharArray(buff, 0, buff.GetLength(0), charBuff, 1, Base64FormattingOptions.None);
                                ////charBuff[0] = TransTypeToChar(EnumTransDataType.校验);
                                //Array.Copy(buff, 0, charBuff, 1, buff.Length);
                                //charBuff[charBuff.Length - 1] = (byte)'.';//添加结尾标记
                                ////Array.Resize<char>(ref charBuff, i + 2);
                                //Console.WriteLine("try Connect2");
                                ////streamWriter.Write(charBuff);
                                //clientTables[client.Key].tcpClient.GetStream().Write(charBuff, 0, charBuff.Length);
                                //Console.WriteLine("try Connect3");
                                ////streamWriter.Flush();
                                ////Console.WriteLine("try Connect5");
                                //clientTables[client.Key].SendData(EnumTransDataType.校验, client.Key);
                            }
                            else
                            {
                                Console.WriteLine("CreateLink fail.{0},Count:{1}", client.Key, clientTables.Count.ToString());
                            }
                            _enRefresh = true;
                        }
                        listenerThread.Join(100);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (Log != null)
                        Log.Write(ex.Message);
                }
            }

            private void client_Received(object sender, TransEventArgs Data)
            {
                Console.WriteLine("client_Received:{0}",Data.ToString ()); 
                DataTansService o = (DataTansService)sender;
                if (Data.TransType == EnumTransDataType.校验)
                {
                    if (MustVerify)
                    {
                        //校验不合格 从cliendTables中删除.
                        if (Verify(o.Key, Data.ToString()))
                        {
                            o.Available = true;
                            o.SendData(EnumTransDataType.欢迎, SERVICESWELLCOME);
                        }
                        else
                            clientTables.Remove(o.Key);
                    }
                    else
                        o.Available = true;
                }
                else if (Data.TransType != EnumTransDataType.未知 && o.Available)
                {
                    if (Data.TransType == EnumTransDataType.请求 && Data.ToString().Equals("closed", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Remove:{0}",o.Key ); 
                        o.Close();
                        if (Log != null)
                            Log.Write("Remove:" + o.Key);
                        clientTables.Remove(o.Key);
                    }
                    else
                    {
                        if (TransData != null)
                            TransData(sender, Data);
                    }
                }
                
            }

            private void RefreshConnectionsList()
            {
                while (true)
                {
                    if (_enRefresh)
                    {
                        try
                        {
                            lock (clientTables)
                            {
                                foreach (DataTansService dt in clientTables)
                                {
                                    if (!_enRefresh)
                                        break;
                                    if (!dt.tcpClient.Connected)
                                    {
                                        dt.Close();
                                        clientTables.Remove(dt.Key);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Log != null)
                                Log.Write(ex.Message);
                        }
                    }
                    connectionsThread.Join(300);                   
                }
            }

            private bool Verify(string code1, string code2)
            {
                return true;
            }

        }

        public class UdpListener
        {

            private Thread listenerThread;
            private static bool messageReceived = true;
            private int listenPort = 0;
            private int responsePort = 0;

            private IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 0);
            private bool server = false;

            public delegate void RomoteIPEventHandler(object sender, IPEndPoint ipPoint);

            public event RomoteIPEventHandler RomoteIP;

            /// <summary>实现使用UDP广播得到服务器地址
            /// listenPort是侦听广播的端口. responsePort是回应端口.
            /// listenPort 不能和 responsePort相同.
            /// 这个构造函数应该用于服务器.而且responsePortt应该等于客户端的listenPor
            /// </summary>
            public UdpListener(int listenPort, int responsePort )
            {
                this.listenPort = listenPort;
                this.responsePort = responsePort;
                server = true;
                listenerThread = new Thread(new ThreadStart(DoListen));
                listenerThread.Start();
            }

            /// <summary>实现使用UDP广播得到服务器地址
            /// listenPort是侦听远程主机回应的端口
            /// 这个构造函数应该用于客户端.而且listenPort应该等于服务器的responsePort
            /// </summary>
            public UdpListener(int listenPort)
            {
                this.listenPort = listenPort;
                this.responsePort = 0;
                server = false;
                listenerThread = new Thread(new ThreadStart(DoListen));
                listenerThread.Name = "UdpListener"; 
                listenerThread.Start();
            }

            public Log Log { get; set; }

            public void Close()
            {
                try
                {
                    listenerThread.Abort();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private void ReceiveCallback(IAsyncResult ar)
            {
                UdpClient u = (UdpClient)(ar.AsyncState);
                IPEndPoint e = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    Byte[] receiveBytes = u.EndReceive(ar, ref e);
                    string receiveString = Encoding.ASCII.GetString(receiveBytes);

                    Console.WriteLine("", receiveString);
                    Console.WriteLine("Received: {0}, IP: {1}, Port: {2}", receiveString, e.Address.ToString(), e.Port.ToString());
                    if (Log != null)
                        Log.Write("Received: " + receiveString + ", IP:" + e.Address.ToString() + ", Port:" + e.Port.ToString());
                    if (receiveString.Equals(SERVICESASK) && server)
                    {
                        Byte[] sendBytes = Encoding.ASCII.GetBytes(SERVICESRESPONSE);
                        Console.WriteLine("send ,IP , Port: {0},{1} , {2}", SERVICESRESPONSE, e.Address.ToString(), e.Port.ToString());
                        if (Log != null)
                            Log.Write("send: "+ SERVICESRESPONSE +", IP:" +e.Address.ToString()+", Port:"+ e.Port.ToString());
                        u.Send(sendBytes, sendBytes.Length, new IPEndPoint(IPAddress.Broadcast, responsePort));
                    }
                    else if (receiveString.Equals(SERVICESRESPONSE))
                    {
                        RomoteIP(this, e);
                    }
                    messageReceived = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    messageReceived = true;
                    if (Log != null)
                        Log.Write(ex.ToString());
                }
            }

            private void DoListen()
            {
                try
                {
                    IPEndPoint e = new IPEndPoint(IPAddress.Any, listenPort);
                    UdpClient u = new UdpClient(e);
                    u.EnableBroadcast = true;
                    Console.WriteLine("listening for messages");
                    if (Log != null)
                        Log.Write("listening for messages");
                    while (true)
                    {
                        if (messageReceived)
                        {
                            messageReceived = false;
                            u.BeginReceive(new AsyncCallback(ReceiveCallback), u);
                        }
                        else
                            listenerThread.Join(300);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    if (Log != null)
                                    Log.Write(e.ToString());
                }
            }
        }

        /// <summary>使用UDP广播得到服务器地址
        /// 这只是一个样本. 应该在相应的登录界面实现.
        /// </summary>
        public class ServicesLocalizer
        {
            int port = 0;
            int broadcastPort = 0;
            private UdpListener udpListener;
            private IPEndPoint ipEndPoint;

            public event UnvaryingSagacity.Core.Net.TransService.UdpListener.RomoteIPEventHandler RomoteIP;
            /// <summary>使用UDP广播得到服务器地址,这只是一个样本. 应该在相应的登录界面实现.
            /// listenPort是侦听远程主机回应的端口. broadcastPort是广播的端口.
            /// listenPort 不能和 broadcastPort相同.
            /// </summary>
            
            public ServicesLocalizer(){}

            public Log Log { get; set; }

            public void Open(int listenPort, int broadcastPort)
            {
                port = listenPort;
                this.broadcastPort = broadcastPort;
                udpListener = new UdpListener(listenPort);
                udpListener.RomoteIP += new UdpListener.RomoteIPEventHandler(udpListener_RomoteIP);
            }

            void udpListener_RomoteIP(object sender, IPEndPoint ipPoint)
            {
                if (RomoteIP != null)
                    RomoteIP(sender, ipPoint);
                ipEndPoint = ipPoint;
            }

            public IPEndPoint Romote
            {
                get { return ipEndPoint; }
            }

            public void Broadcast()
            {
                try
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(SERVICESASK);
                    UdpClient udpClient = new UdpClient();
                    udpClient.EnableBroadcast = true;
                    if (Log != null)
                        Log.Write("Broadcast:" + SERVICESASK + ", IP:" + IPAddress.Broadcast.ToString() + ", Port:" + broadcastPort);
                    udpClient.Send(sendBytes, sendBytes.Length, new IPEndPoint(IPAddress.Broadcast, broadcastPort));
                    udpClient.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    if (Log != null)
                        Log.Write(e.ToString());
                }
            }

            public void Close()
            {
                if (udpListener != null)
                    udpListener.Close();
            }
        }


    }
}

