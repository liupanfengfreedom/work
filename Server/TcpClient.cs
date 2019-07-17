#define UTF16
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
namespace MatchServer
{
   // public delegate void OnReceivedCompleted(List<byte>mcontent);
    public delegate void OnReceivedCompleted(byte[] buffer);
 class TCPClient
    {
        public String map { private set; get; }
        public String vip { private set; get; }
        public String rank { private set; get; }
        public String nvn { private set; get; }

        /// <summary>
        /// //////////////////////////////////////////////
        /// </summary>
        public  Room room;
        public bool isinmatchpool=false;
        public bool mclosed = false;
        Socket clientsocket;
        public OnReceivedCompleted OnReceivedCompletePointer=null;
        bool entrymapok;
        const int BUFFER_SIZE = 65536;
        public byte[] receivebuffer = new byte[BUFFER_SIZE];
        Thread ReceiveThread;
        public bool getentrymapisok() {
            return entrymapok;
        }
        public TCPClient(Socket msocket)
        {
            Console.WriteLine("TCPClient "+ msocket.RemoteEndPoint);
            entrymapok = false;
            clientsocket = msocket;
            OnReceivedCompletePointer += messagehandler;
            ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();      
        }
        ~TCPClient()
        {
            Console.WriteLine("TCPClient In destructor.");
        }
        public void Send(byte[] buffer)
        {
            if (clientsocket != null)
            {
                clientsocket.Send(buffer);
            }
        }
        public void Send(String message)
        {
#if UTF16
            UnicodeEncoding asen = new UnicodeEncoding();
#else
            ASCIIEncoding asen = new ASCIIEncoding();
#endif
            if (clientsocket != null)
            {
                clientsocket.Send(asen.GetBytes(message));
            }
        }
        void ReceiveLoop()
        {
            while (true)
            {
                try {
                    Array.Clear(receivebuffer, 0, receivebuffer.Length);
                    clientsocket.Receive(receivebuffer);
                    OnReceivedCompletePointer?.Invoke(receivebuffer);
                    Thread.Sleep(30);
                }
                catch (SocketException)
                {
                    mclosed = true;
                    CloseSocket();
                    room.Remove(this);
                    ReceiveThread.Abort();
                }
            }

        }
        public void CloseSocket()
        {
            clientsocket.Close();
        }
        void messagehandler(byte[] buffer)
        {
            FMessagePackage mp;
#if UTF16
            var str1 = System.Text.Encoding.Unicode.GetString(buffer);
            mp = JsonConvert.DeserializeObject<FMessagePackage>(str1);
#else
            var str = System.Text.Encoding.UTF8.GetString(buffer);
            mp = JsonConvert.DeserializeObject<FMessagePackage>(str);
#endif
            switch (mp.MT) 
            {
                case MessageType.MATCH:
                    map = mp.PayLoad;
                    Console.WriteLine(map);
                    break;
                case MessageType.EntryMAPOK:
                    entrymapok = true;
                    break;
                case MessageType.EXITGAME:
                    mclosed = true;
                    CloseSocket();
                    room.Remove(this);
                    ReceiveThread.Abort();
                    break;
            }
        }
    }
}
