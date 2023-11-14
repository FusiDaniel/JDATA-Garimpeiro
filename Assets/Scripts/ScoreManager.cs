using TMPro;
using UnityEngine;

public enum eScoreEvent
{
    monte,
    mina,
    minaOuro,
    gameVitoria,
    gameDerrota
}
public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;
    static public int SCORE_DA_PARTIDA_ANTERIOR = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]
    public int serie = 0;
    public int scoreRodada = 0;
    public int score = 0;

    TMP_Text scoreText;

    void Awake()
    {
        if (S == null) S = this;
        else Debug.LogError("ScoreManager. Awake(): S já existe!");

        if (PlayerPrefs.HasKey("GarimpeiroHighScore")) HIGH_SCORE = PlayerPrefs.GetInt("GarimpeiroHighScore");

        score += SCORE_DA_PARTIDA_ANTERIOR;
        SCORE_DA_PARTIDA_ANTERIOR = 0;
        S.scoreText = GetComponent<TMP_Text>();
    }
    void Start()
    {
        S.scoreText.text = "Início, Record Atual = " + HIGH_SCORE;
    }
    static public void EVENT(eScoreEvent evt)
    {
        try
        {
            S.Event(evt);
        }
        catch (System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager:EVENT() chamado enquanto S = nul1. \n" + nre);
        }
    }
    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            case eScoreEvent.monte:
            case eScoreEvent.gameVitoria:
            case eScoreEvent.gameDerrota:
                serie = 0;
                score += scoreRodada;
                scoreRodada = 0;
                break;
            case eScoreEvent.mina:
                serie++;
                scoreRodada += serie;
                break;
        }
        switch (evt)
        {
            case eScoreEvent.gameVitoria:

                SCORE_DA_PARTIDA_ANTERIOR = score;
                if (SCORE_DA_PARTIDA_ANTERIOR > HIGH_SCORE)
                {
                    HIGH_SCORE = SCORE_DA_PARTIDA_ANTERIOR;
                    PlayerPrefs.SetInt("GarimpeiroHighScore", score);
                }
                print("VITÓRIA! Pontos desta Partida: " + score);
                break;
            case eScoreEvent.gameDerrota:
                if (HIGH_SCORE <= score) print("Você teve uma pontuação alta! High score: " + score);
                else print("Sua pontuação no game foi: " + score);
                break;
            default:
                scoreText.text = "Total: " + score.ToString() +
                " da rodada:" + scoreRodada.ToString() +
                ", série: " + serie.ToString();
                break;
        }
    }
}
