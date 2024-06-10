using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    [HideInInspector]
    public float min;
    [HideInInspector]
    public float max;

    private float _current;

    public float current
    {   get { return _current; }
        set { _current = value; UpdateFill(); }
    } 

    public Image mask;
    public Image maskBG;

    /*
#if UNITY_EDITOR
    void Update()
    {
        UpdateFill();
    }
#endif
*/
    private void UpdateFill()
    {
        float currentOffset = _current - min;
        float maxOffset = max - min;
        float fillAmount = currentOffset / maxOffset;
        mask.fillAmount = fillAmount;
        if (maskBG != null) 
            maskBG.fillAmount = fillAmount;
    }
}
