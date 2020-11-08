using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class NextTurnEvent: UnityEvent<TourPerso>{}
public class TurnManager : MonoBehaviour
{

    public NextTurnEvent nextTurnEvent;
    public Button nextTurnButton;

    public static TurnManager instance;

    void Awake(){
        instance = this;
    }

    void Start()
    {
        nextTurnEvent.Invoke(TourPerso.Hero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleterTour(TourPerso tourCompleter){
        switch(tourCompleter) {
            case TourPerso.Hero:
                nextTurnButton.interactable = false;
                nextTurnEvent.Invoke(TourPerso.Ennemi);
            break;
            case TourPerso.Ennemi:
                nextTurnButton.interactable = true;
                nextTurnEvent.Invoke(TourPerso.Hero);
            break;
        }
    }
}

public enum TourPerso{
    Hero,
    Ennemi
}