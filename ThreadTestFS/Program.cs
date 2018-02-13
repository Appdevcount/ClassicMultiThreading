using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ThreadTestFS
{
    class Program
    {
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        static void Main(string[] args)
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                Console.WriteLine("Another Instance of Application is already Running..");
                Console.WriteLine("");
                Console.WriteLine("**********Closing This instance in 2 seconds***********");
                Thread.Sleep(2000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            bool TRUE = true;
            while (TRUE)
            {
                string[] ScodeDirs = Directory.GetDirectories(@"D:\msg\");

                int ScodeStackcnt = ScodeDirs.Length;
                foreach (string scodedir in ScodeDirs)
                {
                    string scodefoldername = scodedir.Substring(scodedir.Length - 4);
                    Console.WriteLine("###### Current Folder " + scodefoldername + " ######");
                    if (Regex.IsMatch(scodefoldername, "^[0-9]*$"))
                    {
                        string[] scfiles = Directory.GetFiles(scodedir + @"\dr\" + scodefoldername + @"\", "*.XML");
                        int Totfiles = scfiles.Length;
                        Console.WriteLine("Pending No. Of Files to Process >>> " + Totfiles);
                        Console.WriteLine("Pending No. Of Files to Process >>> " + Totfiles);
                        Console.WriteLine("Pending No. Of Files to Process >>> " + Totfiles);
                        var tasks = new Task[Totfiles];


                        using (var finished = new CountdownEvent(1))
                        {

                            foreach (string file in scfiles)
                        {
                            int tasknum = Array.FindIndex(scfiles, x => x.Contains(file));
                            Dictionary<string, Filehandlerclass> dict = new Dictionary<string, Filehandlerclass>();
                            string key = Path.GetFileName(file).ToString();
                            dict[key] = new Filehandlerclass();
                            //tasks[tasknum] = Task.Factory.StartNew(() => dict[key].onefile(file));

                            //if (tasknum % 30 == 0)
                            //{
                            //    //Task.WaitAll(tasks);
                            //    Thread.Sleep(3000);
                            //}

                            ThreadPool.SetMaxThreads(150000, 30);
                            //ThreadPool.QueueUserWorkItem((s) => dict[key].onefile(file));
                            finished.AddCount();
                            ThreadPool.QueueUserWorkItem((a) =>
                            {
                                try
                                {
                                    dict[key].onefile(file);

                                }
                                finally
                                {
                                    finished.Signal();
                                }
                            });

                            //Console.WriteLine( tasks[tasknum].Status.ToString());


                        }
                        finished.Signal();
                        finished.Wait();
                        }
                        //Task.WaitAll(tasks);
                    }
                    ScodeStackcnt--;
                    Console.WriteLine("Pending No. Of Folders to Process >>> " + ScodeStackcnt);
                    Console.WriteLine("Pending No. Of Folders to Process >>> " + ScodeStackcnt);
                    Console.WriteLine("Pending No. Of Folders to Process >>> " + ScodeStackcnt);
                }
            }
            Console.WriteLine("Completed one cycle of checking the DR Files.. \n Please rerun the Program from taskbar to Begin again...");
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }

    }

    public class Filehandlerclass
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(Program));


        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();


        public void onefile(string file)
        {
            try
            {

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);


                string ID, ShortCode, OperatorId, ConnectionId, Text, LanguageId, date, status, error;
                Int64 MSISDN, messageId;

                ID = xmlDoc.SelectSingleNode("SMS/ID").InnerText;
                MSISDN = Convert.ToInt64(xmlDoc.SelectSingleNode("SMS/MSISDN").InnerText);
                ShortCode = xmlDoc.SelectSingleNode("SMS/ShortCode").InnerText;
                OperatorId = xmlDoc.SelectSingleNode("SMS/OperatorId").InnerText;
                ConnectionId = xmlDoc.SelectSingleNode("SMS/ConnectionId").InnerText;
                Text = xmlDoc.SelectSingleNode("SMS/Text").InnerText;
                LanguageId = xmlDoc.SelectSingleNode("SMS/LanguageId").InnerText;
                date = xmlDoc.SelectSingleNode("SMS/date").InnerText;
                status = xmlDoc.SelectSingleNode("SMS/status").InnerText;
                error = xmlDoc.SelectSingleNode("SMS/error").InnerText;
                messageId = Convert.ToInt64(xmlDoc.SelectSingleNode("SMS/messageId").InnerText);

                DateTime dated = Convert.ToDateTime(date);

                string fulldate = dated.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                string fulltime = dated.ToLongTimeString();
                string tdatetime = dated.ToString("yyyy-MM-dd HH:mm:ss"); ;

                string cs = @"Data Source=IKWM-APPVAS1\DBVAS1; Initial Catalog=VASIraq; User ID=sa;Password=Isys@969";


                string[] scodelist2 = { "2854", "2856", "2863", "2873", "2877", "2881", "2882" };
                string[] scodelist4 = { "4151", "4152", "4153", "4154", "4155", "4156", "4157", "4181" };

                if (status == "3")
                {
                    if (scodelist2.Contains(ShortCode))
                    {
                        //_readWriteLock.EnterWriteLock();
                        //try
                        //{
                        string CURRDATElogfile = DateTime.Now.ToString("dd-MM-yyyy HH:MM:SS");

                        //    if (File.Exists(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //    {
                        //        FileInfo fi = new FileInfo(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //        if (fi.Length > (1024 * 1024 * 20))
                        //        {
                        //            Random rndm = new Random();
                        //            File.Create(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt");
                        //            using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt"))
                        //            {
                        //                //Thread.Sleep(1000);
                        //                sw.WriteLine(dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                                   + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" );
                        //                sw.Close();
                        //            }
                        ////                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        ////FileShare.Write, 4096, FileOptions.None))
                        ////                {
                        ////                    using (StreamWriter writer = new StreamWriter(fs))
                        ////                    {
                        ////                        writer.AutoFlush = true;
                        ////                        //for (int i = 0; i < 20; ++i)
                        ////                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        ////                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" );
                        ////                    }
                        ////                }
                        //}
                        //        else
                        //        {
                        //            using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //            {
                        //                //Thread.Sleep(1000);
                        //                sw.WriteLine(dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                                   + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" );
                        //                sw.Close();
                        //            }
                        //        }
                        //    }
                        //    //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //    //FileShare.Write, 4096, FileOptions.None))
                        //    //                {
                        //    //                    using (StreamWriter writer = new StreamWriter(fs))
                        //    //                    {
                        //    //                        writer.AutoFlush = true;
                        //    //                        for (int i = 0; i < 20; ++i)
                        //    //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //    //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //    //                    }
                        //    //                }
                        //    //                //using (StreamWriter sw = new StreamWriter(@"D:\TEST\" + CURRDATElogfile + ".txt", true))
                        //    //                //{
                        //    //                //    sw.WriteLine("Unixv::::::" + "unixTimestamp ****" + "http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //    //                //       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //    //                //}
                        //    //                //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //    //                //FileShare.Write, 4096, FileOptions.None))
                        //    //                //                {
                        //    //                //                    using (StreamWriter writer = new StreamWriter(fs))
                        //    //                //                    {
                        //    //                //                        writer.AutoFlush = true;
                        //    //                //                        for (int i = 0; i < 20; ++i)
                        //    //                //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //    //                //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //    //                //                    }
                        //    //                //                }
                        //    //            }
                        //    else
                        //    {
                        //        File.Create(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //        using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile  + ".txt"))
                        //        {
                        //            //Thread.Sleep(1000);
                        //            sw.WriteLine(dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                               + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" );
                        //            sw.Close();
                        //        }
                        //        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //        //FileShare.Write, 4096, FileOptions.None))
                        //        //                {
                        //        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //        //                    {
                        //        //                        writer.AutoFlush = true;
                        //        //                        for (int i = 0; i < 20; ++i)
                        //        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //        //                    }
                        //        //                }
                        //    }

                        //    // Append text to the file
                        //    //    using (StreamWriter sw = File.AppendText(path))
                        //    //{
                        //    //    //Thread.Sleep(1000);
                        //    //    sw.WriteLine(text);
                        //    //    sw.Close();
                        //    //}
                        //}
                        //finally
                        //{
                        //    // Release lock
                        //    _readWriteLock.ExitWriteLock();
                        //}

                        //Console.WriteLine("http://spgw.atlas-me.com/PaymentProviders/Iraq/ZainDeliveryNotification.aspx?&shortCode=");
                        WebRequest request = WebRequest.Create("http://spgw.atlas-me.com/PaymentProviders/Iraq/ZainDeliveryNotification.aspx?&shortCode="
                                + ShortCode + "&msisdn=" + MSISDN + "&DRStatus=delivrd&DRID=" + messageId);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse response = request.GetResponse();
                        //Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription);
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromURL = reader.ReadToEnd();
                        Console.WriteLine(responseFromURL);
                        reader.Close();
                        response.Close();

                        if (responseFromURL == "OK" | responseFromURL == "1")
                            responseFromURL = "1";
                        else
                            responseFromURL = "0";

                        //string responseFromURL = "1";

                        string query1 = "insert into DrSuccess(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                            + ",'" + messageId + "','delivrd','" + ShortCode + "','" + fulldate + "','" + fulltime + "','" + tdatetime + "','" + responseFromURL + "')";

                        SqlConnection cn1 = new SqlConnection(cs);
                        cn1.Open();

                        SqlCommand cmd1 = new SqlCommand(query1, cn1);
                        int j = cmd1.ExecuteNonQuery();
                        if (j == 1)
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + " & Inserted");

                        }
                        else
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + "& notInserted");

                        }
                        cn1.Close();

                        string FILENAME = Path.GetFileName(file);

                        string DUMPLOC = @"D:\iGWDrDump\" + ShortCode + @"\";

                        string CURRDATE = DateTime.Now.ToString("dd-MM-yyyy") + @"\";

                        string CHKDIR = DUMPLOC + CURRDATE;

                        //string DESTFILE = DUMPLOC + FILENAME;

                        string DESTFILE = DUMPLOC + CURRDATE + FILENAME;

                        if (Directory.Exists(CHKDIR))
                        {
                            File.Move(file, DESTFILE); // @"D:\msg\TEST\DUMP\" + FILENAME);
                                                       //Console.ReadLine();
                        }
                        else
                        {
                            Directory.CreateDirectory(CHKDIR);
                            File.Move(file, DESTFILE);
                        }
                    }
                    else if (scodelist4.Contains(ShortCode))
                    {

                        WebRequest request = WebRequest.Create("http://service.esaltel.com:8888/SMSInterface/InsertDR?" +
                        "&shortCode=" + ShortCode
                        + "&msisdn=" + MSISDN
                        + "&DRStatus=deliv"
                        + "&DRID=" + messageId);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse response = request.GetResponse();
                        //Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription);
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromURL = reader.ReadToEnd();
                        Console.WriteLine(responseFromURL);
                        reader.Close();
                        response.Close();

                        if (responseFromURL == "0")// | responseFromURL == "1")
                            responseFromURL = "1";
                        else
                            responseFromURL = "0"; //they send -1 for Invalid

                        //string responseFromURL = "0";

                        string query3 = "insert into DrSuccess(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                            + ",'" + messageId + "','delivrd','" + ShortCode + "','" + fulldate + "','" + fulltime + "','" + tdatetime + "','" + responseFromURL + "')";
                        //string query2 = "insert into DrSuccess(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                        //       + ",'" + messageId + "','delivrd','" + ShortCode + "','" + fulldate + "','" +
                        //       fulltime + "','" + tdatetime + "','" + responseFromURL + "')";

                        SqlConnection cn3 = new SqlConnection(cs);
                        cn3.Open();

                        SqlCommand cmd3 = new SqlCommand(query3, cn3);
                        int i = cmd3.ExecuteNonQuery();
                        if (i == 1)
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted ");// + ((HttpWebResponse)response).StatusDescription + " & Inserted");

                        }
                        else
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted ");// + ((HttpWebResponse)response).StatusDescription + "& notInserted");
                        }
                        cn3.Close();

                        string FILENAME = Path.GetFileName(file);

                        string DUMPLOC = @"D:\iGWDrDump\" + ShortCode + @"\";

                        string CURRDATE = DateTime.Now.ToString("dd-MM-yyyy") + @"\";

                        string CHKDIR = DUMPLOC + CURRDATE;

                        //string DESTFILE = DUMPLOC + FILENAME;

                        string DESTFILE = DUMPLOC + CURRDATE + FILENAME;

                        if (Directory.Exists(CHKDIR))
                        {
                            File.Move(file, DESTFILE); // @"D:\msg\TEST\DUMP\" + FILENAME);
                                                       //Console.ReadLine();
                        }
                        else
                        {
                            Directory.CreateDirectory(CHKDIR);
                            File.Move(file, DESTFILE);
                        }
                    }

                    else
                    {
                        Int32 unixTimestamp = (Int32)(TimeZoneInfo.ConvertTimeToUtc(dated, TimeZoneInfo.Local).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        log4net.Config.XmlConfigurator.Configure();
                        //————————–
                        //log4net.Config.BasicConfigurator.Configure();
                        log4net.ILog log = log4net.LogManager.GetLogger("CustomLoggerName");//(Program);
                        //log.Debug("");
                        log.Info(file + " XML Date " + dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                                                          + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);

                        //        _readWriteLock.EnterWriteLock();
                        //        try
                        //        {
                        //            string CURRDATElogfile = DateTime.Now.ToString("dd-MM-yyyy HH:MM:SS");

                        //            if (File.Exists(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //            {
                        //                FileInfo fi = new FileInfo(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                if (fi.Length > (1024 * 1024 * 20))
                        //                {
                        //                    Random rndm = new Random();
                        //                    File.Create(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt");
                        //                    using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt"))
                        //                    {
                        //                        //Thread.Sleep(1000);
                        //                        sw.WriteLine(dated+" URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                        sw.Close();
                        //                    }
                        //                    //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //                    //FileShare.Write, 4096, FileOptions.None))
                        //                    //                {
                        //                    //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    //                    {
                        //                    //                        writer.AutoFlush = true;
                        //                    //                        for (int i = 0; i < 20; ++i)
                        //                    //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                    //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    //                    }
                        //                    //                }
                        //                }
                        //                else
                        //                {
                        //                    using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //                    {
                        //                        //Thread.Sleep(1000);
                        //                        sw.WriteLine(dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                        sw.Close();
                        //                    }
                        //                }
                        //            }
                        //            //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //            //FileShare.Write, 4096, FileOptions.None))
                        //            //                {
                        //            //                    using (StreamWriter writer = new StreamWriter(fs))
                        //            //                    {
                        //            //                        writer.AutoFlush = true;
                        //            //                        for (int i = 0; i < 20; ++i)
                        //            //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //            //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //            //                    }
                        //            //                }
                        //            //                //using (StreamWriter sw = new StreamWriter(@"D:\TEST\" + CURRDATElogfile + ".txt", true))
                        //            //                //{
                        //            //                //    sw.WriteLine("Unixv::::::" + "unixTimestamp ****" + "http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //            //                //       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //            //                //}
                        //            //                //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //            //                //FileShare.Write, 4096, FileOptions.None))
                        //            //                //                {
                        //            //                //                    using (StreamWriter writer = new StreamWriter(fs))
                        //            //                //                    {
                        //            //                //                        writer.AutoFlush = true;
                        //            //                //                        for (int i = 0; i < 20; ++i)
                        //            //                //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //            //                //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //            //                //                    }
                        //            //                //                }
                        //            //            }
                        //            else
                        //            {
                        //                File.Create(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //FileShare.Write, 4096, FileOptions.None))
                        //                {
                        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    {
                        //                        writer.AutoFlush = true;
                        //                        for (int i = 0; i < 20; ++i)
                        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    }
                        //                }
                        //            }

                        //                // Append text to the file
                        //            //    using (StreamWriter sw = File.AppendText(path))
                        //            //{
                        //            //    //Thread.Sleep(1000);
                        //            //    sw.WriteLine(text);
                        //            //    sw.Close();
                        //            //}
                        //        }
                        //        finally
                        //        {
                        //            // Release lock
                        //            _readWriteLock.ExitWriteLock();
                        //        }
                        //=======================================================
                        //            string CURRDATElogfile = DateTime.Now.ToString("dd-MM-yyyy") ;

                        //            if (File.Exists(@"D:\TEST\" + CURRDATElogfile + ".txt") )
                        //            {
                        //                FileInfo fi = new FileInfo(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                if(fi.Length>(1024*1024*10))
                        //                {
                        //                    Random rndm = new Random();
                        //                    File.Create(@"D:\TEST\" + CURRDATElogfile +"-"+ rndm.Next()+".txt");
                        //                    using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //    FileShare.Write, 4096, FileOptions.None))
                        //                    {
                        //                        using (StreamWriter writer = new StreamWriter(fs))
                        //                        {
                        //                            writer.AutoFlush = true;
                        //                            for (int i = 0; i < 20; ++i)
                        //                                writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                        }
                        //                    }
                        //                }
                        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //FileShare.Write, 4096, FileOptions.None))
                        //                {
                        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    {
                        //                        writer.AutoFlush = true;
                        //                        for (int i = 0; i < 20; ++i)
                        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    }
                        //                }
                        //                //using (StreamWriter sw = new StreamWriter(@"D:\TEST\" + CURRDATElogfile + ".txt", true))
                        //                //{
                        //                //    sw.WriteLine("Unixv::::::" + "unixTimestamp ****" + "http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                //       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                //}
                        //                //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //                //FileShare.Write, 4096, FileOptions.None))
                        //                //                {
                        //                //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                //                    {
                        //                //                        writer.AutoFlush = true;
                        //                //                        for (int i = 0; i < 20; ++i)
                        //                //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                //                    }
                        //                //                }
                        //            }
                        //            else
                        //            {
                        //                File.Create(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //FileShare.Write, 4096, FileOptions.None))
                        //                {
                        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    {
                        //                        writer.AutoFlush = true;
                        //                        for (int i = 0; i < 20; ++i)
                        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    }
                        //                }
                        //            }

                        Console.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        WebRequest request = WebRequest.Create("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=delivrd&shortCode="
                            + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);

                        request.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse response = request.GetResponse();
                        //Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription);
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromURL = reader.ReadToEnd();
                        Console.WriteLine(responseFromURL);
                        reader.Close();
                        response.Close();

                        if (responseFromURL == "OK" | responseFromURL == "1")
                            responseFromURL = "1";
                        else
                            responseFromURL = "0";

                        //string responseFromURL = "1";

                        string query2 = "insert into DrSuccess(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                                                       + ",'" + messageId + "','delivrd','" + ShortCode + "','" + fulldate + "','" +
                                                       fulltime + "','" + tdatetime + "','" + responseFromURL + "')";

                        SqlConnection cn2 = new SqlConnection(cs);
                        cn2.Open();

                        SqlCommand cmd2 = new SqlCommand(query2, cn2);
                        int l = cmd2.ExecuteNonQuery();
                        if (l == 1)
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + " & Inserted");

                        }
                        else
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + "& notInserted");
                        }
                        cn2.Close();

                        string FILENAME = Path.GetFileName(file);

                        string DUMPLOC = @"D:\iGWDrDump\" + ShortCode + @"\";
                        string CURRDATE = DateTime.Now.ToString("dd-MM-yyyy") + @"\";

                        string CHKDIR = DUMPLOC + CURRDATE;

                        //string DESTFILE = DUMPLOC + FILENAME;

                        string DESTFILE = DUMPLOC + CURRDATE + FILENAME;

                        if (Directory.Exists(CHKDIR))
                        {
                            File.Move(file, DESTFILE); // @"D:\msg\TEST\DUMP\" + FILENAME);
                                                       //Console.ReadLine();
                        }
                        else
                        {
                            Directory.CreateDirectory(CHKDIR);
                            File.Move(file, DESTFILE);
                        }
                    }


                }
                else //actual not 3
                {
                    if (scodelist2.Contains(ShortCode))
                    {
                        //Console.WriteLine("http://spgw.atlas-me.com/PaymentProviders/Iraq/ZainDeliveryNotification.aspx?&shortCode=");
                        WebRequest request = WebRequest.Create("http://spgw.atlas-me.com/PaymentProviders/Iraq/ZainDeliveryNotification.aspx?&shortCode="
                            + ShortCode + "&msisdn=" + MSISDN + "&DRStatus=undeliv&DRID=" + messageId);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse response = request.GetResponse();
                        //Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription);
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromURL = reader.ReadToEnd();
                        Console.WriteLine(responseFromURL);
                        reader.Close();
                        response.Close();

                        if (responseFromURL == "OK" | responseFromURL == "1")
                            responseFromURL = "1";
                        else
                            responseFromURL = "0";

                        //string responseFromURL = "1";

                        string query3 = "insert into DrFailed(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                            + ",'" + messageId + "','undeliv','" + ShortCode + "','" + fulldate + "','" + fulltime + "','" + tdatetime + "','" + responseFromURL + "')";

                        SqlConnection cn3 = new SqlConnection(cs);
                        cn3.Open();

                        SqlCommand cmd3 = new SqlCommand(query3, cn3);
                        int i = cmd3.ExecuteNonQuery();
                        if (i == 1)
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + " & Inserted");

                        }
                        else
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + "& notInserted");
                        }
                        cn3.Close();

                        string FILENAME = Path.GetFileName(file);

                        string DUMPLOC = @"D:\iGWDrDump\" + ShortCode + @"\";

                        string CURRDATE = DateTime.Now.ToString("dd-MM-yyyy") + @"\";

                        string CHKDIR = DUMPLOC + CURRDATE;

                        //string DESTFILE = DUMPLOC + FILENAME;

                        string DESTFILE = DUMPLOC + CURRDATE + FILENAME;

                        if (Directory.Exists(CHKDIR))
                        {
                            File.Move(file, DESTFILE); // @"D:\msg\TEST\DUMP\" + FILENAME);
                                                       //Console.ReadLine();
                        }
                        else
                        {
                            Directory.CreateDirectory(CHKDIR);
                            File.Move(file, DESTFILE);
                        }
                    }

                    else if (scodelist4.Contains(ShortCode))
                    {

                        WebRequest request = WebRequest.Create("http://service.esaltel.com:8888/SMSInterface/InsertDR?" +
                        "&shortCode=" + ShortCode
                        + "&msisdn=" + MSISDN
                        + "&DRStatus=undeliv"
                        + "&DRID=" + messageId);
                        request.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse response = request.GetResponse();
                        //Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription);
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromURL = reader.ReadToEnd();
                        Console.WriteLine(responseFromURL);
                        reader.Close();
                        response.Close();

                        if (responseFromURL == "0" )//| responseFromURL == "1")
                            responseFromURL = "1";
                        else
                            responseFromURL = "0";

                        //string responseFromURL = "0";

                        string query3 = "insert into DrFailed(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                            + ",'" + messageId + "','undeliv','" + ShortCode + "','" + fulldate + "','" + fulltime + "','" + tdatetime + "','" + responseFromURL + "')";

                        SqlConnection cn3 = new SqlConnection(cs);
                        cn3.Open();

                        SqlCommand cmd3 = new SqlCommand(query3, cn3);
                        int i = cmd3.ExecuteNonQuery();
                        if (i == 1)
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted ");// + ((HttpWebResponse)response).StatusDescription + " & Inserted");

                        }
                        else
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted ");// + ((HttpWebResponse)response).StatusDescription + "& notInserted");
                        }
                        cn3.Close();

                        string FILENAME = Path.GetFileName(file);

                        string DUMPLOC = @"D:\iGWDrDump\" + ShortCode + @"\";
                        string CURRDATE = DateTime.Now.ToString("dd-MM-yyyy") + @"\";

                        string CHKDIR = DUMPLOC + CURRDATE;

                        //string DESTFILE = DUMPLOC + FILENAME;

                        string DESTFILE = DUMPLOC + CURRDATE + FILENAME;

                        if (Directory.Exists(CHKDIR))
                        {
                            File.Move(file, DESTFILE); // @"D:\msg\TEST\DUMP\" + FILENAME);
                                                       //Console.ReadLine();
                        }
                        else
                        {
                            Directory.CreateDirectory(CHKDIR);
                            File.Move(file, DESTFILE);
                        }
                    }
                    else
                    {

                        Int32 unixTimestamp = (Int32)(TimeZoneInfo.ConvertTimeToUtc(dated, TimeZoneInfo.Local).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        log4net.Config.XmlConfigurator.Configure();
                        //————————–
                        //log4net.Config.BasicConfigurator.Configure();
                        log4net.ILog log = log4net.LogManager.GetLogger("CustomLoggerName");//(Program);
                        //log.Debug("");
                        log.Info(file + " XML Date " + dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                                                          + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);


                        //        _readWriteLock.EnterWriteLock();
                        //        try
                        //        {
                        //            string CURRDATElogfile = DateTime.Now.ToString("dd-MM-yyyy HH:MM:SS");

                        //            if (File.Exists(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //            {
                        //                FileInfo fi = new FileInfo(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                if (fi.Length > (1024 * 1024 * 20))
                        //                {
                        //                    Random rndm = new Random();
                        //                    File.Create(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt");
                        //                    using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt"))
                        //                    {
                        //                        //Thread.Sleep(1000);
                        //                        sw.WriteLine(dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                        sw.Close();
                        //                    }
                        //                    //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //                    //FileShare.Write, 4096, FileOptions.None))
                        //                    //                {
                        //                    //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    //                    {
                        //                    //                        writer.AutoFlush = true;
                        //                    //                        for (int i = 0; i < 20; ++i)
                        //                    //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                    //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    //                    }
                        //                    //                }
                        //                }
                        //                else
                        //                {
                        //                    using (StreamWriter sw = File.AppendText(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //                    {
                        //                        //Thread.Sleep(1000);
                        //                        sw.WriteLine(dated + " URL= http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                        sw.Close();
                        //                    }
                        //                }
                        //            }
                        //            //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //            //FileShare.Write, 4096, FileOptions.None))
                        //            //                {
                        //            //                    using (StreamWriter writer = new StreamWriter(fs))
                        //            //                    {
                        //            //                        writer.AutoFlush = true;
                        //            //                        for (int i = 0; i < 20; ++i)
                        //            //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //            //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //            //                    }
                        //            //                }
                        //            //                //using (StreamWriter sw = new StreamWriter(@"D:\TEST\" + CURRDATElogfile + ".txt", true))
                        //            //                //{
                        //            //                //    sw.WriteLine("Unixv::::::" + "unixTimestamp ****" + "http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //            //                //       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //            //                //}
                        //            //                //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //            //                //FileShare.Write, 4096, FileOptions.None))
                        //            //                //                {
                        //            //                //                    using (StreamWriter writer = new StreamWriter(fs))
                        //            //                //                    {
                        //            //                //                        writer.AutoFlush = true;
                        //            //                //                        for (int i = 0; i < 20; ++i)
                        //            //                //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //            //                //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //            //                //                    }
                        //            //                //                }
                        //            //            }
                        //            else
                        //            {
                        //                File.Create(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //FileShare.Write, 4096, FileOptions.None))
                        //                {
                        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    {
                        //                        writer.AutoFlush = true;
                        //                        for (int i = 0; i < 20; ++i)
                        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    }
                        //                }
                        //            }

                        //            // Append text to the file
                        //            //    using (StreamWriter sw = File.AppendText(path))
                        //            //{
                        //            //    //Thread.Sleep(1000);
                        //            //    sw.WriteLine(text);
                        //            //    sw.Close();
                        //            //}
                        //        }
                        //        finally
                        //        {
                        //            // Release lock
                        //            _readWriteLock.ExitWriteLock();
                        //        }
                        //=======================================================================            
                        //            string CURRDATElogfile = DateTime.Now.ToString("dd-MM-yyyy");

                        //            if (File.Exists(@"D:\TEST\" + CURRDATElogfile + ".txt"))
                        //            {
                        //                FileInfo fi = new FileInfo(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                if (fi.Length > (1024 * 1024 * 10))
                        //                {
                        //                    Random rndm = new Random();
                        //                    File.Create(@"D:\TEST\" + CURRDATElogfile + "-" + rndm.Next() + ".txt");
                        //                    using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //    FileShare.Write, 4096, FileOptions.None))
                        //                    {
                        //                        using (StreamWriter writer = new StreamWriter(fs))
                        //                        {
                        //                            writer.AutoFlush = true;
                        //                            for (int i = 0; i < 20; ++i)
                        //                                writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                           + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                        }
                        //                    }
                        //                }
                        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //   FileShare.Write, 4096, FileOptions.None))
                        //                {
                        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    {
                        //                        writer.AutoFlush = true;
                        //                        for (int i = 0; i < 20; ++i)
                        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    }
                        //                }
                        //                //using (StreamWriter sw = new StreamWriter(@"D:\TEST\" + CURRDATElogfile + ".txt", true))
                        //                //{
                        //                //    sw.WriteLine("Unixv::::::" + "unixTimestamp ****" + "http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                //       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                //}
                        //                //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //                //FileShare.Write, 4096, FileOptions.None))
                        //                //                {
                        //                //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                //                    {
                        //                //                        writer.AutoFlush = true;
                        //                //                        for (int i = 0; i < 20; ++i)
                        //                //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                //                    }
                        //                //                }
                        //            }
                        //            else
                        //            {
                        //                File.Create(@"D:\TEST\" + CURRDATElogfile + ".txt");
                        //                using (FileStream fs = new FileStream(@"D:\TEST\" + CURRDATElogfile + ".txt", FileMode.Open, System.Security.AccessControl.FileSystemRights.AppendData,
                        //FileShare.Write, 4096, FileOptions.None))
                        //                {
                        //                    using (StreamWriter writer = new StreamWriter(fs))
                        //                    {
                        //                        writer.AutoFlush = true;
                        //                        for (int i = 0; i < 20; ++i)
                        //                            writer.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                        //                       + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        //                    }
                        //                }
                        //            }
                        Console.WriteLine("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                            + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);
                        WebRequest request = WebRequest.Create("http://vas.beecell.com/index.php?r=api/DLRTransaction&reply=undeliv&shortCode="
                            + ShortCode + "&opCode=10&msisdn=" + MSISDN + "&datetime=" + unixTimestamp);

                        request.Credentials = CredentialCache.DefaultCredentials;
                        WebResponse response = request.GetResponse();
                        //Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription);
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromURL = reader.ReadToEnd();
                        Console.WriteLine(responseFromURL);
                        reader.Close();
                        response.Close();

                        if (responseFromURL == "OK" | responseFromURL == "1")
                            responseFromURL = "1";
                        else
                            responseFromURL = "0";

                        //string responseFromURL = "1";

                        string query4 = "insert into DrFailed(sender,drid,drstatus,scode,drdates,drtimes,trandate,flag) values(" + MSISDN
                                                       + ",'" + messageId + "','undeliv','" + ShortCode + "','" + fulldate + "','" +
                                                       fulltime + "','" + tdatetime + "','" + responseFromURL + "')";

                        SqlConnection cn4 = new SqlConnection(cs);
                        cn4.Open();

                        SqlCommand cmd4 = new SqlCommand(query4, cn4);
                        int k = cmd4.ExecuteNonQuery();
                        if (k == 1)
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + " & Inserted");

                        }
                        else
                        {
                            Console.WriteLine(ShortCode + " : " + Path.GetFileName(file) + " Posted " + ((HttpWebResponse)response).StatusDescription + "& notInserted");
                        }
                        cn4.Close();

                        string FILENAME = Path.GetFileName(file);

                        string DUMPLOC = @"D:\iGWDrDump\" + ShortCode + @"\";

                        string CURRDATE = DateTime.Now.ToString("dd-MM-yyyy") + @"\";

                        string CHKDIR = DUMPLOC + CURRDATE;

                        //string DESTFILE = DUMPLOC + FILENAME;

                        string DESTFILE = DUMPLOC + CURRDATE + FILENAME;

                        if (Directory.Exists(CHKDIR))
                        {
                            File.Move(file, DESTFILE); // @"D:\msg\TEST\DUMP\" + FILENAME);
                                                       //Console.ReadLine();
                        }
                        else
                        {
                            Directory.CreateDirectory(CHKDIR);
                            File.Move(file, DESTFILE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception ", ex);// ex.Data.ToString() + " ** " + ex.Message.ToString() + " ** ");// + ex.InnerException.ToString() + " ** " + ex.GetBaseException().ToString(), ex);
                Console.WriteLine(ex.Data.ToString() + " ** " + ex.Message.ToString() + " ** ");// +ex.TargetSite+" * "+ex.StackTrace);// + ex.InnerException.ToString());// + " ** " +ex.GetBaseException().ToString());
                //Console.ReadLine();
            }
        }
    }

}
