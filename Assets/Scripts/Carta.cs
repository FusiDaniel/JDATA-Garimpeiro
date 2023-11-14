using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour
{
    [Header("Set Dynamically")]
    public Sprite sprite;
    public string naipe;
    public string nome;
    public int valor;
    public GameObject back = null;

    public bool faceUp
    {
        get { return !back.activeSelf; }
        set { back.SetActive(!value); }
    }

    SpriteRenderer _tSR = null;

    public void OnMouseDown()
    {
        _tSR = GetComponent<SpriteRenderer>();
        if (_tSR.sortingOrder == 3) _tSR.sortingOrder = 1;
        else Destroy(gameObject);
    }
}