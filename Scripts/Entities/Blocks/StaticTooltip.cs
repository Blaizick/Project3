using TMPro;
using UnityEngine;

public class StaticTooltip : MonoBehaviour
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

    public void Show(string title, string desc)
    {
        Set(title, desc);
        root.SetActive(true);
    }
    public void Hide()
    {
        root.SetActive(false);
    }
}