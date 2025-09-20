using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private string _sql;


    [Button]
    private void TEst()
    {
        var a =MySQLManager.Instance.ExecuteQueryToCsv(_sql);
        print(a);
    }
}
