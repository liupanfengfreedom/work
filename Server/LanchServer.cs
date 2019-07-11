﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MatchServer
{
   public class Roomipprocess
    {
        public String mip;
        public Process mprocess;
    }

  public  class LanchServer
    {
        public const int startingport = 7777;
        public const string  serverip = "192.168.1.240";
        public static Process launchserver(int port)
        {
            string Arguments = string.Format(" -log=ue.log -port={0}",port);
            try
            {
                Process myProcess = new Process();
                //using (myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = @"C:\InfiniteLife1_0\win\WindowsNoEditor\InfiniteLife1_0\Binaries\Win64\UEWebsocketServer.exe";
                    //myProcess.StartInfo.FileName = @"G:\UE4 projects\InfiniteLife1_0\win\WindowsNoEditor\InfiniteLife1_0\Binaries\Win64\UEWebsocketServer.exe";
                    myProcess.StartInfo.Arguments = Arguments;
                    //myProcess.StartInfo.Arguments = " -log=myue.log  -port=7788";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
                return myProcess;
            }
            catch (Exception e)
            {
                return null;
                Console.WriteLine(e.Message);
            }
        }
        static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints;
            //ipEndPoints = ipProperties.GetActiveTcpListeners();
            ipEndPoints = ipProperties.GetActiveUdpListeners();
            
            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }
            return inUse;
        }
        static int GetOneAvailablePort()
        {          
            int counter = 0;
            bool b = PortInUse(startingport);
            while (b)
            {
                b = PortInUse(startingport + counter);
                if (b)
                {
                    counter++;
                }
            }         
            return startingport+counter;
        }
        public static Roomipprocess CreateOneRoom()
        {
            Roomipprocess rp = new Roomipprocess();
            int port = GetOneAvailablePort();
            rp.mprocess = launchserver(port);
            string roomip = serverip;
            roomip += ":";
            roomip += port.ToString();
            rp.mip = roomip;
            Console.WriteLine("CreateOneRoom" + roomip);         
            return rp;
        }
        
        public static string getlocalip()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostEntry(hostName).AddressList[0].ToString();
            return myIP;
        }
    }
}
