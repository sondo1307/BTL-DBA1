using System;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;

namespace Script
{
    [CreateAssetMenu(fileName = "TempSO", menuName = "TempSO", order = 0)]
    public class TempSO : ScriptableObject
    {
        public string CsvHeader;
        public List<DataGridColumnData> columnData;
        public bool Refresh;
        
        private void OnValidate()
        {
            if (Refresh)
            {
                Convert();
                Refresh = false;
            }
        }

        private void Convert()
        {
            var a = StringUtils.ConvertHeaderToDataGridHeader(CsvHeader);
            var b = CSVDataHelper.CSVStringToColumnData(columnData, a);
        }
    }
}