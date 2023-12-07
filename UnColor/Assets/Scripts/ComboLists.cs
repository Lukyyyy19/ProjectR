using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ComboLists
{
    public List<Inputs[]> CombosLists;
    [SerializeField] public Inputs[] combo_1;
    [SerializeField] public Inputs[] combo_2;
    [SerializeField] public Inputs[] combo_3;
public ComboLists(){CreateNewCombo();}
    void CreateNewCombo()
    {
        combo_1 = new[] { Inputs.Up, Inputs.West, Inputs.East };
        combo_2 = new[] { Inputs.Down, Inputs.Left, Inputs.West, Inputs.North, Inputs.East };
        combo_3 = new[] { Inputs.Down, Inputs.Left, Inputs.North };
        CombosLists = new List<Inputs[]>{combo_1,combo_2,combo_3};
    }
}
