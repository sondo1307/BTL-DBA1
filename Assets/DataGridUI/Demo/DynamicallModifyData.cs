using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{
    public class DynamicallModifyData : MonoBehaviour
    {
        public DataGridUI dataGridUI;
        public Text textCode;
        public bool isCSV = true;
        public bool isClearRows = true;
        public void ModifyData()
        {
            if (isCSV)
            {
                //dataGridUI.DataFromCSV(true, true, isClearRows,false, textCode.text);
                CSVDataHelper.DataFromCSV(dataGridUI, true, true, isClearRows, false, textCode.text);
            }
            else
            {
                //dataGridUI.DataFromJson(true, true, isClearRows,false, textCode.text);
                JsonDataHelper.DataFromJson(dataGridUI, true, true, isClearRows, false, textCode.text); 
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}