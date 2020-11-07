using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Vie : ScriptableObject
{

    public static int _vies;
    [SerializeField] int _viesInit;

    public static int VieRestante{
        get{return _vies;}
    }

    void Start(){
        _vies = _viesInit;
    }

    public static int EnleverVies(){
        _vies = _vies - 1;
        return _vies;
        }
}
