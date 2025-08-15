using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{
    public class ExportDataDemo : MonoBehaviour
    {
        public DataGridUI dataGridUI;
        public Text text;
        public void ExportToCSV()
        {
            text.text = CSVDataHelper.ExportToCSV(dataGridUI);
        }

        public void ExportToJson()
        {
            text.text = JsonDataHelper.ExportDataToJson(dataGridUI);
        }
    }
}
