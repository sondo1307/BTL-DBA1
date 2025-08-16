using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;

public class MainCrud : MonoBehaviour
{
    [SerializeField] private DataGridUI _dataGridUI;
    
    
    public void ExportToCSV()
    {
        var a = CSVDataHelper.ExportToCSV(_dataGridUI);
        print(a);
    }
}
