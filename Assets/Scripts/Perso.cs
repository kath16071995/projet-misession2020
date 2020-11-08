using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.UI;

public class Perso : MonoBehaviour
{
    [SerializeField] float _vitesse;
    [SerializeField] Tilemap _grid;
    [SerializeField] Tilemap _collision;
    [SerializeField] int _nbMouvement;
    [SerializeField] ScriptableObject _nbVie;
    [SerializeField] string _prochaineScene;

    private Camera _cam;
    private Animator _animation;

    private Vector3 _mousePos;

    private Color _originalCurrentColor;
    private Color _originalDestinationColor;
    
    private Vector3Int _currentPos;
    private Vector3Int _destinationPos;

    private bool _monTour = false;
    private bool _seDeplace = false;

    public Text _viesText;
    public Text _mouvementsText;
    
    void Start()
    {
        _cam = Camera.main;
        _mousePos = new Vector3();
        _currentPos = _grid.WorldToCell(transform.position);
        transform.position = CentrerPositionGrille(transform.position);

        Debug.Log(Vie.VieRestante);
        TurnManager.instance.nextTurnEvent.AddListener(OnNextTurnEvent);

        _animation = GetComponent<Animator>();
        _animation.SetBool("isRunning", false);

        _viesText.text = "" + Vie.VieRestante;
        _mouvementsText.text = "" + _nbMouvement;
    }

    
    void Update()
    {
        Vector2 position =  _cam.ScreenToWorldPoint(Input.mousePosition);
        _mousePos = position;

        if(_nbMouvement != 0 && _monTour && !_seDeplace){
            if(Input.GetMouseButtonUp(0) && _mousePos!=_currentPos){
                Mouvement();
            } 
        }
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
                _seDeplace = true;
                EnleverMouvement(_initPos, _destinationPos);
                // on recupere les couleurs initiales des tuiles dans une propriete et on les changent pour deux couleurs préchoisis
                _originalDestinationColor = _grid.GetColor(_destinationPos);
                AssignerCouleurTuile(_destinationPos, Color.red);
                _originalCurrentColor = _grid.GetColor(_currentPos);
                AssignerCouleurTuile(_currentPos, Color.blue);
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
        // _destinationPos = GetMaxedDistanceCell(_grid.WorldToCell(transform.position), _targetPos, _nbMouvement, _grid, _collision, Axis.X);
        // Debug.Log(_initPos);
        // Debug.Log(_targetPos);
        // Debug.Log(_destinationPos);

        

        float Vitesse = _vitesse * Time.deltaTime;
        float _distanceRestante = DistanceRestanteCalcul(_destinationPos);

        _animation.SetBool("isRunning", true);

        while(_distanceRestante > float.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position,_destinationPos, Vitesse);
            _distanceRestante = DistanceRestanteCalcul(_destinationPos);
            yield return null;
        }

        yield return new WaitForSeconds (0.25f);
        AssignerCouleurTuile(_currentPos, _originalCurrentColor);
        AssignerCouleurTuile(_targetPos, _originalDestinationColor);

        _mouvementsText.text = "" + _nbMouvement;
        _animation.SetBool("isRunning",false);
        _seDeplace = false;
        yield return null;
    }


    // centre le hero au milieu de la cellule
    Vector3 CentrerPositionGrille(Vector3 position){
        Vector3Int cellPosition = _grid.WorldToCell(position);
        return _grid.GetCellCenterWorld(cellPosition);
    }


    // calcul la distance restante entre deux positions
    float DistanceRestanteCalcul(Vector3 destination){return (transform.position - destination).sqrMagnitude;}
    

    // change la couleur de la cellule
    void AssignerCouleurTuile(Vector3Int tuile, Color couleur){
        _grid.SetTileFlags(tuile, TileFlags.None);
        _grid.SetColor(tuile, couleur);
    }

    void OnNextTurnEvent(TourPerso tour){
        if(tour == TourPerso.Hero){
            _monTour = true;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ennemi1")
        {
            Vie.EnleverVies();
            _viesText.text = "" + Vie.VieRestante;
            Debug.Log(Vie.VieRestante);
            if(Vie.VieRestante<0){
                //son de mort
                GameManager.ChangerScene("Start");
            }else{
                //son de collision
                GameManager.ChangerScene("Level 1");
            }
        }

        if (collision.gameObject.tag == "Portal"){
            GameManager.ChangerScene(_prochaineScene);
        }
    }

    public void TerminerTour(){
        TurnManager.instance.CompleterTour(TourPerso.Hero);
        this._monTour = false;
    }

    void EnleverMouvement(Vector3Int posCourant, Vector3Int posDestination){
        var x = posDestination.x - posCourant.x;
        var y = posDestination.y - posCourant.y;

        _nbMouvement = _nbMouvement - (Mathf.Abs(x) + Mathf.Abs(y));
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

public enum OrientationHero{
    N,
    NE,
    NW,
    S,
    SE,
    SW,
    E,
    W
}


public enum Axis {
    X,Y
}
