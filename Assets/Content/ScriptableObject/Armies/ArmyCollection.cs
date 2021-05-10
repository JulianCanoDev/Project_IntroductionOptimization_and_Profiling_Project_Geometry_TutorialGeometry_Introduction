using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "ScriptableCollection/ArmyCollection")]
public class ArmyCollection : ScriptableCollection<SoldierController>
{
    public Material TeamOutfit;
}
