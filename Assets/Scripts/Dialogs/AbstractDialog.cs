using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IconManager;

public abstract class AbstractDialog : MonoBehaviour
{
    public abstract void SelectFirst();
    public abstract void CloseDialog();
    public abstract void SetIcons(InteractionIcons icons);
}
