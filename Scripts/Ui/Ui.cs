using BJect;
using TMPro;
using UnityEngine;

public class Ui : MonoBehaviour
{
    public BlocksUi blocksUi;

    public TMP_Text essenceText;

    [Inject] public ResourcesSystem resources;

    public void Init()
    {
        blocksUi.Init();
    }

    public void Update()
    {
        essenceText.text = ((int)resources.Get(CmsResources.essence)).ToString();
    }
}