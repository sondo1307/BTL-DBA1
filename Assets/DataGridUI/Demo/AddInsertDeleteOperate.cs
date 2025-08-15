using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{
    public class AddInsertDeleteOperate : MonoBehaviour
    {
        public DataGridUI dataGridUI;

        public Text jsonText;
        public Text csvText;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ClearAddJson()
        {
            JsonDataHelper.DataFromJson(dataGridUI, false, true, true, false, jsonText.text);
            //dataGridUI.DataFromJson(false, true, true,false, jsonText.text);
        }

        public void AddJson()
        {
            JsonDataHelper.DataFromJson(dataGridUI, false, true, false, false, jsonText.text);
            //dataGridUI.DataFromJson(false, true, false, false,jsonText.text);

        }

        public void InsertJson()
        {
            JsonDataHelper.DataFromJson(dataGridUI, false, true, false, true, jsonText.text);
            //dataGridUI.DataFromJson(false, true, false,true, jsonText.text);
        }

        public void ClearAddCSV()
        {
            CSVDataHelper.DataFromCSV(dataGridUI, false, true, true, false, csvText.text);
            //dataGridUI.DataFromCSV(false,true,true,false,csvText.text);
        }

        public void AddCSV()
        {
            CSVDataHelper.DataFromCSV(dataGridUI, false, true, false, false, csvText.text);
            //dataGridUI.DataFromCSV(false, true, false,false, csvText.text);
        }

        public void InsertCSV()
        {
            CSVDataHelper.DataFromCSV(dataGridUI,false, true, false, true, csvText.text);
            //dataGridUI.DataFromCSV(false, true, false,true, csvText.text);
        }


        public void ClearAndItemData()
        {
            ClassDataHelper.DataFromRowItemsData(dataGridUI, true,false, InitializationItems());
        }

        public void AddItemData()
        {
            ClassDataHelper.DataFromRowItemsData(dataGridUI, false, false, InitializationItems());
        }

        public void InsertItemData()
        {
            ClassDataHelper.DataFromRowItemsData(dataGridUI, false, true,InitializationItems());
        }


        List<DataGridRowData> InitializationItems()
        {
            List<DataGridRowData> rowsData = new List<DataGridRowData>();

            DataGridRowData data = new DataGridRowData();
            DataGridRowItemData item = new DataGridRowItemData();
            item.textData = "";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "Enoch Homenick";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.checkData = true;
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "Nanjing";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "1-914-164-7993";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "lea";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "willard_lind@beertowne.name";
            data.rowData.Add(item);


            item = new DataGridRowItemData();
            item.textData = "button1";
            data.rowData.Add(item);

            rowsData.Add(data);


            data = new DataGridRowData();
            item = new DataGridRowItemData();
            item.textData = "";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "Dr. Virgil Ara Reinger";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.checkData = false;
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "Shanghai";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "320.141.2108 x55557";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "wilburn_langosh";
            data.rowData.Add(item);

            item = new DataGridRowItemData();
            item.textData = "hailey@stiedemann.info";
            data.rowData.Add(item);


            item = new DataGridRowItemData();
            item.textData = "button2";
            data.rowData.Add(item);

            rowsData.Add(data);

            return rowsData;
        }
    }
}
