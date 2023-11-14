using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        }
    }

    CartaGarimpeiro Draw()
    {
        CartaGarimpeiro cd = monte[0];
        monte.RemoveAt(0);
        return (cd);
    }
}