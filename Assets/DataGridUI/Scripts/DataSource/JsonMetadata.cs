using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maything.UI.DataGridUI
{
    [Serializable]
    public class JsonData
    {
        public List<JsonColumnData> columnData = new List<JsonColumnData>();
        public List<JsonRowData> rowData = new List<JsonRowData>();
    }

    [Serializable]
    public class JsonColumnData
    {
        public string name;
        public string type;
        public float width;

        public string[] dropdownData;

    }

    [Serializable]
    public class JsonRowData
    {
        public List<string> data = new List<string>();
    }
}