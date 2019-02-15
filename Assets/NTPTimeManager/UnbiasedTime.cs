//Copyright(c) 2017 �mer Faruk Say�l�r

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using UnityEngine;
using System;

using System.Net;
using System.IO;

#if UNITY_EDITOR
using System.Net.Sockets;
#endif

#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking;
using System.Text;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif

namespace UnbiasedTimeManager
{

    /// <summary>
    /// Unbiased time combines Unity3D's Time.unscaledTime and NTP server times to report
    /// more reliable device independent Times. 
    /// 
    /// It returns time with uint format in milliseconds
    /// 
    /// If you desire to use another ntp pool servers just changle 
    /// ntpServer = "time.google.com";
    /// </summary>
    public class UnbiasedTime : MonoBehaviour
    {

#region Variables

#region Static Variables
        
        private static UnbiasedTime m_Instance;

        public static UnbiasedTime Instance
        {
            get
            {
                if (!m_Instance)
                {
                    GameObject newObject = new GameObject("UnbiasedTimeManager", typeof(UnbiasedTime));
                    m_Instance = newObject.GetComponent<UnbiasedTime>();
                }
                return m_Instance;
            }
            private set { m_Instance = value; }
        }

        private static string ntpServer = "time.google.com";

#endregion

#region Network Variables

#if UNITY_EDITOR
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ipEndPoint;
#endif
#if !UNITY_EDITOR
        DatagramSocket socket;
        private DataWriter writer;
        
        [NonSerialized]
        public bool timeReceived = false;
        [NonSerialized]
        public ulong milliseconds = 0;
        private bool dgramSocketConnected = false;
#endif
        bool hasIp;

#endregion

#region Option Variables
        public ulong refreshInterval = 64;
        public int maxRetry = 3;
        int trycount = 0;
#endregion

#region Utility

        private ulong lastUpdatedGameTime;
        private ulong lastNetworkTime;

        private ulong timeSinceLastUpdate
        {
            get { return (ulong)(UnityEngine.Time.unscaledTime * 1000) - lastUpdatedGameTime; }
        }

        public ulong time
        {
            get { return lastNetworkTime + timeSinceLastUpdate; }
        }

        public DateTime dateTime
        {
            get { return GetDateTime(time); }
        }

        private bool isUpdating;
        public bool failed;
        public bool TimeSynchronized = false;

        /// <summary>
        /// This event triggers when time received from server succesfully or failed during process.
        /// "bool" = is Succes
        /// "uint" = time
        /// </summary>
        public event Action<bool, ulong> onTimeReceive;

#endregion

#endregion

        public static void Init(string ntp_server = "time.google.com")
        {
            ntpServer = ntp_server;
            GameObject newObject = new GameObject("UnbiasedTimeManager", typeof(UnbiasedTime));
            m_Instance = newObject.GetComponent<UnbiasedTime>();            
        }

        private void Awake()
        {
            Instance = this;
            Initialize();
        }
        

        private void Update()
        {
            if (timeSinceLastUpdate > refreshInterval * 1000)
            {
                GetTime();
            }
        }
        
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            trycount = 0;
            GetTime();
        }

        public void GetTime()
        {
            if (!isUpdating)
            {
                isUpdating = true;
                StartCoroutine(RequestTime());
            }
        }

        bool RequestIPEndpoint()
        {
#if UNITY_EDITOR
            Debug.Log("Requesting IP");
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ntpServer);
                IPAddress[] addresses = entry.AddressList;
                ipEndPoint = new IPEndPoint(addresses[0], 123);
                hasIp = true;
                return true;
            }
            catch (Exception)
            {
                hasIp = false;
                return false;
            }
#endif
#if !UNITY_EDITOR
            hasIp = true;
            return true;
#endif
        }

#if UNITY_EDITOR
        void Connect()
        {
            Debug.Log("Connecting");

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(ipEndPoint);
        }
#endif
#if !UNITY_EDITOR
        async Task Connect() {
            Debug.Log("Connecting");
            await ConnectUWP();
        }
#endif
        

#if !UNITY_EDITOR
        async Task ConnectUWP() {
            socket = new DatagramSocket();
            await socket.ConnectAsync(new HostName(ntpServer), "123");

            //writer = new StreamWriter(socket.OutputStream.AsStreamForWrite()) { AutoFlush = true };
            IOutputStream outStream = await socket.GetOutputStreamAsync(new HostName(ntpServer), "123");
            writer = new DataWriter(outStream);

            //Stream streamIn = socket.InputStream.AsStreamForRead();
            //reader = new StreamReader(streamIn);
            socket.MessageReceived += OnReceivedMsg;
            dgramSocketConnected = true;
        }
