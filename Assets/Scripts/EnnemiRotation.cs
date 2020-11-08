using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiRotation : MonoBehaviour
{
    void Start()
    {
        TurnManager.instance.nextTurnEvent.AddListener(OnNextTurnEvent);
    }

    void Update()
    {
        
    }

    void OnNextTurnEvent(TourPerso tour) {
        if(tour == TourPerso.Ennemi){
            Deplacer();
        }
    }

    void Deplacer(){
    }
}
