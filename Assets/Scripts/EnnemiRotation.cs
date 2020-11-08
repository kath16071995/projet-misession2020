using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class EnnemiRotation : MonoBehaviour
{
    [SerializeField] float _vitesse;
    [SerializeField] Tilemap _grid;
    [SerializeField] Tilemap _collision;
    [SerializeField] int _nbMouvement;
    [SerializeField] List<Vector3> _deplacements;

    private int _mouvementCourant;

    void Start()
    {
        TurnManager.instance.nextTurnEvent.AddListener(OnNextTurnEvent);
        _mouvementCourant = _nbMouvement;
        transform.position = CentrerPositionGrille(transform.position);
    }

    // sert a verifier si cest sont tour
    void OnNextTurnEvent(TourPerso tour){
        if(tour == TourPerso.Ennemi){
            StartCoroutine("Deplacement");
        }
    }


    // coroutine pour le deplacement aletoire
    IEnumerator Deplacement(){

        // verifie si il lui reste des mouvements, si oui il choisi une position aleatoir

        // TODO: au lieu d'utiliser le nombre de move possible. Utiliser une liste de mouvements possibles
        while(_mouvementCourant > 0){
            // Vector3 destinationPos = CentrerPositionGrille(_grid.CellToWorld(targetPos));

            // float vitesse = _vitesse * Time.deltaTime;
            // float _distanceRestante = DistanceRestanteCalcul(destinationPos);

            // // bouge le personnage
            // while(_distanceRestante > float.Epsilon){
            //     transform.position = Vector3.MoveTowards(transform.position,destinationPos, vitesse);
            //     _distanceRestante = DistanceRestanteCalcul(destinationPos);
            //     yield return null;
            // }

            // _mouvementCourant = _mouvementCourant - 1;
        }

        _mouvementCourant = _nbMouvement;
        TurnManager.instance.CompleterTour(TourPerso.Ennemi); //termine le tour ennemi
        yield return null;
    }


    //verifie si la tuile est valide
    // bool ValiderPosition(Vector3Int position, OrientationHero orientation){
    //     Vector3Int _targetPosCollision = ObtenirTargetPosition(position, orientation);
    //     return !_collision.HasTile(_targetPosCollision);
    // }

    // calcul la distance restante entre deux positions
    float DistanceRestanteCalcul(Vector3 destination){return (transform.position - destination).sqrMagnitude;}

    //centre dans la tuile
    Vector3 CentrerPositionGrille(Vector3 position){
        Vector3Int cellPosition = _grid.WorldToCell(position);
        return _grid.GetCellCenterWorld(cellPosition);
    }
}

