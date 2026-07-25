using UnityEngine;
using UnityEngine.UI;

public class Cooldown : MonoBehaviour
{
    public WeaponController weaponController;

    [Header("Normal Cooldown")]
    public Image dial;

    [Header("Opportunity Window")]
    public Image opportunityDial;


    void Update()
    {
        UpdateDial();
        UpdateOpportunityDial();
    }


    private void UpdateDial()
    {
        if(weaponController.IsCharging)
        {
            dial.gameObject.SetActive(true);

            dial.fillAmount =
                weaponController.ChargeProgress;

            return;
        }


        if(weaponController.IsFastReloading)
        {
            dial.gameObject.SetActive(true);

            dial.fillAmount =
                weaponController.FastReloadProgress;

            return;
        }

        if(weaponController.IsOpportunityWindowActive &&
           !weaponController.IsOpportunityWindowPaused)
            return;

        dial.gameObject.SetActive(false);
    }



    private void UpdateOpportunityDial()
    {
        if(weaponController.IsOpportunityWindowActive &&
           !weaponController.IsOpportunityWindowPaused)
        {
            opportunityDial.gameObject.SetActive(true);

            opportunityDial.fillAmount =
                1 - weaponController.OpportunityProgress;

            return;
        }


        opportunityDial.gameObject.SetActive(false);
    }
}