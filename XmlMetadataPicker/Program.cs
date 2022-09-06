using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;

namespace XmlMetadataPicker
{
    internal class Program
    {

        public static List<FileInfo> XmlFileList = new List<FileInfo>();

        public static XmlDocument xml;

        static DirectoryInfo dir;
        

        static void Main(string[] args)
        {

            Console.WriteLine("Author:HaoJun0823:https://blog.haojun0823.xyz/");


            if (args.Length != 0 && Directory.Exists(args[0]))
            {

                dir = new DirectoryInfo(args[0]);

                Console.WriteLine("Working for " + dir.FullName);
                SearchDirectory(0,dir,"map.xml");
                GetMapMetaData();

            }
            else
            {

                Console.Error.WriteLine("You need drag a directory into this file which that directory contain some map.xml files.");
                
            }

            Console.WriteLine("Done!");

            //Console.ReadKey();
            //Console.WriteLine("Will closed after 3s!");
            //Thread.Sleep(3000);

        }



        static void GetMapMetaData()
        {
            xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0","utf-8",null));
            XmlElement dec = xml.CreateElement("AssetDeclaration");
            dec.SetAttribute("xmlns", "uri:ea.com:eala:asset");
            xml.AppendChild(dec);

            int count = 1;

            int invaild = 1;

            foreach(FileInfo file in XmlFileList)
            {


                Console.WriteLine("Try to Get MapMetaData From " + file.FullName);

                XmlDocument mapXml = new XmlDocument();
                mapXml.Load(file.FullName);

                XmlNodeList xnl =  mapXml.GetElementsByTagName("MapMetaData");

                if (xnl.Count != 0)
                {


                    dec.AppendChild(xml.CreateComment("File " + count + "," + file.FullName));
                    dec.AppendChild(xml.ImportNode(xnl.Item(0),true));

                    Console.WriteLine("Add MapMetaData.");
                    count++;

                }
                else
                {

                    dec.AppendChild(xml.CreateComment("Invalid File " + invaild + "," + file.FullName));

                    Console.WriteLine("Nothing, Skip!");
                    invaild++;
                }




            }

            dec.PrependChild(xml.CreateComment("Invalid Files: " + (XmlFileList.Count+1 - count)));
            dec.PrependChild(xml.CreateComment("Valid Files: " + count));
            dec.PrependChild(xml.CreateComment("Total Files: " + (XmlFileList.Count+1)));
            dec.PrependChild(xml.CreateComment("Target Directory: " + dir.FullName));
            dec.PrependChild(xml.CreateComment(DateTime.Now.ToString()));
            dec.PrependChild(xml.CreateComment("https://blog.haojun0823.xyz/"));
            dec.PrependChild(xml.CreateComment("HaoJun0823 SageGameMapMetadataXmlPicker"));









            xml.Save(System.AppDomain.CurrentDomain.BaseDirectory+"MapMetaData.xml");

        }


        public static void SearchDirectory(int depth, DirectoryInfo directory, string extname)
        {
            
            Console.WriteLine(depth + "D-" + directory.Name);
            Console.WriteLine("Search " + directory.FullName + " depth: " + depth + " extname:" + extname);
            DirectoryInfo[] SubDirectory = directory.GetDirectories();
            FileInfo[] Files = directory.GetFiles(extname);


            if (SubDirectory.Length > 0)
            {
                Console.WriteLine("Search subdirectory in " + directory.FullName);
                foreach (DirectoryInfo d in SubDirectory)
                {
                    Console.WriteLine("Get subdirectory, jump in:" + d.FullName);
                    SearchDirectory(depth + 1, d, extname);
                }
            }
            else
            {
                Console.WriteLine("No directory in " + directory.FullName);
            }

            if (Files.Length > 0)
            {
                Console.WriteLine(directory.FullName + " " + extname + " file number count:" + Files.Length);
                foreach (FileInfo f in Files)
                {

                    Console.WriteLine("Add " + f.FullName + " into the list.");
                    Console.WriteLine(depth + "  F-" + f.Name);
                    XmlFileList.Add(f);
                }
            }
            else
            {
                Console.WriteLine(directory.FullName + " doesn't have " + extname + " file");
            }
        }

    }
}
