using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCartaState
{
    monte,
    tablado,
    target,
    descarte
}
public class CartaGarimpeiro : Carta
{
    [Header("Set Dynamically: CartaGarimpeiro")]

    public eCartaState state = eCartaState.monte;

    public List<CartaGarimpeiro> hiddenBy = new List<CartaGarimpeiro>();

    public int layoutID;

    public SlotDef slotDef;
}