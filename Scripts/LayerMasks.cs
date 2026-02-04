
using UnityEngine;

public class LayerMasks : MonoBehaviour
{
    public LayerMask _uiMask;
    public LayerMask _worldMask;

    public static LayerMask uiMask;
    public static LayerMask worldMask;

    public void Init()
    {
        uiMask = _uiMask;
        worldMask = _worldMask;
    }  
}

public static class LayerMasksUtils
{
    public static bool ContainsLayer(LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) > 0;       
    }
}