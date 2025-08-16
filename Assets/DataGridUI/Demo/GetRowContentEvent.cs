using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace Maything.UI.DataGridUI
{
    public class GetRowContentEvent : MonoBehaviour
    {
        public Text text;
        // Start is called before the first frame update

        public void ShowButtonInfo(DataGridRowItemData itemData)
        {
            text.text = "You click key:" + itemData.key + " button! column:" + itemData.columnIndex.ToString() + " row:" + itemData.rowIndex.ToString();
            print(itemData.textData);
            print(itemData.value);
        }

        public void ShowContentChangedInfo(string value, DataGridRowItemData itemData)
        {
            text.text = "You changed key:" + itemData.key + " column:" + itemData.columnIndex.ToString() + " row: " + itemData.rowIndex.ToString()  + " value:" + value;
        }

        public void OnRowClick(List<DataGridColumnData> list, DataGridRowData itemData)
        {
            var a = JsonConvert.SerializeObject(itemData, Formatting.Indented);
            print(a);
        }

        public void OnRowDoubleClick(List<DataGridColumnData> list1, List<DataGridRowData> list2)
        {
            var a = JsonConvert.SerializeObject(list2, Formatting.Indented);
            print(a);
        }
    }
}