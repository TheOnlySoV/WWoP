using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject standardUI;
    public GameObject encounterUI;

    public Transform ballManagerParent;
    public Transform currentBallNameplate;

    InventoryManager im;
    public Transform partyUIParent;
    public Transform pausePartyParent;

    public Transform pauseWindowParent;

    public CompassUtil compassUtil;
    public MiniMapUtil miniMapUtil;

    public Transform trainer;

    public CanvasGroup notificationUIParent;
    public TextMeshProUGUI notificationText;
    public List<Notification> mainNotificationList;
    [Range(0f, 10f)]
    public float notificationTime;

    public bool sendNewNotificationTest = false;
    bool sendingNotification;

    public BarColors barColors;

    [Header("Bag Display")]
    public GameObject bagItemButtonPrefab;
    [Space(10)]
    public Transform floorItemUI;
    public Transform healingItemUI;
    public Transform ballItemUI;
    public Transform moveItemUI;
    public Transform keyItemUI;
    int selectedIndex = -1;

    private void Awake()
    {
        if (instance)
            Destroy(instance);

        instance = this;

        selectedIndex = -1;
    }

    private void Start()
    {
        im = InventoryManager.instance;
        UpdateSelectedBallUI();
        UpdatePlayerUIParty();
    }

    private void Update()
    {
        if (!sendingNotification && mainNotificationList.Count > 0)
            DisplayNewMainNotification(mainNotificationList[0]);
    }

    public void StartEncounter()
    {
        encounterUI.SetActive(true);
        standardUI.SetActive(false);
    }

    public void EndEncounter()
    {
        standardUI.SetActive(true);
        encounterUI.SetActive(false);
    }

    public void UpdatePlayerUIParty()
    {
        for (int i = 0; i <= partyUIParent.childCount - 1; i++)
        {
            if (i > im.party.Count - 1)
            {
                partyUIParent.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            else {
                partyUIParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
            } 
        }
    }

    public void UpdateCompassAndMiniMap(Transform _trainer)
    {
        trainer = _trainer;

        compassUtil.player = trainer;
        miniMapUtil.player = trainer;
        compassUtil.mainCamera = trainer.parent.GetChild(trainer.parent.childCount - 1).transform;
    }

    public void AddNewNotification(Notification newNotification)
    {
        mainNotificationList.Add(newNotification);
    }

    public void DisplayNewMainNotification(Notification noti)
    {
        sendingNotification = true;
        notificationText.text = noti.message;
        mainNotificationList.RemoveAt(0);
        StartCoroutine("NewMainNotification");
    }

    IEnumerator NewMainNotification()
    {
        //fade in the UI to 1
        while (notificationUIParent.alpha < 1)
        {
            notificationUIParent.alpha += Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSecondsRealtime(notificationTime);

        //some identifier to say "ready for next noti..."
        if (mainNotificationList.Count > 0)
            DisplayNewMainNotification(mainNotificationList[0]);
        else
        {
            //fade out Notification UI
            while (notificationUIParent.alpha > 0)
            {
                notificationUIParent.alpha -= Time.deltaTime;
                yield return 0;
            }
            sendingNotification = false;
        }
    }

    public void UpdateSelectedBallUI()
    {
        //update current ball
        Transform selectedParent = ballManagerParent.GetChild(1);
        Pokeball selectedBall = im.bag.pokeballs[im.bag.ballIndex].ball;
        selectedParent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = selectedBall.sprite;
        currentBallNameplate.GetComponent<TextMeshProUGUI>().text = selectedBall.name;

        //if there are more than 1 balls in the ball pocket
        if (im.bag.pokeballs.Count > 1)
        {
            Transform nextParent = ballManagerParent.GetChild(0).GetChild(1);

            int nextIndex = im.bag.ballIndex + 1;
            if (nextIndex > im.bag.pokeballs.Count - 1)
                nextIndex = 0;

            Pokeball nextBall = im.bag.pokeballs[nextIndex].ball;
            nextParent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = nextBall.sprite;

            Transform previousParent = ballManagerParent.GetChild(0).GetChild(0);

            int prevIndex = im.bag.ballIndex - 1;
            if (prevIndex < 0)
                prevIndex = im.bag.pokeballs.Count - 1;

            Pokeball prevBall = im.bag.pokeballs[prevIndex].ball;
            previousParent.GetChild(0).GetChild(0).GetComponent<Image>().sprite = prevBall.sprite;

            ballManagerParent.GetChild(0).gameObject.SetActive(true);
        } else
        {
            ballManagerParent.GetChild(0).gameObject.SetActive(false);
        }

    }

    public void DisplayPokemon()
    {
        for (int i = 0; i < 6; i++)
        {
            if (i < im.party.Count)
            {
                Owned pokemonToDisplay = im.party[i];

                Transform displayedPokemon = pausePartyParent.GetChild(i);
                Destroy(displayedPokemon.GetComponent<Outline>());

                displayedPokemon.GetChild(0).GetComponent<Image>().color = pokemonToDisplay.pokemon.info.type1.typeColor; //type 1 color
                if (pokemonToDisplay.pokemon.info.type2.typeName != "None")
                {
                    displayedPokemon.GetChild(0).GetChild(0).GetComponent<Image>().color = pokemonToDisplay.pokemon.info.type2.typeColor; //type 2 color
                    displayedPokemon.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
                else
                    displayedPokemon.GetChild(0).GetChild(0).gameObject.SetActive(false);

                displayedPokemon.GetChild(1).GetComponent<Image>().sprite = pokemonToDisplay.pokemon.info.profile.gameSprite; //sprite

                displayedPokemon.GetChild(2).GetComponent<TextMeshProUGUI>().text = pokemonToDisplay.pokemon.info.pokemonName; //name
                displayedPokemon.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Level {pokemonToDisplay.level}"; //level

                Slider healthSlider = displayedPokemon.GetChild(3).GetComponent<Slider>();//Health Slider
                healthSlider.maxValue = pokemonToDisplay.stats.HP;
                healthSlider.value = pokemonToDisplay.currentHealth;
                healthSlider.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format($"{healthSlider.value}/{healthSlider.maxValue}"); //health slider

                Slider expSlider = displayedPokemon.GetChild(4).GetComponent<Slider>(); //EXP Slider
                expSlider.maxValue = pokemonToDisplay.nextLevelEXP; //next level exp total
                expSlider.value = pokemonToDisplay.currentEXP; //current exp

                displayedPokemon.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text = pokemonToDisplay.status.ToString(); //status

                pausePartyParent.GetChild(i).gameObject.SetActive(true);
            }
            else
                pausePartyParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void CloseAllPauseWindows()
    {
        for (int i = 0; i <= pauseWindowParent.childCount - 1; i++)
            pauseWindowParent.GetChild(i).gameObject.SetActive(false);
    }

    public void DisplayBagPocket()
    {
        if (selectedIndex < 0)
            selectedIndex = 0;

        switch(selectedIndex)
        {
            case 0:
                DisplayItems();
                break;
            case 1:
                DisplayHealItems();
                break;
            case 2:
                DisplayBalls();
                break;
            case 3:
                DisplayMoves();
                break;
            case 4:
                DisplayKeyItems();
                break;

        }
    }

    public void DisplayItems()
    {
        ClearBagChildren(floorItemUI);
        selectedIndex = 0;
        foreach (BagItem item in InventoryManager.instance.bag.itemBag)
        {
            GameObject newButtonObj = Instantiate(bagItemButtonPrefab, floorItemUI);
            newButtonObj.GetComponent<Image>().sprite = item.item.sprite;
            newButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.item.name;
            newButtonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{item.quantity}";

            Button button = newButtonObj.GetComponent<Button>();
        }
    }

    public void DisplayHealItems()
    {
        ClearBagChildren(healingItemUI);
        selectedIndex = 1;
        foreach (HealingInventoryItem item in InventoryManager.instance.bag.healingBag)
        {
            GameObject newButtonObj = Instantiate(bagItemButtonPrefab, healingItemUI);
            newButtonObj.GetComponent<Image>().sprite = item.item.sprite;
            newButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.item.name;
            newButtonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{item.quantity}";

            Button button = newButtonObj.GetComponent<Button>();
        }
    }

    public void DisplayBalls()
    {
        ClearBagChildren(ballItemUI);
        selectedIndex = 2;
        foreach (BallInventoryItem item in InventoryManager.instance.bag.pokeballs)
        {
            GameObject newButtonObj = Instantiate(bagItemButtonPrefab, ballItemUI);
            newButtonObj.GetComponent<Image>().sprite = item.ball.sprite;
            newButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.ball.name;
            newButtonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{item.quantity}";

            Button button = newButtonObj.GetComponent<Button>();
        }
    }

    public void DisplayMoves()
    {
        ClearBagChildren(moveItemUI);
        selectedIndex = 3;
        foreach (TMInventoryItem item in InventoryManager.instance.bag.moveBag)
        {
            GameObject newButtonObj = Instantiate(bagItemButtonPrefab, moveItemUI);
            newButtonObj.GetComponent<Image>().sprite = item.item.sprite;
            newButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.item.name;
            newButtonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{item.quantity}";

            Button button = newButtonObj.GetComponent<Button>();
        }
    }

    public void DisplayKeyItems()
    {
        ClearBagChildren(keyItemUI);
        selectedIndex = 4;
        foreach (KeyInventoryItem item in InventoryManager.instance.bag.keyItems)
        {
            GameObject newButtonObj = Instantiate(bagItemButtonPrefab, keyItemUI);
            newButtonObj.GetComponent<Image>().sprite = item.item.sprite;
            newButtonObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.item.name;
            newButtonObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"x{item.quantity}";

            Button button = newButtonObj.GetComponent<Button>();
        }
    }

    void ClearBagChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
            Destroy(parent.GetChild(i).gameObject);
    }

    public void SetPocketIndex(int index)
    {
        //currentPocketIndex = index;
    }

    public void ExecuteUseItem(int index)//can only call this if the player is in the healing or floor item bag
    {
    //    string textToDisplay = string.Empty;
    //    switch (currentPocket)
    //    {
    //        case BagSlot.Item:
    //            if (im.itemBag[currentPocketIndex].item.UseBattleItem(0))
    //            {
    //                im.itemBag[currentPocketIndex].quantity--;
    //                if (im.itemBag[currentPocketIndex].quantity <= 0)
    //                    im.itemBag.RemoveAt(currentPocketIndex);
    //                textToDisplay = string.Format("Used {0} on {1}!", im.itemBag[currentPocketIndex].item.name, im.ownedCritters[0].critter.critterName);
    //            }
    //            else
    //                textToDisplay = string.Format("{0} had no effect!");

    //            encounterDisplayTextUI.transform.GetChild(0).GetComponent<Text>().text = textToDisplay;
    //            encounterDisplayTextUI.SetActive(true);

    //            StartCoroutine(UsingItemTextUI());

    //            break;
    //        case BagSlot.Healing:
    //            if (index == GetCurrentPartyIndex(index))
    //                StartCoroutine(DisplayHealing());

    //            im.healingBag[currentPocketIndex].item.HealPokemon(index);

    //            encounterDisplayTextUI.transform.GetChild(0).GetComponent<Text>().text = string.Format("Used {0} on {1}!", im.healingBag[currentPocketIndex].item.name, im.ownedCritters[index].critter.critterName);
    //            encounterDisplayTextUI.SetActive(true);

    //            break;
    //    }
    //    encounterUIParent.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
    }

    int GetCurrentPartyIndex(int selectedIndex)
    {
    //    for (int i = 0; i <= im.ownedCritters.Count - 1; i++)
    //    {
    //        print(string.Format("Checking {0} against {1}", im.ownedCritters[i].name, myPokemon.name));
    //        if (im.ownedCritters[i] == myPokemon)
    //        {
    //            print("RETURNING!");
    //            return i;
    //        }
    //    }
        return 0;
    }

    void SetMyUI()
    {
        //Transform myUIParent = encounterUIParent.transform.GetChild(0).GetChild(0).GetChild(1);

        //myUIParent.GetChild(0).GetChild(0).GetComponent<Text>().text = myPokemon.pokemon.info.pokemonName;

        //myUIParent.GetChild(1).GetComponent<Slider>().maxValue = myPokemon.stats.HP;//Health Slider
        //myUIParent.GetChild(1).GetComponent<Slider>().value = myPokemon.currentHealth;//Health Slider

        //myUIParent.GetChild(2).GetComponent<Slider>().maxValue = myPokemon.nextLevelEXP;//EXP Slider
        //myUIParent.GetChild(2).GetComponent<Slider>().value = myPokemon.currentEXP;//EXP Slider

        //myUIParent.GetChild(3).GetComponent<Text>().text = string.Format("Lv. {0}", myPokemon.level);
    }

    public void EvaluateHealth(Slider slider)
    {
        if (slider.value > slider.maxValue / 2)
            slider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = barColors.healthy;
        if (slider.value > slider.maxValue / 5 && slider.value <= slider.maxValue / 2)
            slider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = barColors.hurt;
        if (slider.value <= slider.maxValue / 5)
            slider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = barColors.critical;

        slider.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = string.Format($"{(int)slider.value}/{(int)slider.maxValue}");
    }

    //IEnumerator FadeUIIn(CanvasGroup target, float delay)
    //{
    //    target.gameObject.SetActive(true);
    //    float t = 0f;
    //    while (t < delay)
    //    {
    //        t += Time.deltaTime;
    //        float a = fadeCurve.Evaluate(t);
    //        target.alpha = a;
    //        yield return 0;
    //    }
    //    target.interactable = true;
    //    target.blocksRaycasts = true;
    //}

    //IEnumerator FadeUIOut(CanvasGroup target, float delay)
    //{
    //    target.interactable = false;
    //    target.blocksRaycasts = false;
    //    float t = delay;
    //    while (t > 0)
    //    {
    //        t -= Time.deltaTime;
    //        float a = fadeCurve.Evaluate(t);
    //        target.alpha = a;
    //        yield return 0;
    //    }
    //    target.gameObject.SetActive(false);
    //}
}

[Serializable]
public class Notification
{
    public string sender;
    public string message;
    public Sprite icon;

    public Notification(string sender, string message)
    {
        this.sender = sender;
        this.message = message;
    }
    public Notification(string sender, string message, Sprite icon)
    {
        this.sender = sender;
        this.message = message;
        this.icon = icon;
    }
}

[Serializable]
public class BarColors
{
    public Color healthy;
    public Color hurt;
    public Color critical;
    [Space(10)]
    public Color expBar;
}