using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BlockUiState
{
    public Button btn;
    public Image img;
}

public class BlockUiPfb : MonoBehaviour
{
    public BlockUiState awailableState;

    public List<BlockUiState> AllStates => new(){awailableState};
}