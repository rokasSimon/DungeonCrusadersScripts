using UnityEngine;

public static class RandomRotation
{
    private static readonly float[] _rotations = new[]
    {
        0f, 90f, 180f, 270f
    };

    public static float Value => _rotations[Random.Range(0, _rotations.Length)];
    public static Quaternion Quaternion => Quaternion.Euler(new Vector3(0f, Value, 0f));
    public static Vector3 Vector => new(0f, Value, 0f);
}