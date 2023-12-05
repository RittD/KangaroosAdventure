using BrokenVector.LowPolyFencePack;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent (typeof (FieldTextScript))]
public class BushScript : MonoBehaviour {

    private Vector3 hiddenBombOffset = new Vector3(0.008f, 0.214f, -0.015f);
    public bool hasBomb = false;
    public bool isLightBush;

    private bool doFadeOut = false;
    private float fadeOutDuration = 0.2f;
    public bool isWithinField = true;


    public GameObject PrefabBomb;
    public GameObject PrefabExplosion;
    public GameObject PrefabExplosionMark;

    public GameObject PrefabFlag;


    private GameObject flag = null;


    GameField field;

    GameObject bomb;

    private Renderer renderer;
    private int materialCount;

    public int fadingConstant = 12;


    private void Start()
    {
        renderer = GetComponent<Renderer>();
        materialCount = renderer.materials.Length;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.tag.Equals("Player")) 
            return;

        PrepareBushFadeOut();


        if (isWithinField) {
            CheckGameWon();

            if (field.type == FieldType.BOMB)
            {
                PlaceBomb();
                //ChickenController.desiredPosition = gameObject.transform.position;
                GridMovement.targetPos = gameObject.transform.position;
                GameStateHandler.SetGameState(GameState.LOSS);
                Invoke("CreateExplosion", 0.5f);
            } else
                GetComponent<FieldTextScript>().CreateTextOnGround();
        }

        AudioManager.GetInstance().PlaySound(Sound.BUSH);

        if (flag != null)
            Destroy(flag, 0);
    }

    private void PlaceBomb()
    {
        Vector3 bombPos = new Vector3(field.posX, 0, field.posZ) + hiddenBombOffset + FieldHandler.FIELD_OFFSET;
        GameObject bombObj = Instantiate(PrefabBomb, bombPos, Quaternion.Euler(new Vector3(0, 180, 0)));
        bomb = bombObj;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    private void OnMouseDown()
    {

        if (GameStateHandler.GetGameState() != GameState.GAME || !isWithinField || IsPointerOverUIObject())
            return;

        SetOrRemoveFlag();
    }

    private void SetOrRemoveFlag()
    {

        if (flag == null)
        {
            float flagOffsetY = (0.635f + FieldHandler.FIELD_OFFSET.y - transform.position.y) * (transform.localScale.z / 30f);
            flag = Instantiate(PrefabFlag, transform.position + new Vector3(0, flagOffsetY, 0), Quaternion.identity);
        }
        else
            Destroy(flag, 0);

        AudioManager.GetInstance().PlaySound(Sound.FLAG);
    }

    private void CheckGameWon() {
        Vector3 currentPosition = new Vector3(field.posX, 0, field.posZ) + FieldHandler.FIELD_OFFSET;
        Vector2 rasterPosition = GridMovement.GetGridPosition(currentPosition);

        int fieldSize = FieldHandler.FIELD_SIZE;
        if (rasterPosition == new Vector2(fieldSize - 1, fieldSize - 1))
            GameStateHandler.SetGameState(GameState.WON);
    }

    public void CreateExplosion()
    {
        bomb.GetComponent<BombScript>().destroy = true;
        Destroy(bomb, 1);
        Vector3 explosionMarkPos = new Vector3(transform.position.x, FieldHandler.FIELD_OFFSET.y + 0.01f, transform.position.z);
        Instantiate(PrefabExplosionMark, explosionMarkPos, Quaternion.Euler(-90,0,0));
        BlowUpChicken();
        AudioManager.GetInstance().PlaySound(Sound.EXPLOSION);

        
        Vector3 bombPos = new Vector3(transform.position.x, FieldHandler.FIELD_OFFSET.y + 0.01f, transform.position.z);
        Destroy(Instantiate(PrefabExplosion, bombPos, Quaternion.identity), 2);
    }

    private void BlowUpChicken()
    {
        GameObject chicken = GameManager.GetInstance().chickenObj;
        Vector3 bombToChickenDirection = Vector3.Normalize(chicken.transform.position - bomb.transform.position);
        float bombForce = 60f;
        float chickenBombDistance = Vector3.Magnitude(chicken.transform.position - bomb.transform.position);
        Vector3 forceToChicken = bombToChickenDirection * bombForce; // /chickenBombDistance
        chicken.GetComponent<Rigidbody>().AddForce(forceToChicken, ForceMode.Impulse);
        chicken.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
    }


    private void Update() {

        if (doFadeOut)
        {
            
            for (int i = 0; i < materialCount; i++)
            {
                Color newColor = renderer.materials[i].color;
                newColor.a = Mathf.Max(newColor.a - Time.deltaTime / fadeOutDuration, 0);
                renderer.materials[i].color = newColor;
                    //Color.Lerp(renderer.materials[i].color, endColors[i], fadingConstant * Time.deltaTime);
            }
            if (renderer.materials[0].color.a == 0)
            {
                Destroy(gameObject,1);
                return;
            }
        }
        
    }

    public Material[] fadematerials;
    Color[] startColors;
    Color[] endColors;

    void PrepareBushFadeOut()
    {
        if (doFadeOut)
            return;

        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        renderer.materials = fadematerials;
        startColors = isLightBush ? FieldHandler.GetInstance().bushLightColors : FieldHandler.GetInstance().bushDarkColors;
        endColors = new Color[startColors.Length];

        for (int i = 0; i < startColors.Length; i++)
        {
            endColors[i] = startColors[i];
            endColors[i].a = 0;
        }

        doFadeOut = true;
    }

    public void SetField(GameField field)
    {
        this.field = field;
    }

    public void SetBomb(GameObject bomb)
    {
        this.bomb = bomb;
    }
}