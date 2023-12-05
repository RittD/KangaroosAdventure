using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour {
    
    public int max_stamina = 125;
    private int stamina;
    public Color red;
    public Color yellow;
    public Color green;


    public int staminaPerStep = 1;
    public int seedSaturation = 50;
    private bool[] active = { true, true, true, true, true };
    private Color barColor;

    public List<GameObject> bars;

    private static StaminaScript instance;

    public void Init()
    {
        instance = this;
        RefreshStamina();
    }


    public static StaminaScript GetInstance() {
        return instance;
    }

    public void DoStep(Vector3 nextPos)
    {
        stamina -= staminaPerStep;
        Vector2 nextField = GridMovement.GetGridPosition(nextPos);

        //Save the chicken, if it lands on a seed with its last step
        if (FieldHandler.GetInstance().GetFieldTypeOfField(nextField) == FieldType.SEED)
            return;

        CheckForChickenStarving();
        SetStaminaBars();
    }

    private void CheckForChickenStarving()
    {
        if (stamina <= 0)
        {
            GameStateHandler.SetGameState(GameState.LOSS);
            AudioManager.GetInstance().PlaySound(Sound.STARVING);
            GameManager.GetInstance().chickenObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
    }

    public void CollectSeed()
    { 
        stamina += seedSaturation;
        if (stamina > max_stamina)
            stamina = max_stamina;
        SetStaminaBars();
    }

    public void SetStaminaBars() {
        int staminaLevel = (int)Math.Ceiling(stamina / (max_stamina/5f));

        if (staminaLevel == 5) {
            active = new bool[] { true, true, true, true, true };
            barColor = green;
        } else if (staminaLevel == 4) {
            active = new bool[] { true, true, true, true, false };
            barColor = green;
        } else if (staminaLevel == 3) {
            active = new bool[] { true, true, true, false, false };
            barColor = yellow;
        } else if (staminaLevel == 2) {
            active = new bool[] { true, true, false, false, false };
            barColor = yellow;
        } else if (staminaLevel == 1) {
            active = new bool[] { true, false, false, false, false };
            barColor = red;
        } else {
            active = new bool[] { false, false, false, false, false };
            barColor = red;
        }

        for(int i = 0; i < active.Length; i++) {
            bars[i].SetActive(active[i]);
            bars[i].GetComponent<Image>().color = barColor;
        }
            
    }

    public void RefreshStamina() {
        stamina = max_stamina;
        SetStaminaBars();
    }
}