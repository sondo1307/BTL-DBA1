using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }
}