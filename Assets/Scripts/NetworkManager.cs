using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    Socket socket;
    bool isConnected;
    public GameObject player;
    public GameObject cube;

    BinaryFormatter sbf;
    MemoryStream sms;

    BinaryFormatter rbf;
    MemoryStream rms;

    Queue<BasePacket> sendQueue;

    static NetworkManager instance = null;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        isConnected = false;
        sbf = new BinaryFormatter();
        sms = new MemoryStream();

        rbf = new BinaryFormatter();
        rms = new MemoryStream();

        sendQueue = new Queue<BasePacket>();
    }

    void Update()
    {
        //inputText = GameObject.Find("UserInput").GetComponent<InputField>().text;
        //inputText = GetComponent<InputField>()?.text;



    }

    public void Connect()
    {
        if (isConnected == false)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4420));
            socket.Blocking = false;
            isConnected = true;
            StartCoroutine(NetworkLoop());
            print("Client connected to server!");
        }
        else
        {
            Debug.LogError("You're already connected fam");
        }
    }

    public void Disconnect()
    {
        socket.Disconnect(false);
        isConnected = false;
    }

    public void SendingMessages()
    {
        ChatPacket chatPacket = new ChatPacket()
        {
            username = "Chris",
            message = "Sup mate"
        };
        sendQueue.Enqueue(chatPacket);
    }

    public void InstantiateObject() 
    {
        InstantiatePacket instantatiatePacket = new InstantiatePacket()
        {
            gameObjectName = "Enemy",
            position = new Vec3(3, 2, -1),
            rotation = new Vec3(0, 0, 0)
        };

        sendQueue.Enqueue(instantatiatePacket);
        Instantiate(
            Resources.Load<GameObject>(instantatiatePacket.gameObjectName), 
            instantatiatePacket.position.GetVector3(),
            Quaternion.Euler(instantatiatePacket.rotation.GetVector3())
            );
        Debug.LogError("man press");

    }

    public void LoadSceneButton() 
    {
        ScenePacket scenePacket = new ScenePacket()
        {
            sceneIndex = 1
        };
        sendQueue.Enqueue(scenePacket);

        SceneManager.LoadScene(scenePacket.sceneIndex);
    }
    /*public void MovementMessage()
    {
        MovementPacket movementPacket = new MovementPacket()
        {
            username = "Chris",
            positionX = player.transform.position.x,
            positionY = player.transform.position.y,
            positionZ = player.transform.position.z,

            rotationX = player.transform.rotation.eulerAngles.x,
            rotationY = player.transform.rotation.eulerAngles.y,
            rotationZ = player.transform.rotation.eulerAngles.z,
        };

        byte[] buffer = Util.Serialize(movementPacket);
        socket.Send(buffer);
    }*/

    public IEnumerator NetworkLoop()
    {
        float tickRate = 1.0f / 14.0f;
       

        while (isConnected)
        {
            try
            {

                if (player != null || cube != null)
                {
                    MovementPacket movementPacket = new MovementPacket()
                    {
                        position = new Vec3(player.transform.position),
                        rotation = new Vec3(player.transform.rotation.eulerAngles)
                    };
                    sendQueue.Enqueue(movementPacket);
                }


                if (sendQueue.Count > 0)
                {
                    sms.Seek(0, SeekOrigin.Begin);
                    sbf.Serialize(sms, sendQueue.Dequeue());
                    sms.Seek(0, SeekOrigin.Begin);
                    socket.Send(sms.ToArray());
                }
                

            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.WouldBlock)
                {
                    Debug.LogError(ex);
                }
            }

            try
            {
                byte[] buffer = new byte[2048];
                socket.Receive(buffer);

                rms.Seek(0, SeekOrigin.Begin);
                Debug.Log("We are here" + rms.Length);
                rms.Write(buffer, 0, 2048);
                rms.Seek(0, SeekOrigin.Begin);
                //MemoryStream m = new MemoryStream(buffer);

                if (rms.Length > 0)
                {

                    try
                    {
                        BasePacket BP = (BasePacket)rbf.Deserialize(rms);
                        switch (BP.packetType)
                        {
                            case BasePacket.Type.ChatPacket:
                                ChatPacket CP = (ChatPacket)BP;
                                Debug.LogError($"{CP.username}: {CP.message}");
                                break;
                            case BasePacket.Type.MovementPacket:
                                MovementPacket MP = (MovementPacket)BP;
                                cube.transform.position = MP.position.GetVector3();
                                cube.transform.rotation = Quaternion.Euler(MP.rotation.GetVector3());
                                //Debug.LogError($"{MP.positionX}: {MP.positionY}: {MP.positionZ}: {MP.rotationX}: {MP.rotationY}:{MP.rotationZ}");
                                break;
                            case BasePacket.Type.InstantiatePacket:
                                InstantiatePacket IP = (InstantiatePacket)BP;
                                Instantiate(Resources.Load<GameObject>(IP.gameObjectName), IP.position.GetVector3(), Quaternion.Euler(IP.rotation.GetVector3()));
                                print(IP.gameObjectName);
                                break;
                            case BasePacket.Type.ScenePacket:
                                ScenePacket SP = (ScenePacket)BP;
                                SceneManager.LoadScene(SP.sceneIndex);  
                                break;
                            default:
                                break;
                        }
                    }
                    catch
                    {
                        print("Failed to Deserialize");   
                    }

                   
                }

            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.WouldBlock)
                {
                    Debug.LogError(ex);
                }
            }
            yield return new WaitForSeconds(tickRate);
        }
    }
}