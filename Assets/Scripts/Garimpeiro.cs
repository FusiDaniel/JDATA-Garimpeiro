using System.Collections.Generic;
using UnityEngine;

public class Garimpeiro : MonoBehaviour
{
    static public Garimpeiro S;

    [Header("Set in Inspector")]
    public TextAsset layoutXML;
    public float xOffset = 0f;
    public float yOffset = 0f;
    public Vector3 layoutCenter;
    public bool startFaceUp = true;
    public Sprite cartaBack;
    public GameObject prefabCarta;
    public GameObject prefabSprite;

    [Header("Set Dynamically")]
    public Baralho baralho;
    public Layout layout;
    public List<CartaGarimpeiro> monte;
    public List<string> nomesCartas;
    public List<Carta> cartasBaralho;
    public Transform pivoBaralho;
    public CartaGarimpeiro target;
    public List<CartaGarimpeiro> tablado;
    public List<CartaGarimpeiro> descarte;

    void Awake()
    {
        S = this;
    }

    void Start()
    {
        baralho = GetComponent<Baralho>();
        Baralho.Embaralha(ref cartasBaralho);
        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);
        monte = ConverteListaCartasToListCartasGarimpeiro(baralho.cartasBaralho);
        LayoutGame();
    }

    List<CartaGarimpeiro> ConverteListaCartasToListCartasGarimpeiro(List<Carta> lCD)
    {
        List<CartaGarimpeiro> lCP = new List<CartaGarimpeiro>();
        CartaGarimpeiro tCP;
        foreach (Carta tCD in lCD)
        {
            tCP = tCD as CartaGarimpeiro;
            lCP.Add(tCP);
        }
        return (lCP);
    }

    void LayoutGame()
    {
        if (pivoBaralho == null)
        {
            GameObject tGO = new GameObject("_pivoBaralho");
            pivoBaralho = tGO.transform;
            pivoBaralho.transform.position = layoutCenter;
        }
        CartaGarimpeiro cp;
        foreach (SlotDef tSD in layout.slotDefs)
        {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = pivoBaralho;
            cp.transform.localPosition = new Vector3(
                layout.multiplicador.x * tSD.x,
                layout.multiplicador.y * tSD.y + 5,
                -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            cp.state = eCartaState.tablado;
            cp.SetSortingLayerName(tSD.layerName);
            tablado.Add(cp);
        }
    }

    CartaGarimpeiro Draw()
    {
        CartaGarimpeiro cd = monte[0];
        monte.RemoveAt(0);
        return (cd);
    }

    void MoveParaDescarte(CartaGarimpeiro ct)
    {
        ct.state = eCartaState.descarte;
        descarte.Add(ct);
        ct.transform.parent = pivoBaralho;
        ct.transform.localPosition = new Vector3(
        layout.multiplicador.x * layout.descarte.x,
        layout.multiplicador.y * layout.descarte.y,
        -layout.descarte.layerID + 0.5f);
        ct.faceUp = true;
        ct.SetSortingLayerName(layout.descarte.layerName);
        ct.SetSortOrder(-100 + descarte.Count);
    }

    void MoveParaTarget(CartaGarimpeiro ct)
    {
        if (target != null) MoveParaDescarte(target);
        target = ct;
        ct.state = eCartaState.target;
        ct.transform.parent = pivoBaralho;
        ct.transform.localPosition = new Vector3(
        layout.multiplicador.x * layout.descarte.x,
        layout.multiplicador.y * layout.descarte.y,
        -layout.descarte.layerID);
        ct.faceUp = true;
        ct.SetSortingLayerName(layout.descarte.layerName);
        ct.SetSortOrder(0);
    }

    void UpdateMonte()
    {
        CartaGarimpeiro ct;
        for (int i = 0; i < monte.Count; i++)
        {
            ct = monte[i];
            ct.transform.parent = pivoBaralho;
            Vector2 dpSepara = layout.monte.espaco;
            ct.transform.localPosition = new Vector3(
            layout.multiplicador.x * (layout.monte.x + i * dpSepara.x),
            layout.multiplicador.y * (layout.monte.y + i * dpSepara.y),
            -layout.monte.layerID + 0.1f * i);
            ct.faceUp = false;

            ct.state = eCartaState.monte;
            ct.SetSortingLayerName(layout.monte.layerName);
            ct.SetSortOrder(-10 * i);
        }

    }

    public void CartaClicada(CartaGarimpeiro cd)
    {
        switch (cd.state)
        {
            case eCartaState.target:
                break;
            case eCartaState.monte:
                if (target != null) MoveParaDescarte(target);
                MoveParaTarget(Draw());
                UpdateMonte();
                break;
            case eCartaState.tablado:
                break;
        }

    }
}