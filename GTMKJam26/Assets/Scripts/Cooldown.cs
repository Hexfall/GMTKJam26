using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public float time = 2.0f;
    private float _since_start = 0.0f;
    private float _progress = 0.0f;
    public Image dial;
    
    void Update()
    {
        _since_start += Time.deltaTime;

        UpdateDial();
    }

    void UpdateDial()
    {
        _progress = _since_start / time;
        dial.fillAmount = _progress;
    }
}
