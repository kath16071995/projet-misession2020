using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class Ennemi : MonoBehaviour
{
    [SerializeField] float _vitesse;
    [SerializeField] Tilemap _grid;
    [SerializeField] Tilemap _collision;

    private Camera _cam;
    private Animator _animation;
    
    private static Vector3Int _currentPos;
    private Vector3Int _destinationPos;

    private Vector3 _mousePos;

    private bool _monTour = false;


    public static Vector3Int PositionActuel{
        get{return _currentPos;}
    }
     void Start()
    {
        _cam = Camera.main;
        _mousePos = new Vector3();
        _currentPos = _grid.WorldToCell(transform.position);
        transform.position = CentrerPositionGrille(transform.position);
        //TurnManager.instance.nextTurnEvent.AddListener(OnNextTurnEvent);

        _animation = GetComponent<Animator>();
    }

    
    void Update()
    {
        Vector2 position =  _cam.ScreenToWorldPoint(Input.mousePosition);
        _mousePos = position;
        _currentPos = _grid.WorldToCell(transform.position);
    }

    // sert a verifier les conditions plus complexe pour le deplacement
    void Mouvement(){ 
        
        // tansforme les positions world en position de cellule sur la grid
        Vector3Int _destinationPos = _grid.WorldToCell(_mousePos);
        Vector3Int _initPos = _grid.WorldToCell(transform.position);

        Vector3Int collisionPosition = _collision.WorldToCell(_mousePos);
        _currentPos = _grid.WorldToCell(transform.position);

        // on centre les positions au centre de leur cellule
        Vector3 _centerMouse = CentrerPositionGrille(_destinationPos);
        Vector3 _centerPerso = CentrerPositionGrille(_initPos);

        

        // on verifie si on clique sur une cellule de la grid et non une cellule de collision, et ensuit on verifie si elle est sur la meme axe des X ou Y que le hero
        if(!_collision.HasTile(collisionPosition)){
            if(_grid.HasTile(_destinationPos) && (_centerMouse.x==_centerPerso.x || _centerMouse.y == _centerPerso.y)){
               StartCoroutine("Deplacement");
            }
        }else{
            Debug.Log("Collision!");
        }

    }


    // coroutine de deplacement
    // REVOIR LA VIDEO SUR LES COROUTINES FAIT PAR LE PROF
    IEnumerator Deplacement(){

        Vector3Int _initPos = _grid.WorldToCell(transform.position);
        Vector3Int _targetPos = _grid.WorldToCell(_mousePos);



        Vector3 _destinationPos = CentrerPositionGrille(_mousePos);

        float Vitesse = _vitesse * Time.deltaTime;
        float _distanceRestante = DistanceRestanteCalcul(_destinationPos);

        while(_distanceRestante > float.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position,_destinationPos, Vitesse);
            _distanceRestante = DistanceRestanteCalcul(_destinationPos);
            yield return null;
        }
        yield return null;
    }


    // centre le hero au milieu de la cellule
    Vector3 CentrerPositionGrille(Vector3 position){
        Vector3Int cellPosition = _grid.WorldToCell(position);
        return _grid.GetCellCenterWorld(cellPosition);
    }


    // calcul la distance restante entre deux positions
    float DistanceRestanteCalcul(Vector3 destination){return (transform.position - destination).sqrMagnitude;}
    

    void OnNextTurnEvent(TourPerso tour){
        Debug.Log("Mon TOur");
        if(tour == TourPerso.Hero){
            _monTour = true;
        }
    }


    /// <summary>
    /// Réduit la distance à parcourir en fonction des unités de déplacement disponibles et des collisions
    /// rencontrées sur le chemin (en appelant GetMaxedNoCollisionCell).
    /// Retourne de nouvelles coordonnées grid limitées. La fonction ne réduit pas le nombre d'unités disponibles (à faire manuellement).
    /// </summary>
    /// <param name="currentPos">Coordonnées grid du personnage</param>
    /// <param name="gridDestination">Coordonnées grid de la destination (UNE FOIS VALIDÉE)</param>
    /// <param name="unitsLeft">Nombre d'unités restantes pour se déplacer</param>
    /// <param name="groundMap">Tilemap ground</param>
    /// <param name="collisionMap">Tilemap de collision</param>
    /// <param name="axe">Valeur du enum à passer selon l'axe sur lequel on désire se déplacer</param>
    /// <returns>Nouvelles coordonnées grid à atteindre une fois les collisions/trous et unités considérés.</returns>
    Vector3Int GetMaxedDistanceCell(Vector3Int currentPos, Vector3Int gridDestination, int unitsLeft, Tilemap groundMap, Tilemap collisionMap, Axis axe) {        
        // Récupération des valeurs en X ou en Y
        int fromVal = currentPos[(int) axe];
        int toVal = gridDestination[(int)axe];
        int difference = toVal - fromVal;
        // Calcul de la direction en fonction de la polarité de la différence entre les deux valeurs
        int direction = (int)Mathf.Sign(difference);  
        // Limitation de la différence au nombre d'unités disponibles
        gridDestination[(int) axe]  = fromVal + direction * Mathf.Min(Mathf.Abs(difference), unitsLeft);
        
        return GetMaxedNoCollisionCell(currentPos, gridDestination, direction, groundMap, collisionMap, axe);
    }




    /// <summary>
    /// Limite le déplacement prévu à la distance avant collision en prenant chaque cellule entre la position actuelle
    /// et la destination. Si une collision est rencontrée, on utilise la dernière cellule validée.
    /// Déjà appelée par GetMaxedDistanceCell.
    /// </summary>
    Vector3Int GetMaxedNoCollisionCell(Vector3Int fromCell, Vector3Int toCell, int direction, Tilemap groundMap, Tilemap collisionMap, Axis axe) {
        int nextValX = fromCell.x;
        int nextValY = fromCell.y;

        Vector3Int newCellPos = new Vector3Int(nextValX, nextValY, toCell.z);
        
        // Utilisé afin d'éviter les boucles sans fin en cas d'erreur
        int sentinel = 1;
        do {
            int lastValX = nextValX;
            int lastValY = nextValY;

            if (axe == Axis.X) {
                // récupération de la prochaine tuile en X
                nextValX = nextValX + direction;
                newCellPos.x = nextValX;
            } else if (axe == Axis.Y) {
                // récupération de la prochaine tuile en Y
                nextValY = nextValY + direction;
                newCellPos.y = nextValY;
            }

            // détection d'une collision ou d'un trou
            if(collisionMap.HasTile(newCellPos) || !groundMap.HasTile(newCellPos)) {
                // la dernière tuile valide est retournée
                return new Vector3Int(lastValX, lastValY, toCell.z);
            }
            sentinel++;
        } while (newCellPos != toCell && sentinel < 500);

        // aucune collision ni trou rencontrés sur le chemin
        return newCellPos;
    }
}

