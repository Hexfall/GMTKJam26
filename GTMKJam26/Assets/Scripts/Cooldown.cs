using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public WeaponController weaponController;
    public Image chargeDial;
    public Image opportunityDial;


    void Update()
    {
        UpdateChargeDial();
        UpdateOpportunityDial();
    }


    void UpdateChargeDial()
    {
        chargeDial.fillAmount =
            weaponController.IsCharging
                ? weaponController.ChargeProgress
                : 0;
    }


    void UpdateOpportunityDial()
    {
        opportunityDial.gameObject.SetActive(
            weaponController.IsOpportunityWindowActive &&
            !weaponController.IsOpportunityWindowPaused
        );


        opportunityDial.fillAmount =
            weaponController.OpportunityProgress;
    }
}
