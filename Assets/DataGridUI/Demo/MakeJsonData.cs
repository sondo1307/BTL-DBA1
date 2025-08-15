using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class MakeJsonData : MonoBehaviour
    {
        public Text text;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MakeData()
        {
            JsonData data = new JsonData();

            JsonColumnData columnData = new JsonColumnData();
            columnData.name = "";
            columnData.width = 38;
            columnData.type = "SelectedBox";
            data.columnData.Add(columnData);

            columnData = new JsonColumnData();
            columnData.name = "Name";
            columnData.width = 200;
            columnData.type = "";
            data.columnData.Add(columnData);

            columnData = new JsonColumnData();
            columnData.name = "City";
            columnData.width = 180;
            columnData.type = "";
            data.columnData.Add(columnData);

            columnData = new JsonColumnData();
            columnData.name = "Phone";
            columnData.width = 200;
            columnData.type = "";
            data.columnData.Add(columnData);

            columnData = new JsonColumnData();
            columnData.name = "Login Name";
            columnData.width = 200;
            columnData.type = "";
            data.columnData.Add(columnData);

            columnData = new JsonColumnData();
            columnData.name = "Email";
            columnData.width = 300;
            columnData.type = "";
            data.columnData.Add(columnData);


            JsonRowData rowData = new JsonRowData();
            List<string> rData = new List<string>();
            rData.Add("");
            rData.Add("Prof. Dillan Joan Lehner");
            rData.Add("Beahanside");
            rData.Add("1-460-764-6266 x392");
            rData.Add("kaylah_mckenzie");
            rData.Add("retta_stiedemann@dicki.name");

            rowData.data = rData;
            data.rowData.Add(rowData);


            rowData = new JsonRowData();
            rData = new List<string>();
            rData.Add("");
            rData.Add("Ms. Conrad Tony Reilly");
            rData.Add("O'Haramouth");
            rData.Add("(569)746-1631");
            rData.Add("freddie");
            rData.Add("bradly@littel.com");
            rowData.data = rData;
            data.rowData.Add(rowData);

            string code = JsonUtility.ToJson(data);

            Debug.Log(code);

            text.text = code;


        }
    }
}