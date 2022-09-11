using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class CalendarBoostActivator : MonoBehaviour
{
    private Decade currentDecade;

    public void DeactivateOldBoost()
    {
        if(currentDecade != null) HandlingBoost(false, currentDecade, BoostSender.Calendar);
    }

    public void ActivateBoost(Decade decade)
    {
        DeactivateOldBoost();

        currentDecade = decade;
        HandlingBoost(true, currentDecade, BoostSender.Calendar);
    }

    public void HandlingBoost(bool isActivation, Decade decade, BoostSender boostSender)
    {
        //do smth
    }


}
