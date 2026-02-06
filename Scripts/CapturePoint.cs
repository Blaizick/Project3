using System;
using System.Collections.Generic;
using System.Text;
using BJect;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CmsCaptureTimeComp : CmsComp
{
    public float captureTime;
}

public class CapturePoint : MonoBehaviour
{
    [NonSerialized] public CmsEnt cmsEnt;
    
    [NonSerialized] public float progress;

    [NonSerialized, Inject] public ResourcesSystem resources;

    public TeamComp teamComp;

    public Image awailableFillerImg;
    public Image capturedFillerImg;
    public TooltipTrigger tooltipTrigger;

    [NonSerialized] public Castle captureCastle;

    public GameObject capturedState;
    public GameObject capturedStateUi;
    public GameObject awailableState;
    public GameObject awailableStateUi;

    public virtual void Init()
    {
        
    }

    public virtual void Update()
    {
        tooltipTrigger.title = cmsEnt.Get<CmsNameComp>().name;
        
        StringBuilder sb = new();
        sb.AppendLine(cmsEnt.Get<CmsDescComp>().desc);
        sb.AppendLine();
        sb.AppendLine("Resources:");
        foreach (var i in cmsEnt.GetAll<CmsResourceStackComp>())
        {
            sb.AppendLine(i.AsStack().ToString());
        }
        sb.AppendLine();
        if (progress > 1.0f)
        {
            sb.AppendLine("This point is captured!");
        }
        else
        {
            sb.AppendLine($"Capture time: {(int)(progress * cmsEnt.Get<CmsCaptureTimeComp>().captureTime)}/{(int)cmsEnt.Get<CmsCaptureTimeComp>().captureTime}");
        }
        tooltipTrigger.desc = sb.ToString();

        if (captureCastle && progress < 1.0f)
        {
            var cp = Time.deltaTime / cmsEnt.Get<CmsCaptureTimeComp>().captureTime;
        
            foreach (var r in cmsEnt.GetAll<CmsResourceStackComp>())
            {
                resources.ForTeam(teamComp.team).Add(new(r.resource, r.count * cp));
            }
        
            progress += cp;
        }

        capturedFillerImg.fillAmount = progress;
        awailableFillerImg.fillAmount = progress;

        capturedState.SetActive(progress > 1.0f);
        awailableState.SetActive(progress <= 1.0f);
        capturedStateUi.SetActive(progress > 1.0f);
        awailableStateUi.SetActive(progress <= 1.0f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Castle>(out var c) &&
            c.teamComp.team.IsAlly(teamComp.team))
        {
            captureCastle = c;
        }        
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Castle>(out var c) &&
            c.teamComp.team.IsAlly(teamComp.team))
        {
            captureCastle = null;
        }
    }
}

[Serializable]
public class CmsSpawnCapturePointComp : CmsComp
{
    public int count;
    public CmsEnt capturePoint;
}

public class CapturePointSystem
{
    public CmsEnt profile;

    public DiContainer container;
    public MapSystem map;

    public CapturePointSystem(DiContainer container, MapSystem map)
    {
        this.container = container;
        this.map = map;
    }

    public void Init()
    {
        profile = Profiles.capturePoints;

        foreach (var i in profile.GetAll<CmsSpawnCapturePointComp>())
        {
            for (int j = 0; j < i.count; j++)
            {
                var scr = container.Instantiate(i.capturePoint.Get<CmsCapturePointPfbComp>().pfb);
                scr.cmsEnt = i.capturePoint;
                scr.transform.position = map.GetRandomPointInBounds();
                scr.teamComp.Set(Teams.Get(profile.Get<CmsTeamComp>().team));
                scr.Init();
            }
        }   
    }
}

[Serializable]
public class CmsCapturePointPfbComp : CmsComp
{
    public CapturePoint pfb;
}

public class CmsDamageBonusComp : CmsComp
{
    public float damageBonus;
}