#endif

        void Disconnect()
        {
#if UNITY_EDITOR
            socket.Close();
#endif
#if !UNITY_EDITOR
            writer.DetachStream();
            writer.Dispose();
            socket.Dispose();
            socket = null;
            dgramSocketConnected = false;
#endif
        }

#if UNITY_EDITOR
        void Reconnect()
        {
            Debug.Log("Reconnecting");
            Disconnect();
            Connect();
        }
#endif
#if !UNITY_EDITOR
        async Task Reconnect() {
            Debug.Log("Reconnecting");
            Disconnect();
            await Connect();
        }
#endif
        public bool IsConnected()
        {
#if UNITY_EDITOR
            if (socket.Connected)
            {
                bool connectionStatus;
                bool blockingState = socket.Blocking;
                try
                {
                    var ntpData = new byte[1];
                    ntpData[0] = 0x00;
                    socket.Send(ntpData);
                    connectionStatus = true;
                }
                catch (SocketException)
                {
                    connectionStatus = false;
                }
                return connectionStatus;
            }
            else
            {
                return false;
            }
#endif
#if !UNITY_EDITOR
            return dgramSocketConnected;
#endif
        }

#if UNITY_EDITOR
        bool SatisfyConnection()
        {
            if (IsConnected())
            {
                return true;
            }
            else if (hasIp)
            {
                Reconnect();
                return IsConnected();
            }
            else
            {
                if (RequestIPEndpoint())
                {
                    Connect();
                    return IsConnected();
                }
                else
                {
                    return false;
                }
            }
        }
#endif

#if !UNITY_EDITOR
        async Task<bool> SatisfyConnection() {
            if (IsConnected()) {
                return true;
            }
            else if (hasIp) {
                await Reconnect();
                return IsConnected();
            }
            else {
                if (RequestIPEndpoint()) {
                    await Connect();
                    return IsConnected();
                }
                else {
                    return false;
                }
            }
        }
#endif

#if UNITY_EDITOR
        public ulong TryToGetTime(out bool isSucces, float timeout = 3)
        {
            if (SatisfyConnection())
            {
                ulong time = GetNetworkTimeMillis(out isSucces, timeout);
                if (isSucces)
                {
                    return time;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                isSucces = false;
                return 0;
            }
        }
#endif

#if !UNITY_EDITOR
        public async void TryToGetTime(float timeout = 3) {     
            bool connected = await SatisfyConnection();
            if (connected)
            {
                GetNetworkTimeMillis(timeout);
            }
        }
#endif


#if UNITY_EDITOR
        public ulong GetNetworkTimeMillis(out bool success, float timeout = 6)
        {
            Debug.Log("Getting Time From Server: " + ntpServer);
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            int recievedBytes;

            try
            {
                socket.ReceiveTimeout = (int)(timeout * 1000);
                socket.SendTimeout = (int)(timeout * 1000);
                socket.Send(ntpData);
                recievedBytes = socket.Receive(ntpData);
            }
            catch (SocketException)
            {
                success = false;
                return 0;
            }

            
            if (recievedBytes == 48)
                success = true;
            else
                success = false;

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            return milliseconds;
        }
#endif
#if !UNITY_EDITOR
        public async void GetNetworkTimeMillis(float timeout = 6) {
            Debug.Log("Getting Time From Server: " + ntpServer);
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;

            writer.WriteBytes(ntpData);
            await writer.StoreAsync();
        }

        private void OnReceivedMsg(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args) {
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            int receivedBytes = 0;

            args.GetDataReader().ReadBytes(ntpData);
            receivedBytes = ntpData.Length;


            Debug.Log("Message received!");

            if (receivedBytes == 48) {
                ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
                ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

                ulong _milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

                timeReceived = true;
                milliseconds = _milliseconds;
            }
            else {
                timeReceived = false;
                milliseconds = 0;
            }
        }
#endif
        public static DateTime GetDateTime(ulong milliseconds)
        {
            return (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
        }

        IEnumerator RequestTime()
        {
            GetNetworkTime getTime = new GetNetworkTime(this, 5);
            yield return getTime;
            trycount++;
            if (getTime.isSuccess)
            {
                lastNetworkTime = getTime.time;
                lastUpdatedGameTime = (ulong)(UnityEngine.Time.unscaledTime * 1000);
                if (onTimeReceive != null)
                    onTimeReceive(true, time);
                isUpdating = false;
                trycount = 0;
                TimeSynchronized = true;
            }
            else
            {
                if (trycount < maxRetry)
                {
                    Debug.Log("Retrying to get server time");
                    StartCoroutine(RequestTime());
                }
                else
                {
                    Debug.Log("Failed to get server time");
                    if (onTimeReceive != null)
                        onTimeReceive(false, 0);
                    failed = true;
                }
            }
        }

    }
}