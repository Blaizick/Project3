using UnityEngine;

public class MapSystem : MonoBehaviour
{
    public BoxCollider2D col;

    public float Width => col.size.x;
    public float Height => col.size.y;
    public Vector2 Size => col.size;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Size);
    }

    public Vector2 GetRandomPointInBounds()
    {
        Vector2 hs = Size * 0.5f;
        Vector2 min = (Vector2)transform.position - hs;
        Vector2 max = (Vector2)transform.position + hs;

        return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
    }

    public bool IsPositionInBounds(Vector2 pos)
    {
        Vector2 hs = Size * 0.5f;
        Vector2 min = (Vector2)transform.position - hs;
        Vector2 max = (Vector2)transform.position + hs;

        return pos.x > min.x && pos.y > min.y && pos.x < max.x && pos.y < max.y;
    }
    
    public Vector2 NormalizePos(Vector2 pos)
    {
        return (pos + (Size * 0.5f)) / Size;
    }
}
