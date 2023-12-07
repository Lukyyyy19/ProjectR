using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CombosComparer
{
    [SerializeField] private ComboLists _comboLists;

    public CombosComparer()
    {
        Init();
    }

    private void Init()
    {
        _comboLists = new ComboLists();
    }
//FIJARSE EN EL PRIMER INPUT SI COINCIDE FIJARSE EN EL SIGUIENTE Y ASI PORQUE SINO NO VAMOS A SABER DONDE EMPIEZA EL COMBO
//FOR DE INPUTS CUANDO COINCIDA CON EL INICIO DEL COMBO VER SI EL SIGUIENTE MOVIMIENTO COINCIDE
    public bool IsComboCreated(Inputs[] inputs)
    {
        var comboLists = _comboLists.CombosLists;
        bool isSameCombo = false;
        for (int i = 0; i < comboLists.Count; i++)
        {
            isSameCombo = inputs.Select(((inputs1, j) => inputs.Skip(j).Take(comboLists[i].Length)))
            .Any(subList => subList.SequenceEqual(comboLists[i]));
            if(isSameCombo)break;
        }
        // if (currentList.Length > inputs.Length) return false;
        // for (int i = 0; i < inputs.Length; i++)
        // {
        //     bool subListMatch = true;
        //     for (int j = 0; j < currentList.Length; j++)
        //     {
        //         if (inputs[i + j] != currentList[j])
        //         {
        //             subListMatch = false;
        //             break;
        //         }
        //     }
        //
        //     if (subListMatch)
        //     {
        //         isSameCombo = true;
        //         break;
        //     }
        // }
        // if (currentList.Length > inputs.Length) return false;
        // int aux = 0 ;
        // for (int x = 0; x < currentList.Length; x++)
        // {
        //     for (int z = 0; z < inputs.Length; z++)
        //     {
        //         z = aux;
        //         if (currentList[x] != inputs[z])
        //         {
        //             aux = z;
        //             continue;
        //         }
        //         aux = z;
        //         isSameCombo = currentList[x] == inputs[z];
        //         break;
        //     }
        // }
        // for (int i = 0; i < currentList.Length; i++)
        // {
        //     _isSameCombo = inputs[i] == currentList[i];
        //     Debug.Log(_isSameCombo);
        //     if (!_isSameCombo) return _isSameCombo;
        // }

        if (isSameCombo) Debug.Log("Launching Combo");
        return isSameCombo;
    }
}