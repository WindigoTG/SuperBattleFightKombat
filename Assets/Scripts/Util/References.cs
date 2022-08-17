using UnityEngine;

public class References
{
    private static float _inputThreshold = 0.1f;
    public static float InputThreshold => _inputThreshold;

    private static float _fallThreshold = -1f;
    public static float FallThreshold => _fallThreshold;

    private static Vector3 _leftScale = new Vector3(-1, 1, 1);
    private static Vector3 _rightScale = new Vector3(1, 1, 1);
    public static Vector3 LeftScale => _leftScale;
    public static Vector3 RightScale => _rightScale;
}
