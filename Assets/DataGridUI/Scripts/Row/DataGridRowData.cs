using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maything.UI.DataGridUI
{

    [Serializable]
    public class DataGridRowData
    {
        public bool isSelected = false;
        //public List<string> rowData = new List<string>();
        public List<DataGridRowItemData> rowData = new List<DataGridRowItemData>();

        [HideInInspector]
        public GameObject objectUI = null;
    }

    [Serializable]
    public class DataGridRowItemData
    {
        public string textData;
        public string key;
        public bool checkData;
        public Sprite photoData;
        public string[] dropdownData;

        [HideInInspector]
        public int columnIndex;
        [HideInInspector]
        public int rowIndex;

        [HideInInspector]
        public string value = "";
    }


}