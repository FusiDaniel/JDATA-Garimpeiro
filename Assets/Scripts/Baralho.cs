using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baralho : MonoBehaviour
{
    public bool startFaceUp = true;
    public GameObject prefabCarta;
    public GameObject prefabSprite;
    [Header("Set Dynamically")]

    public List<string> nomesCartas;
    public List<Carta> cartasBaralho;
    public Transform pivoBaralho;

    private Sprite _tSp = null;
    private GameObject _tGO = null;
    private SpriteRenderer _tSR = null;

    void Start()
    {
        IniciaBaralho();
    }

    public void IniciaBaralho()
    {
        GameObject centro = GameObject.Find("centroDaTela");
        pivoBaralho = centro.transform;
        bool mostra = startFaceUp;
        DescartaBaralho(mostra);
    }

    public void DescartaBaralho(bool flagMostra)
    {
        nomesCartas = new List<string>();
        string[] letras = new string[] { "C", "O", "E", "P" };
        foreach (string s in letras)
        {
            for (int i = 0; i < 13; i++)
            {
                nomesCartas.Add(s + (i + 1));
            }
        }
        cartasBaralho = new List<Carta>();
        for (int i = 0; i < nomesCartas.Count; i++)
        {
            cartasBaralho.Add(MakeCarta(flagMostra, i));
        }
    }

    private Carta MakeCarta(bool faceUp, int cNum)
    {
        _tGO = Instantiate(prefabCarta) as GameObject;
        _tGO.transform.parent = pivoBaralho;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        //_tGO.transform.localPosition = new Vector3(cNum%13 * 6 - 35, cNum/13 * 8 - 10, 0);
        _tGO.transform.localPosition = new Vector3(20, 5, 0);
        Carta _carta = _tGO.GetComponent<Carta>();
        if (nomesCartas[cNum].StartsWith("C")) _carta.naipe = "_of_hearts";
        else if (nomesCartas[cNum].StartsWith("O")) _carta.naipe = "_of_diamonds";
        else if (nomesCartas[cNum].StartsWith("E")) _carta.naipe = "_of_clubs";
        else if (nomesCartas[cNum].StartsWith("P")) _carta.naipe = "_of_spades";
        _carta.valor = int.Parse(nomesCartas[cNum].Substring(1));
        string nomeDaCarta = "";
        string numeroCarta = "";
        if (_carta.valor == 1) numeroCarta = "ace";
        else if (_carta.valor == 11) numeroCarta = "jack";
        else if (_carta.valor == 12) numeroCarta = "queen";
        else if (_carta.valor == 13) numeroCarta = "king";
        else numeroCarta = "" + _carta.valor;
        nomeDaCarta = numeroCarta + _carta.naipe;
        _carta.nome = "face";
        _tSp = (Sprite)(Resources.Load<Sprite>(nomeDaCarta));
        Sprite s1back = (Sprite)(Resources.Load<Sprite>("Card_Back_1"));
        _tSR.sprite = _tSp;
        _tSR.sortingOrder = 2;
        _tGO = Instantiate(prefabSprite) as GameObject;
        _tSR = _tGO.GetComponent<SpriteRenderer>();
        _tSR.sprite = s1back;
        _tGO.transform.SetParent(_carta.transform);
        _tGO.transform.localPosition = Vector3.zero;
        if (faceUp) _tSR.sortingOrder = 1;
        else _tSR.sortingOrder = 3;
        _carta.faceUp = faceUp;
        _carta.back = _tGO;
        _carta.nome = "back";
        return _carta;
    }

    static public void Embaralha(ref List<Carta> oCartas)
    {
        List<Carta> tCartas = new List<Carta>();
        int ndx;
        tCartas = new List<Carta>();
        while (oCartas.Count > 0)
        {
            ndx = Random.Range(0, oCartas.Count);
            tCartas.Add(oCartas[ndx]);
            oCartas.RemoveAt(ndx);
        }
        oCartas = tCartas;
    }
}
