﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class EnnemiAleatoire : MonoBehaviour
{
    [SerializeField] float _vitesse;
    [SerializeField] Tilemap _grid;
    [SerializeField] Tilemap _collision;
    [SerializeField] int _nbMouvement;

    private int _mouvementCourant;

    void Start()
    {
        TurnManager.instance.nextTurnEvent.AddListener(OnNextTurnEvent);
        _mouvementCourant = _nbMouvement;
        transform.position = CentrerPositionGrille(transform.position);
    }

    void Update()
    {

    }

    void OnNextTurnEvent(TourPerso tour){
        if(tour == TourPerso.Ennemi){
            StartCoroutine("Deplacement");
        }
    }

    IEnumerator Deplacement(){
        while(_mouvementCourant > 0){
            List<OrientationHero> orientationValide = OrientationValide();
            OrientationHero orientation = OrientationAleatoire(orientationValide);
            Vector3Int targetPos = ObtenirTargetPosition(_grid.WorldToCell(transform.position), orientation);
            Vector3 destinationPos = CentrerPositionGrille(_grid.CellToWorld(targetPos));

            float vitesse = _vitesse * Time.deltaTime;
            float _distanceRestante = DistanceRestanteCalcul(destinationPos);

            while(_distanceRestante > float.Epsilon){
                transform.position = Vector3.MoveTowards(transform.position,destinationPos, vitesse);
                _distanceRestante = DistanceRestanteCalcul(destinationPos);
                yield return null;
            }

            _mouvementCourant = _mouvementCourant - 1;
        }

        _mouvementCourant = _nbMouvement;
        TurnManager.instance.CompleterTour(TourPerso.Ennemi);
    }

    List<OrientationHero> OrientationValide(){
        Vector3Int ennemiPos = _grid.WorldToCell(transform.position);
        List<OrientationHero> liste = new List<OrientationHero>();

        if(ValiderPosition(ennemiPos, OrientationHero.NE)) {
            liste.Add(OrientationHero.NE);
        }
        if(ValiderPosition(ennemiPos, OrientationHero.NW)) {
            liste.Add(OrientationHero.NW);
        }
        if(ValiderPosition(ennemiPos, OrientationHero.SE)) {
            liste.Add(OrientationHero.SE);
        }
        if(ValiderPosition(ennemiPos, OrientationHero.SW)) {
            liste.Add(OrientationHero.SW);
        }

        return liste;
    }

    OrientationHero OrientationAleatoire(List<OrientationHero> orientation){
        int numero = Random.Range(0, orientation.Count);
        return orientation[numero];
    }

    Vector3Int ObtenirTargetPosition(Vector3Int position, OrientationHero orientation){
        switch (orientation)
        {
            case OrientationHero.NE:
                return new Vector3Int(position.x + 1, position.y, 0);
            break;
            case OrientationHero.NW:
                return new Vector3Int(position.x, position.y + 1, 0);
            break;
            case OrientationHero.SE:
                return new Vector3Int(position.x, position.y - 1, 0);
            break;
            case OrientationHero.SW:
                return new Vector3Int(position.x - 1, position.y, 0);
            break;
            default:
                return position;
        }
    }

    bool ValiderPosition(Vector3Int position, OrientationHero orientation){
        Vector3Int _targetPosCollision = ObtenirTargetPosition(position, orientation);
        return !_collision.HasTile(_targetPosCollision);
    }

    // calcul la distance restante entre deux positions
    float DistanceRestanteCalcul(Vector3 destination){return (transform.position - destination).sqrMagnitude;}

    Vector3 CentrerPositionGrille(Vector3 position){
        Vector3Int cellPosition = _grid.WorldToCell(position);
        return _grid.GetCellCenterWorld(cellPosition);
    }
}

