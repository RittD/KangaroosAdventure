using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(StaminaScript))]
[RequireComponent(typeof(CanvasHandler))]
public class GameManager : MonoBehaviour {
    
    private static GameManager instance;


    public GameObject fenceDoorObj;
    public GameObject chickenObj;

    public GameObject reviveButton;
    public GameObject restartButton;
    public GameObject menuButton;
    public Toggle stdViewToggle;

    public Text debugText;


    [Range(0.0f, 1.0f)]
    public static float smoothness = 0.1f;

    [Range(0.0f, 1.0f)]
    public static float bombFadeoutSmoothness = 0.4f;

    private int revivesPerGame = 3;
    public int remainingRevives;

    private int bomb;
    private int bush;
    private int tree;
    private int seed;

    private bool allDestroyed = false;

    int failedLoadingAttempts;



    private readonly string adId = "ca-app-pub-3940256099942544/5224354917";//Debug:ca-app-pub-3940256099942544/5224354917 //Real: ca-app-pub-4850295519510041/8434429887
    public RewardedAd ad;
    bool adWatched = false;
    float lastOne = 0;


    public static GameManager GetInstance() {
        return instance;
    }

    void Start()
    {
        InitSingletons();        

        MobileAds.Initialize(initStatus => { });
        CreateAndLoadAd();

        GameStateHandler.SetGameState(GameState.MENU);
        CanvasHandler.GetInstance().SetMenuCanvasActive(false);
    }
    

    private void InitSingletons()
    {
        instance = this;
        GetComponent<FieldHandler>().Init();
        GetComponent<AudioManager>().Init();
        GetComponent<StaminaScript>().Init();
        GetComponent<CanvasHandler>().Init();
    }

    public void RestartGame(){
        RefreshScene();

        GameStateHandler.SetGameState(GameState.GAME);
    }

    public void RefreshScene(){
        FieldHandler fieldCreator = FieldHandler.GetInstance();
        bool notFirstGame = fieldCreator.fields != null;


        Animation doorAnimation = fenceDoorObj.GetComponent<Animation>();
        doorAnimation.Rewind();
        doorAnimation.Play();
        doorAnimation.Sample();
        doorAnimation.Stop();
        remainingRevives = revivesPerGame;

        if (notFirstGame)
            fieldCreator.DestroyAllObjectsWithTag("FieldObject");

        allDestroyed = true;
    }



    void Update()
    {
        ReviveIfVideoWatched();

        ReloadAdIfNecessary();

        if (allDestroyed && GameObject.FindGameObjectsWithTag("FieldObject").Length == 0)
        {
            FieldHandler.GetInstance().SetUpNewField();
            allDestroyed = false;
        }
    }

    private void ReviveIfVideoWatched()
    {
        if (adWatched)
        {
            ReviveChicken();
            adWatched = false;
        }
    }

    private void ReviveChicken()
    {
        chickenObj.transform.position = GridMovement.targetPos + Vector3.up;
        GameStateHandler.SetGameState(GameState.GAME);
    }

    private void ReloadAdIfNecessary()
    {
        if (failedLoadingAttempts > 0 && failedLoadingAttempts < 10)
        {
            float time = Time.time;
            if (time - lastOne > 10)
            {
                CreateAndLoadAd();
                lastOne = time;
            }
        }
    }


    public bool GetAllDestroyed()
    {
        return allDestroyed;
    }


    public void HideReviveButton()
    {
        reviveButton.SetActive(false);
        restartButton.SetActive(true);
        menuButton.SetActive(true);
    }

    public void TryToShowReviveButton()
    {

        if (remainingRevives > 0)
        {
            bool adLoaded = ad.IsLoaded();
            reviveButton.GetComponent<Button>().interactable = adLoaded;
            reviveButton.GetComponentInChildren<Text>().text = adLoaded ? "Watch video to revive" : "No video available";
            
            reviveButton.SetActive(true);
            restartButton.SetActive(false);
            menuButton.SetActive(false);
            Invoke("HideReviveButton", 5f);
            remainingRevives--;
        }
        else
            HideReviveButton();
        
    }

    private void CreateAndLoadAd()
    {
        ad = new RewardedAd(adId);
        ad.OnAdLoaded += HandleAdLoaded;
        ad.OnAdFailedToLoad += HandleAdFailedToLoad;
        ad.OnUserEarnedReward += HandleUserEarnedReward;
        ad.OnAdClosed += HandleAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        ad.LoadAd(request);
    }

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        failedLoadingAttempts = 0;
    }

    public void HandleAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        failedLoadingAttempts++;
    }


    public void HandleAdClosed(object sender, EventArgs args)
    {
        CreateAndLoadAd();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        adWatched = true;
    }
}