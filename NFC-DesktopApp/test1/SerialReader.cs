using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
namespace test1
{
    class SerialReader
    {
        String data;
        string uidString = "NFCID";
        string aidString = "AID";
        string uid = "", aid = "";
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
                myport.PortName = "COM4";

                //myport1 = new SerialPort();
                //myport1.BaudRate = 115200;
                //myport1.PortName = "COM5";

                //myport2 = new SerialPort();
                //myport2.BaudRate = 96000;
                //myport2.PortName = "COM4";

                myport.Open();
                //myport1.Open();
            }
            catch(Exception e )
            {
                if (showerror)
                    Console.WriteLine(e.Message.ToString());
                return false;
            }
            return true;
        }

        //Read Data from NFC 
        public void Read()
        {

            data = myport.ReadLine();

            //Search for NFCID Value: //7 bytes
            if (data.Contains(uidString))
            {
                uid = data.Split(new string[] { "NFCID " }, StringSplitOptions.None).Last().Replace("\r", "");
                char[] charUID = uid.ToCharArray();
                Console.WriteLine("NFC ID: " + uid);

            }
            //Search for AID Value: //2 bytes
            if (data.Contains(aidString))
            {
                aid = data.Split(new string[] { "AID " }, StringSplitOptions.None).Last().Replace("\r", "");
                Console.WriteLine("Arduino ID: " + aid);
            }

            
            if (!String.IsNullOrEmpty(uid) && !String.IsNullOrEmpty(aid))
            {
                //Transmit - Full Payload
                Transmit(aid, uid);
            }



        }
        public void close()
        {
            
            myport.Close();
     
        }

        //Send Data to Server
        public void Write()
        {
            Console.WriteLine(data);

        }

        //Transmit to Server (Allah y7r2o bl Java)
        public void Transmit(string aid, string uid)
        {
            //string dTime = DateTime.Now.ToString("HHmm");
            //string dDate = DateTime.Now.ToString("ddMMyy");
            //string dateTime = string.Concat(dTime, dDate);

            //Payload = <UID,AID> + <dTime,dDate>

            //string fullPayload = "";
            //fullPayload = string.Concat(idValues, dateTime);

            string sURL;
            sURL = "http://197.50.41.222:8084/Rms/AddLog?idSerial=" + uid+"&hwNum="+aid;

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
            

           /* string newIDValues = String.Join(" ",
                idValues.ToCharArray().Aggregate("",
                        (result, c) => result += ((!string.IsNullOrEmpty(result) &&
                                                   (result.Length + 1) % 3 == 0) ? " " : "") + c.ToString())
                    .Split(' ').ToList().Select(
                        x => x.Length == 1
                            ? String.Format("{0}{1}", Int32.Parse(x) - 1, x)
                            : x).ToArray());*/

            //Array of Bytes Ready to the Server
            //4 or 7 bytes - NFC ID
            //2 bytes - AID

            //Removed
            //1 byte - hour
            //1 byte - min
            //1 byte - day
            //1 byte - month
            //1 byte - year
            //Total : 14 bytes

            /*byte[] bytesStream = newIDValues
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
