namespace Control
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController player);
        CursorType GetCursorType();
    }
}