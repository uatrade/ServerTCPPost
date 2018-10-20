
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Server
{
    class Program
    {
        static int port = 4001;

        static List<string> ReadXML(string findIndex)
        {
            List<string> streets = new List<string>();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(@"D:\Академия Шаг\Сетевое Программирование\ДЗ№1\DZ1\ind.xml");
                xmlDoc.Load(@"ind.xml");
                XmlElement xmlRoot = xmlDoc.DocumentElement;


                foreach(XmlNode xnode in xmlRoot)
                {
                    // получаем атрибут index
                    if (xnode.Attributes.Count > 0)
                    {
                        XmlNode attr = xnode.Attributes.GetNamedItem("index");
                        if (attr != null)

                            if(findIndex== attr.Value)
                            {
                                foreach (XmlNode xchild in xnode.ChildNodes)
                                {
                                    if(xchild.Name=="street")
                                    {
                                        //Console.WriteLine(xchild.InnerText);
                                        streets.Add(xchild.InnerText);
                                    }
                                }
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            return streets;
        }
        static void Main(string[] args)
        {

            //IPEndPoint IpEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.105"), port);
            IPEndPoint IpEndPoint = new IPEndPoint(IPAddress.Any, port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(IpEndPoint);
                listenSocket.Listen(15);
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {

                    Socket handler = listenSocket.Accept();
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[1024];

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        if(bytes!=0)
                            Console.WriteLine("Получение данных");

                    } while (handler.Available>0);

                    List<string> mystreets = new List<string>();

                      mystreets=ReadXML(builder.ToString());   //Получаем список

                    // List<string> mystreets = ReadXML("50001");   //Получаем список


                    foreach (var item in mystreets)
                    {
                        data = Encoding.Unicode.GetBytes(item);
                        handler.Send(data);
                        data = new byte[1024];
                    }
                    // закрываем сокет

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }

        }
    }
}
