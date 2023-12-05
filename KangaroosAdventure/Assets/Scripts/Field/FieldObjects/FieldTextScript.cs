using UnityEngine;

public class FieldTextScript : MonoBehaviour
{
    public GameObject[] PrefabTexts;
    private static Vector3 textFieldOffset = new Vector3(0.313f, 0, -0.287f);


    public void CreateTextOnGround()
    {
        Vector3 currentPos = transform.position;
        Vector2 currentRasterPos = GridMovement.GetGridPosition(currentPos);
        int bombCount = FieldHandler.GetInstance().GetAllNeighbourFieldsOfType(currentRasterPos, FieldType.BOMB).Count;
        GameObject textObj = GetMatchingTextObjectForBombCount(bombCount);
        Vector3 textPos = new Vector3(currentPos.x, 0.01f + FieldHandler.FIELD_OFFSET.y, currentPos.z) + textFieldOffset;
        Quaternion textRot = Quaternion.Euler(new Vector3(90, 0, 0));

        GameObject groundText = Instantiate(textObj, textPos, textRot);

        groundText.GetComponent<TMPro.TextMeshPro>().text = bombCount + "";
    }


    private GameObject GetMatchingTextObjectForBombCount(int bombCount)
    {
        GameManager gameManager = GameManager.GetInstance();
        if (bombCount == 0)
            return PrefabTexts[0]; // DarkGreen
        if (bombCount <= 2)
            return PrefabTexts[1]; // Green
        if (bombCount <= 4)
            return PrefabTexts[2]; // Yellow
        else
            return PrefabTexts[3]; // Red
    }
}
