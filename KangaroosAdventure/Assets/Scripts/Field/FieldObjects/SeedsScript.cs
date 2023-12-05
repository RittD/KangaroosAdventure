using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(FieldTextScript))]
public class SeedsScript : MonoBehaviour {
    private bool destroy = false;
    private Vector3 originalScale;
    
    private float maxGrowth = 0.04f;
    private float periodLength = 4;
    private float randomMovementOffset;

    private void Start()
    {
        originalScale= transform.localScale;
        randomMovementOffset = Random.Range(0, periodLength);
    }


    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Player")) {
            destroy = true;

            StaminaScript.GetInstance().CollectSeed();


            Destroy(gameObject, 2);

            GameManager gameManager = GameManager.GetInstance();

            GetComponent<FieldTextScript>().CreateTextOnGround();

            AudioManager.GetInstance().PlaySound(Sound.EATING);
        }
    }

    private void Update() {
        if (destroy)
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), 0.3f);

        else if (GameStateHandler.GetGameState() == GameState.GAME)
        {
            float growthFactor = maxGrowth * Mathf.Sin((randomMovementOffset + Time.time) / periodLength * 2 * Mathf.PI);
            transform.localScale = originalScale * (1 + growthFactor);
        }
    }
}
