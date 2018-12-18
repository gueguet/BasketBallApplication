using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient : MonoBehaviour
{

    public GameObject ball;
    private float xPos = 10.0f;
    private float yPos = 10.0f;

    // you can change these values to change the x and y axis translations 
    public float xFactor = 00.1f;
    public float yFactor = 000.1f;   


    Thread receiveThread;
    UdpClient client;
    public int port;

    // here we receive data as string
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = "";

    void Start()
    {
        init();
    }

    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);

        GUIStyle style = new GUIStyle();

        style.alignment = TextAnchor.UpperLeft;

        GUI.Box(rectObj, "# UDPReceive\n127.0.0.1 " + port + " #\n"

                  //+ "shell> nc -u 127.0.0.1 : "+port +" \n"

                  + "\nLast Packet: \n" + lastReceivedUDPPacket

                  //+ "\n\nAll Messages: \n"+allReceivedUDPPackets

                  , style);

    }

    private void init()
    {
        print("UPDSend.init()");

        port = 5065;

        print("Sending to 127.0.0.1 : " + port);

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                // we split the string so as to get float data for x and y position
                string[] positionArray = text.Split(char.Parse(","));

                StringBuilder sb_1 = new StringBuilder(positionArray[0]);
                sb_1.Remove(0, 1);
                positionArray[0] = sb_1.ToString();

                StringBuilder sb_2 = new StringBuilder(positionArray[1]);
                sb_2.Remove(positionArray[1].Length - 1, 1);
                positionArray[1] = sb_2.ToString();

                xPos = float.Parse(positionArray[0]); 
                yPos = float.Parse(positionArray[1]);

                lastReceivedUDPPacket = text;
                allReceivedUDPPackets = allReceivedUDPPackets + text;
            }

            catch (Exception e)
            {
                print(e.ToString());
            }
        }
    }

    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }

    // Update is called once per frame
    void Update()
    {

        // set the values to reassess the translation 
        ball.transform.position = new Vector3(-((xPos * xFactor) - 32), -(yPos * yFactor) + 24, 6);

    }

    void OnApplicationQuit()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
            Debug.Log(receiveThread.IsAlive); //must be false
        }
    }
}