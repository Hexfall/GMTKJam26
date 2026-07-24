using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public WeaponController weaponController;
    public Image dial;
    
    void Update()
    {
        UpdateDial();
    }

    void UpdateDial()
    {
        dial.fillAmount = weaponController.IsCharging ? weaponController.ChargeProgress : 0.0f;
    }
}
