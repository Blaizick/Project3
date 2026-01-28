using UnityEngine;

public static class MathUtils
{
    public static float GetLookAtDegrees(in Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
    public static float GetLookAtDegrees(in Vector2 a, in Vector2 b)
    {
        return GetLookAtDegrees(b - a);
    }
    public static Quaternion GetLookAtRotation(in Vector2 dir)
    {
        return Quaternion.Euler(0f, 0f, GetLookAtDegrees(dir));
    }
    public static Quaternion GetLookAtRotation(in Vector2 a, in Vector2 b)
    {
        return GetLookAtRotation(b - a);
    }

    public static Quaternion OffsetRotation(in Quaternion quaternion, float d)
    {
        return Quaternion.Euler(0, 0, quaternion.eulerAngles.z + d);
    }

    public static bool IsWithin(in Vector2 a, in Vector2 b, float kEpsilon = 0.01f)
    {
        return Vector2.Distance(a, b) < kEpsilon;
    }

    public static Vector2 RotateDirection(in Vector2 dir, float delta)
    {
        float rad = Mathf.Deg2Rad * delta;
        
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        Vector2 a = new Vector2(cos, sin);
        Vector2 b = new Vector2(-sin, cos);

        return dir.x * a + dir.y * b;
    }
    public static Vector2 RotateAround(in Vector2 point, in Vector2 anchor, float delta)
    {
        return anchor + RotateDirection(point - anchor, delta);
    }
}