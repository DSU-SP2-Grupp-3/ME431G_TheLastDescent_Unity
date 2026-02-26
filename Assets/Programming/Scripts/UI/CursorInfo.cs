using UnityEngine;

[CreateAssetMenu(fileName = "NewCursorInfo", menuName = "UI/Cursor", order = 0)]
public class CursorInfo : ScriptableObject
{
    public Texture2D texture;
    public Vector2 hotSpot;
    public CursorMode cursorMode = CursorMode.Auto;
}