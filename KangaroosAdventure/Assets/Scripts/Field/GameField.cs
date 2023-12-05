public class GameField
{
    public float posX;
    public float posZ;
    public FieldType type;
    public bool notViewableByCamera;
    public bool alreadySet;

    public GameField(float posX, float posZ)
    {
        this.posX = posX;
        this.posZ = posZ;
        type = FieldType.BUSH;
    }
}