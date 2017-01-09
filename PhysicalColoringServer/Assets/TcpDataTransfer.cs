//using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TcpDataTransfer : MonoBehaviour
{
    public RawImage Image;
    public Text ObjectID;
    public Text PlayerID;

    public string ServerIP = "127.0.0.1";
    public int port = 3003;

    TcpListener listen;

    bool bRecievedData;
    bool bKeepRecieving;

    byte[] Data;

    public event Action OnDataComepletelySent;
    public event Action<byte[]> OnDataComepletelyRecieved;

    public static TcpDataTransfer singlton;
    [Serializable]
    public class ObjectToTransfer
    {
        public int PlayerID;
        public int ObjectID;
        public byte[] TextureRawData;
        public int Width;
        public int Height;
        public TextureFormat Format;
        public bool mipmap;
        public ObjectToTransfer(int _PlayerID, int _ObjectID, Texture2D ScannedTexture)
        {
            PlayerID = _PlayerID;
            ObjectID = _ObjectID;
            TextureRawData = ScannedTexture.GetRawTextureData();
            Width = ScannedTexture.width;
            Height = ScannedTexture.height;
            Format = ScannedTexture.format;
            mipmap = ScannedTexture.mipmapCount > 1;
        }

        public Texture2D GetTexture()
        {
            Texture2D OutputTexture = new Texture2D(Width, Height, Format, mipmap);
            OutputTexture.LoadRawTextureData(TextureRawData);
            OutputTexture.Apply();
            return OutputTexture;
        }
    }


    private void Awake()
    {
        if (!singlton)
            singlton = this;
        else
            Debug.LogError("TcpDataTransfer has multiple instances in your scene");
    }

    void Start()
    {
        InitiateListener();
        print("Shit");
    }

    private void Update()
    {
        if(IsDataRecieved())
        {
            ObjectToTransfer obj = ReadObjectAndClear();
            Image.texture = obj.GetTexture();
            ObjectID.text = obj.ObjectID + "";
            PlayerID.text = obj.PlayerID + "";
        }
    }


    public void SendObject(ObjectToTransfer Obj)
    {
        SendData(ObjectToByteArray(Obj));
    }

    void SendData(byte[] _Data)
    {
            Data = _Data;
            Send();
    }

    void Send()
    {
        IPAddress ipAddress = IPAddress.Parse(ServerIP);
        int bufferSize = 1024;

        TcpClient client = new TcpClient();
        NetworkStream netStream;

        // Connect to server
        try
        {
            client.Connect(new IPEndPoint(ipAddress, port));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        netStream = client.GetStream();
        // Read bytes from image

        // Build the package
        byte[] dataLength = BitConverter.GetBytes(Data.Length);
        byte[] package = new byte[4 + Data.Length];
        dataLength.CopyTo(package, 0);
        Data.CopyTo(package, 4);

        // Send to server
        int bytesSent = 0;
        int bytesLeft = package.Length;

        //netStream.Write(package, 0, package.Length);
        while (bytesLeft > 0)
        {

            int nextPacketSize = (bytesLeft > bufferSize) ? bufferSize : bytesLeft;

            netStream.Write(package, bytesSent, nextPacketSize);
            bytesSent += nextPacketSize;
            bytesLeft -= nextPacketSize;
        }

        if (OnDataComepletelySent != null)
            OnDataComepletelySent();
        // Clean up
        netStream.Close();
        client.Close();
    }

    void InitiateListener()
    {
        listen = new TcpListener(GetIPAddress(), port);
        bKeepRecieving = true;
        //Thread RecieveThread = new Thread(RecieveDataThread);
        //RecieveThread.Start();
        RecieveDataThread();
    }

    void RecieveDataThread()
    {
        while (bKeepRecieving)
        {
            int bufferSize = 1024;
            NetworkStream netStream;
            int bytesRead = 0;
            int allBytesRead = 0;

            // Start listening
            listen.Start();

            // Accept client
            TcpClient client = listen.AcceptTcpClient();
            netStream = client.GetStream();


            // Read length of incoming data
            byte[] length = new byte[4];
            bytesRead = netStream.Read(length, 0, 4);
            int dataLength = BitConverter.ToInt32(length, 0);

            // Read the data
            int bytesLeft = dataLength;
            Data = new byte[dataLength];
            while (bytesLeft > 0)
            {

                int nextPacketSize = (bytesLeft > bufferSize) ? bufferSize : bytesLeft;

                bytesRead = netStream.Read(Data, allBytesRead, nextPacketSize);
                allBytesRead += bytesRead;
                bytesLeft -= bytesRead;

            }

            bRecievedData = true;
            bKeepRecieving = false;
            // Save image to desktop
            // Clean up
            netStream.Close();
            client.Close();
        }
    }

    public bool IsDataRecieved()
    {
        return bRecievedData;
    }

    public ObjectToTransfer ReadObjectAndClear()
    {
        if (IsDataRecieved())
        {
            bRecievedData = false;
            return ByteArrayToObject(Data);
        }
        return null;
    }


    public static byte[] ObjectToByteArray(ObjectToTransfer obj)
    {
        return Encoding.ASCII.GetBytes(JsonUtility.ToJson(obj));
    }

    public static ObjectToTransfer ByteArrayToObject(byte[] arrBytes)
    {
        return JsonUtility.FromJson<ObjectToTransfer>(ASCIIEncoding.ASCII.GetString(arrBytes));
    }

    public IPAddress GetIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        throw new Exception("Local IP Address Not Found!");
    }

    public string GetIPAddressString()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("Local IP Address Not Found!");
    }
}
