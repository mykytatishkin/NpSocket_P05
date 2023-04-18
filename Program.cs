using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NpSocket_P05
{
    class Program {
        static void Main(string[] args)
        {
            /* CONNECTION
            var ip = IPAddress.Parse("0.0.0.0");
            var endPoint = new IPEndPoint(ip, 80);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);


            try
            {
                socket.Connect(endPoint);
                if(socket.Connected)
                {
                    var strSend = "GET\r\n\r\n";
                    socket.Send(Encoding.UTF8.GetBytes(strSend));
                    var buffer = new byte[1024];
                    int length= 0;
                    do
                    {
                        length = socket.Receive(buffer);
                        Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, length));
                    } while (length > 0);
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            */

            // Server Socket
            /*
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            var ip = IPAddress.Parse("127.0.0.1");
            var endPoint = new IPEndPoint(ip, 1234);
            socket.Bind(endPoint);
            socket.Listen(10);
            try
            {
                while(true)
                {
                    var newSocket = socket.Accept();
                    Console.WriteLine(newSocket.RemoteEndPoint?.ToString());
                    newSocket?.Send(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
                    newSocket?.Shutdown(SocketShutdown.Both);
                    newSocket?.Close();
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Close();
            }*/
            var server = new AsyncServerSocket("127.0.0.1", 1234);
            server.StartServer();
            Console.ReadKey();
        }

        

        class AsyncServerSocket
        {
            IPEndPoint endPoint;
            Socket? socket;
            public AsyncServerSocket(string? addr, int port)
            {
                endPoint = new IPEndPoint(IPAddress.Parse(addr), port);
            }
            void MyAcceptCallbackFunction(IAsyncResult res)
            {
                var socket = res.AsyncState as Socket;
                var newSocket = socket?.EndAccept(res);
                Console.WriteLine(newSocket?.RemoteEndPoint?.ToString());
                var buffer = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                newSocket?.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, MySendCallbackFunction, newSocket);
                socket?.BeginAccept(MyAcceptCallbackFunction, socket);
            }
            void MySendCallbackFunction(IAsyncResult res)
            {
                var newSocket = res.AsyncState as Socket;
                newSocket?.EndSend(res);
                newSocket?.Shutdown(SocketShutdown.Both);
                newSocket?.Close();
            }
            public void StartServer()
            {
                if (socket != null)
                {
                    return;
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                socket.Bind(endPoint);
                socket.Listen(10);
                socket.BeginAccept(MyAcceptCallbackFunction, socket);
            }
        }
    }
}