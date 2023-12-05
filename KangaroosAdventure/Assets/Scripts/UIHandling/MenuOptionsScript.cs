using UnityEngine;
using UnityEngine.UI;

public class MenuOptionsScript : MonoBehaviour {
    
    public Slider bombSlider;
    public Slider treeSlider;
    public Slider seedSlider;

    public Text bombCountText;
    public Text treeCountText;
    public Text seedCountText;

    public Toggle easyToggle;
    public Toggle mediumToggle;
    public Toggle hardToggle;
    public Toggle customToggle;
    public ToggleGroup difficultyTG;

    private float easyBombCount = 35f;
    private float easyTreeCount = 3f;
    private float easySeedCount = 25f;

    private float mediumBombCount = 70f;
    private float mediumTreeCount = 5f;
    private float mediumSeedCount = 30f;

    private float hardBombCount = 100f;
    private float hardTreeCount = 5f;
    private float hardSeedCount = 25f;

    private bool waiting = false;
    private bool updateSlider = true;


    public void SetEasy() {
        if (!updateSlider)
            return;

        waiting = true;
        bombSlider.value = easyBombCount;
        treeSlider.value = easyTreeCount;
        waiting = false;
        seedSlider.value = easySeedCount;
    }

    public void SetMedium() {
        if (!updateSlider)
            return;

        waiting = true;
        bombSlider.value = mediumBombCount;
        treeSlider.value = mediumTreeCount;
        waiting = false;
        seedSlider.value = mediumSeedCount;
    }

    public void SetHard() {
        if (!updateSlider)
            return;

        waiting = true;
        bombSlider.value = hardBombCount;
        treeSlider.value = hardTreeCount;
        waiting = false;
        seedSlider.value = hardSeedCount;
    }

    //public void SetRandom()
    //{
    //    waiting = true;
    //    bombSlider.value = Random.Range(bombSlider.minValue, bombSlider.maxValue + 1);
    //    treeSlider.value = Random.Range(treeSlider.minValue, treeSlider.maxValue + 1);
    //    waiting = false;
    //    seedSlider.value = Random.Range(seedSlider.minValue, seedSlider.maxValue + 1);
    //}

    public void UpdateBombCount()
    {
        FieldHandler.GetInstance().SetBombCount((int)bombSlider.value);
        bombCountText.text = "" + (int)bombSlider.value;
        UpdateToggleAndScene();

    }

    private void UpdateToggleAndScene()
    {
        if (!waiting)
        {
            GameManager.GetInstance().RefreshScene();
            float bombs = bombSlider.value;
            float trees = treeSlider.value;
            float seeds = seedSlider.value;

            updateSlider = false;

            if (bombs == easyBombCount && trees == easyTreeCount && seeds == easySeedCount)
            {
                easyToggle.isOn = true;
                mediumToggle.isOn = hardToggle.isOn = customToggle.isOn = false;
            }

            else if (bombs == mediumBombCount && trees == mediumTreeCount && seeds == mediumSeedCount)
            {
                mediumToggle.isOn = true;
                easyToggle.isOn = hardToggle.isOn = customToggle.isOn = false;
            }

            else if (bombs == hardBombCount && trees == hardTreeCount && seeds == hardSeedCount)
            {
                hardToggle.isOn = true;
                easyToggle.isOn = mediumToggle.isOn = customToggle.isOn = false;
            }

            else
            {
                customToggle.isOn = true;
                easyToggle.isOn = mediumToggle.isOn = hardToggle.isOn = false;
            }

            updateSlider = true;

        }
    }

    public void UpdateTreeCount() {
        FieldHandler.GetInstance().SetTreeCount((int)treeSlider.value);
        treeCountText.text = "" + (int)treeSlider.value;
        UpdateToggleAndScene();
    }

    public void UpdateSeedCount() {
        FieldHandler.GetInstance().SetSeedCount((int)seedSlider.value);
        seedCountText.text = "" + (int)seedSlider.value;
        UpdateToggleAndScene();
    }

    public void BackToMenu() {
        GameStateHandler.SetGameState(GameState.MENU);
    }

    public void ToggleSoundActive() {
        AudioManager.GetInstance().ToggleSounds();
    }

    public void ToggleMusicActive() {
        AudioManager.GetInstance().ToggleMusic();
    }
}
