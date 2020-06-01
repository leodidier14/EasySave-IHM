using System.Net.Sockets;
using System.Text;

namespace IHM.ModelNameSpace
{
    class ServerConnection
    {
        //client socket
        public Socket workSocket = null;
        //size of receive buffer  
        public const int BufferSize = 256;
        //receive buffer
        public byte[] buffer = new byte[BufferSize];
        //received data string  
        public StringBuilder sb = new StringBuilder();
    }
}
