using System;
using BJect;
using UnityEngine;

public class FogOfWarSystem
{
    [NonSerialized] public Sprite sprite;
    [NonSerialized] public CmsEnt profile;
    [Inject, NonSerialized] public MapSystem map;

    public CmsHiddenColComp hiddenColComp;
    public CmsRevealedColComp revealedColComp;

    public void Init()
    {
        profile = Profiles.fogOfWar;

        sprite = profile.Get<CmsSpriteComp>().sprite;

        revealedColComp = profile.Get<CmsRevealedColComp>();
        hiddenColComp = profile.Get<CmsHiddenColComp>();

        HideEverything();
    }

    public void HideEverything()
    {
        Color[] colors = new Color[sprite.texture.width * sprite.texture.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = hiddenColComp.hiddenCol;
        }
        sprite.texture.SetPixels(colors);
        sprite.texture.Apply();
    }

    public void RevealWorld(Vector2 pos, float radius)
    {
        Reveal(map.NormalizePos(pos), radius / map.Width);
    }

    /// <summary>
    /// Normalized pos(0 - 1)
    /// </summary>
    public void Reveal(Vector2 pos, float radius)
    {
        var tex = sprite.texture;

        Vector2Int centerPix = new Vector2Int((int)(pos.x * tex.width), (int)(pos.y * tex.height));
        
        int pixRadius = (int)(radius * tex.width);
        
        for (int cx = -pixRadius; cx < pixRadius; cx++)
        {
            for (int cy = -pixRadius; cy < pixRadius; cy++)
            {
                int x = centerPix.x + cx;
                int y = centerPix.y + cy;

                if (Vector2Int.Distance(centerPix, new Vector2Int(x, y)) <= pixRadius && 
                    x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                {
                    tex.SetPixel(x, y, revealedColComp.revealedCol);
                }
            }
        }

        tex.Apply();
    }    
}

[Serializable]
public class CmsRevealedColComp : CmsComp
{
    public Color revealedCol;
}

[Serializable]
public class CmsHiddenColComp : CmsComp
{
    public Color hiddenCol;
}