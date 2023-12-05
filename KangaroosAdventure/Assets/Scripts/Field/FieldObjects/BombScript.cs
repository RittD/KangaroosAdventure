using UnityEngine;

public class BombScript : MonoBehaviour {
    public bool destroy = false;

    // Update is called once per frame
    void Update() {
        if (destroy) {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), GameManager.bombFadeoutSmoothness);
        }
    }
}
