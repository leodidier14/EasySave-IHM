using System;
using IHM.ViewModelNameSpace;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using IHM.ObserverNameSpace;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IHM.ModelNameSpace
{
    class Client : IObservable
    {
        private const int port = 1337;
        private bool connection;

        private List<IObserver> _observers = new List<IObserver>();

        //notify the connection state to the thread
        private ManualResetEvent connectDone = new ManualResetEvent(false);

        //notify the receive state to the thread
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        private Socket socket;

        private ViewModel viewModel;

        public Client(ViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        //connect to the server
        public void startClient(string ip)
        {
            try
            {
                //establish the remote endpoint for the socket
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                //create a TCP/IP socket  
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //connect to the remote endpoint 
                this.socket.BeginConnect(remoteEP, new AsyncCallback(connectCallback), this.socket);
                this.connectDone.WaitOne();
                this.connection = true;

                //while the client is connected to the server, receive the data
                while (this.connection)
                {
                    this.receiveDone.Reset();
                    this.receive(this.socket);
                    this.receiveDone.WaitOne();
                }

            }
            catch (Exception)
            {

            }

        }

        //manage the connection
        public void connectCallback(IAsyncResult ar)
        {
            try
            {
                //retrieve the socket from server connection object
                Socket client = (Socket)ar.AsyncState;

                //complete the connection
                client.EndConnect(ar);

                //signal that the connection has been made.  
                this.connectDone.Set();

                //square color in interface green
                viewModel.IsConnected = true;

            }
            catch (SocketException)
            {

            }
        }

        //receive method
        public void receive(Socket socket)
        {
            try
            {
                //create the server connection object
                ServerConnection serverConnection = new ServerConnection();
                serverConnection.workSocket = socket;

                //begin receiving the data from the remote device.  
                socket.BeginReceive(serverConnection.buffer, 0, ServerConnection.BufferSize, 0, new AsyncCallback(receiveCallback), serverConnection);
            }
            catch (Exception)
            {

            }
        }

        ////continue receiving asynchronously 
        public void receiveCallback(IAsyncResult ar)
        {
            try
            {
                string data = "";

                //retrieve the server connection object and the client socket from the asynchronous server connection object
                ServerConnection serverConnection = (ServerConnection)ar.AsyncState;
                Socket client = serverConnection.workSocket;

                //read data from the remote device 
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    //store the data received 
                    serverConnection.sb.Append(Encoding.ASCII.GetString(serverConnection.buffer, 0, bytesRead));

                    //check for end-of-file tag. If it is not there, read more data -> information backup list
                    data = serverConnection.sb.ToString();
                    if (data.IndexOf("<ALL>") > -1)
                    {
                        //all the data has been read from the client

                        //remove the end-of-file tag
                        if (data != null && data.Length != 0)
                        {
                            data = data.Substring(0, data.Length - 5);
                        }

                        //deserialize the backup list to send it to view model (notify)
                        var response = JsonConvert.DeserializeObject<List<Backup>>(data);
                        this.notify(response);
                    }

                    //check for end-of-file tag. If it is not there, read more data -> information progress
                    else if (data.IndexOf("<ONE>") > -1)
                    {
                        //all the data has been read from the client

                        //remove the end-of-file tag
                        if (data != null && data.Length != 0)
                        {
                            data = data.Substring(0, data.Length - 5);
                        }

                        //deserialize the progress of one backup to send it to view model (notify)
                        var response = JsonConvert.DeserializeObject<Backup>(data);
                        notify(response);
                    }

                    //if the end-of-file tag is not there, read more data
                    else
                    {
                        client.BeginReceive(serverConnection.buffer, 0, ServerConnection.BufferSize, 0, new AsyncCallback(receiveCallback), serverConnection);
                    }
                }

                //signal the thread that the receive method is finished
                this.receiveDone.Set();
            }

            //if this exception is catch = client is deconnected
            catch (SocketException)
            {
                this.viewModel.disconnect();
            }

            //if the client tries to receive data after the socket has been stopped
            catch (ObjectDisposedException)
            {

            }
        }

        //when client is deconnected 
        public void disconnect()
        {
            //square color in interface red
            this.viewModel.IsConnected = false;
            
            //set the boolean in false for the starting Cliend method stop the receive while true
            this.connection = false;
            
            //stop the socket
            this.socket.Shutdown(SocketShutdown.Both);
            this.socket.Close();

            //signal the thread that the receive method is finished for not stopping the execution.
            this.receiveDone.Set();

        }

        public void attach(IObserver observer)
        {
            this._observers.Add(observer);
        }

        public void detach(IObserver observer)
        {
            this._observers.Remove(observer);
        }

        //notify the view model that the backup list is updated
        public void notify(List<Backup> allBackup)
        {
            foreach (var observer in _observers)
            {
                observer.update(this, allBackup);
            }
        }

        //notify the view model that the progress of one backup is updated
        public void notify(Backup oneBackup)
        {
            foreach (var observer in _observers)
            {
                observer.update(this, oneBackup);
            }
        }
    }
}




