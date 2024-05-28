using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    public Button startButton;
    public List<Server> servers = new List<Server>();

    [Header("Unity Editor Setup Fields")]
    public TMP_InputField IDInputField;

    [SerializeField]
    int serverID;
    [SerializeField]
    int enteredID;
    [SerializeField]
    string enteredName = string.Empty;
    [SerializeField]
    int selectedServer = 0;
    [SerializeField]
    int selectedModel = 0;

    bool ready = false;

    public CanvasGroup startGameFader;
    public AnimationCurve fadeCurve;

    private void OnValidate()
    {
        foreach (Server server in servers)
        {
            server.serverName = server.sceneToLoad.ToString();
        }
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (!ready)
            return;
    }

    public void SetName(TMP_InputField inputField)
    {
        enteredName = inputField.text;
    }

    public void SetID(TMP_InputField inputField)
    {
        if (int.Parse(inputField.text) > 999999)
        {
            enteredID = 999999;
        }
        if (int.Parse(inputField.text) < 0)
        {
            enteredID = 0;
        }
        enteredID = int.Parse(inputField.text.ToString().PadLeft(6, '0'));
    }

    public void SelectMap(TMP_Dropdown dropdown)
    {
        selectedServer = dropdown.value;
    }

    public void SelectModel(TMP_Dropdown dropdown)
    {
        selectedModel = dropdown.value;
    }

    public void SetServer(int scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public void CheckRandomToggle(Toggle random)
    {
        if (random.isOn)
        {
            enteredID = Random.Range(0, 999999);
            //turn off the input field
            IDInputField.interactable = false;
            IDInputField.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = enteredID.ToString();
        }
        else
        {
            IDInputField.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "0-999999";
            IDInputField.interactable = true;
        }
    }

    public void TransferServerInformation(NetworkManager transferTarget)
    {
        transferTarget.port = servers[selectedServer].streamPort;
        transferTarget.host = servers[selectedServer].streamPath;

        transferTarget.clientTrainer.modelSelection = selectedModel;
        transferTarget.clientTrainer.serverID = enteredID;
        transferTarget.clientTrainer.tID = enteredID;
        transferTarget.clientTrainer.name = enteredName;
        
        //create character
        GameObject newTrainer = Instantiate(transferTarget.modelOptions[selectedModel].clientPrefab);
        //put the trainer at the save location
        Transform trainer = newTrainer.transform.GetChild(0);
        trainer.name = enteredName;
        trainer.GetComponent<NetworkedTransform>().modelSelection = selectedModel;
        trainer.GetComponent<NetworkedTransform>().ServerID = serverID;
        trainer.GetComponent<NetworkedTransform>().tID = enteredID;
        trainer.GetComponent<NetworkedTransform>().username = enteredName;

        GameManager.instance.trainer = trainer.gameObject;

        //Destroy(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void BeginNewGame()
    {
        StartCoroutine("NewGameFade");
    }

    IEnumerator NewGameFade()
    {
        startGameFader.blocksRaycasts = true;

        enteredID = Random.Range(0, 999999);
        //get the serverID
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime;
            float a = fadeCurve.Evaluate(t);
            startGameFader.alpha = a;
            yield return 0;
        }

        SetServer(1);
    }
}

[System.Serializable]
public class Server
{
    [HideInInspector]
    public string serverName;

    public string streamPath = IPAddress.None.ToString();
    public ushort streamPort = 0;

    public Maps sceneToLoad;
}

public enum Maps { MainMenu, Kanto, Johto, Hoenn, Sinnoh, Unova, Kalos, Alola, Overworld, TestEnvironment }