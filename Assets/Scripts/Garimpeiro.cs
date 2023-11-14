using System.Collections.Generic;
using TMPro;
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
    public GameObject EndingScreen;
    public TMP_Text scoreTitle;
    public TMP_Text scoreBody;

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
        EndingScreen.SetActive(false);
        ScoreManager.scoreTitle = scoreTitle;
        ScoreManager.scoreBody = scoreBody;
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
        foreach (CartaGarimpeiro tCP in tablado)
        {
            foreach (int hid in tCP.slotDef.hiddenBy)
            {
                cp = BuscaCartaPelolayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }
        MoveParaTarget(Draw());
        UpdateMonte();
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
                MoveParaDescarte(target);
                MoveParaTarget(Draw());
                UpdateMonte();
                ScoreManager.EVENT(eScoreEvent.monte);
                break;
            case eCartaState.tablado:
                bool jogadaValida = true;
                if (!cd.faceUp) jogadaValida = false;
                if (!ValorAdjacente(cd, target)) jogadaValida = false;
                if (!jogadaValida) return;
                tablado.Remove(cd);
                MoveParaTarget(cd);
                SetFacesTablado();
                ScoreManager.EVENT(eScoreEvent.mina);
                break;
        }
        VerificaGameOver();
    }

    void VerificaGameOver()
    {
        if (tablado.Count == 0)
        {
            scoreTitle.text = "Vitória!";

            GameOver(true);
            return;
        }
        if (monte.Count > 0) return;
        foreach (CartaGarimpeiro ct in tablado)
        {
            if (ValorAdjacente(ct, target)) return;
        }
        GameOver(false);
    }

    void GameOver(bool won)
    {
        
        if (won)
        {
            ScoreManager.EVENT(eScoreEvent.gameVitoria);
            EndingScreen.SetActive(true);
        }
        else
        {
            ScoreManager.EVENT(eScoreEvent.gameDerrota);
            EndingScreen.SetActive(true);
        }
    }

    public bool ValorAdjacente(CartaGarimpeiro c0, CartaGarimpeiro c1)
    {
        if (!c0.faceUp || !c1.faceUp) return (false);
        if (Mathf.Abs(c0.valor - c1.valor) == 1) return (true);
        if (c0.valor == 1 && c1.valor == 13) return (true);
        if (c0.valor == 13 && c1.valor == 1) return (true);
        return (false);
    }

    CartaGarimpeiro BuscaCartaPelolayoutID(int layoutID)
    {
        foreach (CartaGarimpeiro tCP in tablado)
        {
            if (tCP.layoutID == layoutID) return (tCP);
        }
        return (null);
    }

    void SetFacesTablado()
    {
        foreach (CartaGarimpeiro ct in tablado)
        {
            bool faceUp = true;
            foreach (CartaGarimpeiro cover in ct.hiddenBy)
            {
                if (cover.state == eCartaState.tablado) faceUp = false;
            }
            ct.faceUp = faceUp;
        }
    }
    public void chamaGarimpeiroGameplay()
    {
        SceneManager.LoadScene("GarimpeiroGameplay");
    }
    public void chamaMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

