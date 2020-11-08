using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text _viesText;

    void Start(){
        _viesText.text = "You have " + Vie.VieRestante + " lives left";
    }
}
