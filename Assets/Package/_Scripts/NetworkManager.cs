using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEngine.Windows;
using System.Diagnostics.CodeAnalysis;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public Transform serverPlayerParent;

    public string host;
    public ushort port;

    public static UdpClient client;
    public static IPEndPoint ep;

    public TrainerData clientTrainer;
    public List<TrainerData> serversideTrainer;

    public List<ModelOption> modelOptions;

    void Awake()
    {
        if (instance != null)
            Destroy(instance);

        MainMenuManager.instance.TransferServerInformation(this);
        Destroy(MainMenuManager.instance.gameObject);
        instance = this;

        client = new UdpClient();
        client.Connect(host, port);
    }

    private void Start()
    {
        SendMessageToServer(ServerInterpreter.StringifyData(clientTrainer));
    }

    private void Update()
    {
        Recieve();//Keep the feed open for updating player positions
    }

    public async void Recieve()
    {
        //Recieve
        var results = await client.ReceiveAsync();
        byte[] recieveBytes = results.Buffer;
        string str = Encoding.ASCII.GetString(recieveBytes);

        string[] command = str.Split("|");

        //print(command[0]);
        switch (command[0])
        {
            case "NetworkTrainerData":
                List<TrainerData> trainers;
                List<string> fromServer = new List<string>();
                
                for(int i = 1; i < command.Length; i++)
                {
                    fromServer.Add(command[i]);
                }
                trainers = ServerInterpreter.InterpretData(fromServer.ToArray());

                for (int i = 0; i < trainers.Count; i++)
                {
                    bool add = true;

                    for (int j = 0; j < serversideTrainer.Count; j++)
                    {
                        if (trainers[i].serverID == serversideTrainer[j].serverID)
                        {
                            serversideTrainer[j] = trainers[i];
                            add = false;
                        }
                    }

                    if (add == true && trainers[i].serverID != clientTrainer.serverID)
                    {
                        serversideTrainer.Add(trainers[i]);
                        //print($"{trainers[i].name} {trainers[i].modelSelection} {modelOptions[trainers[i].modelSelection].serverPrefab}");
                        GameObject newServerPlayer = Instantiate(modelOptions[trainers[i].modelSelection].serverPrefab, serverPlayerParent);

                        newServerPlayer.transform.GetChild(0).GetComponent<NetworkedTransform>().username = trainers[i].name;
                        newServerPlayer.transform.GetChild(0).GetComponent<NetworkedTransform>().SetServerID(trainers[i].serverID);
                        newServerPlayer.name = trainers[i].name;
                    }

                    if (clientTrainer.serverID < 0)
                    {
                        clientTrainer.serverID = trainers[i].serverID;
                        if (GameManager.instance.trainer.GetComponent<NetworkedTransform>().ServerID != clientTrainer.serverID)
                            GameManager.instance.trainer.GetComponent<NetworkedTransform>().ServerID = clientTrainer.serverID;
                    }
                }
                break;
            default:
                break;
        }
    }

    public void UpdateClientObjectData(TrainerData obj)
    {
        if (clientTrainer.serverID == obj.serverID)
        {
            clientTrainer = obj;
            SendMessageToServer(ServerInterpreter.StringifyData(clientTrainer));
            return;
        }
    }

    public TrainerData GetServerObjectData(int ID, bool client)
    {
        if (client)
        {
            if (clientTrainer.serverID == ID)
            {
                return clientTrainer;
            }
        }

        for (int i = 0; i < serversideTrainer.Count; i++)
        {
            if (serversideTrainer[i].serverID == ID)
            {
                return serversideTrainer[i];
            }
        }

        return null;
    }

    public async void SendMessageToServer(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        await client.SendAsync(data, data.Length);
        //print(message);
    }

    public void TryEmote(int index)
    {
        GameManager.instance.trainer.GetComponent<PlayerController>().TryEmote(index);
    }
}
[System.Serializable]
public class ServerPlayer
{
    public string playerName;
    public int playerID;
    public Transform serverPlayer;

    [AllowNull] public Vector3 position;//Leave empty in the inspector
    public int emoteState;//Leave empty in the inspector
    public bool emoteActive;//Leave empty in the inspector
}

[System.Serializable]
public class ModelOption
{
    public string modelName;
    public GameObject clientPrefab;
    public GameObject serverPrefab;
}