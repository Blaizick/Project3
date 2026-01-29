using TMPro;
using UnityEngine;

public class BlockTooltipUi : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descText;

    public GameObject root;

    public void Init()
    {
        root.SetActive(false);
    }

    public void Set(string title, string desc)
    {
        titleText.text = title;
        descText.text = desc;
    }
}