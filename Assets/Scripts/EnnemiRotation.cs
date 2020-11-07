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
        // var temp = new Vector3(transform.position.x + 8, transform.position.y, 0);
        // transform.position = temp;
    }
}
