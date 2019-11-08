using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace test1
{
    class SerialReader
    {
        String data;
        //string uidString = "";
        int counter = 0;
        string aidString = "AID";
        string alarm = "Alarm";
        string  aid = "";
        public SerialPort myport;
        public SerialPort myport1;
        public SerialPort myport2;
        UnauthorizedAccessException exp = new UnauthorizedAccessException();

        public bool initialize( bool showerror=false)
        {
            try
            {
                myport = new SerialPort();
                myport.BaudRate = 115200;
                myport.PortName = "COM3";

                //myport1 = new SerialPort();
                //myport1.BaudRate = 115200;
                //myport1.PortName = "COM5";

                //myport2 = new SerialPort();
                //myport2.BaudRate = 96000;
                //myport2.PortName = "COM4";

                myport.Open();
                //myport1.Open();
                //myport2.Open();
                
            }
            catch (Exception e )
            {
                if (showerror)
                    Console.WriteLine(e.Message.ToString());
                return false;
            }
            return true;
        }

        public void Read()
        {

            data = myport.ReadLine();
           
            //Search for AID Value: //2 bytes
            //If this "If condition" is executed, this means the alarm is ON.
            if (data.Contains(aidString))
            {

                aid = data.Split(new string[] { "AID " }, StringSplitOptions.None).Last().Replace("\r", "");
                Console.WriteLine(counter++ + " Alarm!!! | AID = " + aid);
                Transmit(aid);

            }


            //string idValues = string.Concat(uid, aid);

            //if (!String.IsNullOrEmpty(uid) && !String.IsNullOrEmpty(aid))
            //{
            //    //Transmit - Full Payload
            //    Transmit(idValues);
            //}



        }
        public void close()
        {
            
            myport.Close();
            //myport1.Close();
            //myport2.Close();

        }

        //Send Data to Server
        public void Write()
        {
            Console.WriteLine(data);

        }

        //Transmit to Server (Allah y7r2o bl Java)
        public void Transmit(string aid)
        {

            string sURL;
            sURL = "http://197.50.41.222:8084/Rms/HWStolen?&hwnum=" + aid;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;

            wrGETURL.Proxy = WebProxy.GetDefaultProxy();

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sLine = "";
            int i = 0;
            Console.WriteLine("Reading server output");
            while (sLine != null)
            {
                i++;
                sLine = objReader.ReadLine();
                if (sLine != null)
                    Console.WriteLine("{0}:{1}", i, sLine);
            }

            //2 bytes - AID

            /*string alarmAID = String.Join(" ",
                aid.ToCharArray().Aggregate("",
                        (result, c) => result += ((!string.IsNullOrEmpty(result) &&
                                                   (result.Length + 1) % 3 == 0) ? " " : "") + c.ToString())
                    .Split(' ').ToList().Select(
                        x => x.Length == 1
                            ? String.Format("{0}{1}", Int32.Parse(x) - 1, x)
                            : x).ToArray());

            byte[] bytesStream = alarmAID
                .Split(' ') // Split into items 
                .Select(item => Convert.ToByte(item, 16)) // Convert each item into byte
                .ToArray();*/


        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                // free managed resources
                if (myport != null)
                {
                    myport.Dispose();
                    myport = null;
                }
            }
            // free native resources if there are any.
        }
     }
}
