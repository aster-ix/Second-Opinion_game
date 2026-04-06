using System.Collections.Generic;
using UnityEngine;

public class DialogNPCManager : MonoBehaviour
{
    public Dictionary<NPCType, DialogObject> dialogs = new();
    
    
}

public enum NPCType
{
    Husband,
    Doctor,
    Joker,
    Neighbour,
    Fantom,
    
}
