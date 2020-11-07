using System.Collections;
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

    void Start()
    {
        TurnManager.instance.nextTurnEvent.AddListener(OnNextTurnEvent);
    }

    void Update()
    {

    }

    void OnNextTurnEvent(TourPerso tour){
        if(tour == TourPerso.Ennemi){
            Deplacer();
        }
    }

    void Deplacer(){
        Vector3Int initPos = _grid.WorldToCell(transform.position);
        OrientationHero orientation = OrientationAleatoire();
        Vector3Int targetPos = ObtenirTargetPosition(initPos, orientation);


        bool valide = ValiderPosition(initPos, orientation);

        _grid.SetTileFlags(targetPos, TileFlags.None);
        _grid.SetColor(targetPos, Color.red);

        Debug.Log(orientation);
        Debug.Log(valide);
    }

    OrientationHero OrientationAleatoire(){
        int numero = Random.Range(0,4);

        switch (numero)
        {
            case 0:
                return OrientationHero.NE;
            break;
            case 1:
                return OrientationHero.NW;
            break;
            case 2:
                return OrientationHero.SE;
            break;
            case 3:
                return OrientationHero.SW;
            break;
            default:
                return OrientationHero.NE;
        }
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
        Vector3Int _targetPosCollision = new Vector3Int(0, 0, 0);

        switch (orientation)
        {
            case OrientationHero.N:
                _targetPosCollision = _grid.WorldToCell(new Vector3(position.x + 1, position.y + 1, 0));
            break;
            case OrientationHero.E:
                _targetPosCollision = _grid.WorldToCell(new Vector3(position.x + 1, position.y - 1, 0));
            break;
            case OrientationHero.S:
                _targetPosCollision = _grid.WorldToCell(new Vector3(position.x - 1, position.y + 1, 0));
            break;
            case OrientationHero.W:
                _targetPosCollision = _grid.WorldToCell(new Vector3(position.x - 1, position.y - 1, 0));
            break;
        }

        return !_collision.HasTile(_targetPosCollision);
    }
}

