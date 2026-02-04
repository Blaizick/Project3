using UnityEngine;

public class MapSystem : MonoBehaviour
{
    public Vector2 size;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, size);
    }

    public Vector2 GetRandomPointInBounds()
    {
        Vector2 hs = size * 0.5f;
        Vector2 min = (Vector2)transform.position - hs;
        Vector2 max = (Vector2)transform.position + hs;

        return new Vector2(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y));
    }
}
