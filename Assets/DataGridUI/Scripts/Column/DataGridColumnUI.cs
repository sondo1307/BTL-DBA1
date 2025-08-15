using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maything.UI.DataGridUI
{

    public class DataGridColumnUI : MonoBehaviour
    {
        [HideInInspector]
        public DataGridUI dataGridUI;

        [HideInInspector]
        public RectTransform content;

        [HideInInspector]
        public List<DataGridColumnData> columnData = new List<DataGridColumnData>();

        List<DataGridColumnItemUI> itemUIs = new List<DataGridColumnItemUI>();

        RectTransform ownerTransform;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InitializationColumn()
        {
            ownerTransform = GetComponent<RectTransform>();
            if (columnData == null) return;

            //float checkOffset = 0;
            float offsetX = 0;

            //if (dataGridUI.isMultiple && dataGridUI.theme.multipleCheckBoxTheme.isCheckBox)
            //{
            //    checkOffset = dataGridUI.theme.multipleCheckBoxTheme.rowWidth;
            //    offsetX += checkOffset;
            //}

            foreach (DataGridColumnData data in columnData)
            {
                offsetX += data.width;
            }
            ownerTransform.sizeDelta = new Vector2(offsetX, dataGridUI.theme.columnTheme.columnHeight);

            offsetX = 0;
            foreach (DataGridColumnData data in columnData)
            {
                GameObject go = Instantiate(dataGridUI.theme.columnItemTheme.columnItemTemplate, content);
                RectTransform rect = go.GetComponent<RectTransform>();
                DataGridColumnItemUI itemUI = go.GetComponent<DataGridColumnItemUI>();
                if (itemUI != null)
                {
                    itemUI.data = data;
                    itemUI.columnUI = this;
                    itemUI.data.objectUI = go;
                    itemUI.text.fontSize = dataGridUI.theme.columnItemTheme.textFontSize;
                    itemUIs.Add(itemUI);
                }

                rect.sizeDelta = new Vector2(data.width, dataGridUI.theme.columnTheme.columnHeight);
                rect.localPosition = new Vector3(ownerTransform.rect.width / -2f + offsetX, ownerTransform.rect.height / 2f, 0);
                offsetX += data.width;
            }


        }

        public void InitializationResizeBar()
        {
            float offsetX = 0;
            for (int i=0;i< content.childCount;i++)
            {
                Transform tran = content.GetChild(i);
                RectTransform tranRect = tran.GetComponent<RectTransform>();
                if (tran == null) continue;

                offsetX += tranRect.rect.width;

                DataGridColumnItemUI itemUI = tran.GetComponent<DataGridColumnItemUI>();
                if (itemUI == null) continue;

                GameObject go = Instantiate(dataGridUI.theme.columnTheme.columnSplitMoverTemplate, dataGridUI.columnMoverContent);
                RectTransform rect = go.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(4, dataGridUI.theme.columnTheme.columnHeight);
                rect.localPosition = new Vector3(dataGridUI.columnMoverContent.rect.width / -2f + offsetX, dataGridUI.columnMoverContent.rect.height / 2f - tranRect.rect.height/2f, 0);
                //rect.offsetMin = new Vector2(offsetX, tranRect.rect.height / -2f);
                //rect.offsetMax = new Vector2(offsetX + 4, tranRect.rect.height );
                ColumnSplitMover mover = go.GetComponent<ColumnSplitMover>();
                if (mover!=null)
                {
                    DataGridColumnItemUI leftItemUI = tran.GetComponent<DataGridColumnItemUI>();
                    mover.columnItemUI = leftItemUI;
                    mover.dataGridUI = dataGridUI;

                    RectTransform moverRect =  mover.moverBar.GetComponent<RectTransform>();
                    RectTransform gridRect = dataGridUI.GetComponent<RectTransform>();

                    //moverRect.sizeDelta = new Vector2(4, gridRect.rect.height);
                    moverRect.offsetMin = new Vector2(0, -gridRect.rect.height);
                    moverRect.offsetMax = new Vector2(0, 0);
                    //moverRect.localPosition = new Vector3(0,0);
                }
                itemUI.data.moverUI = go;
            }
        }

        public void ResizeColumn()
        {
            float offsetX = 0;
            
            foreach (DataGridColumnData data in columnData)
            {
                offsetX += data.width;
            }
            ownerTransform.sizeDelta = new Vector2(offsetX, dataGridUI.theme.columnTheme.columnHeight);

            offsetX = 0;
            foreach (DataGridColumnData data in columnData)
            {
                GameObject go = data.objectUI;
                RectTransform rect = go.GetComponent<RectTransform>();
                DataGridColumnItemUI itemUI = go.GetComponent<DataGridColumnItemUI>();

                rect.sizeDelta = new Vector2(data.width, dataGridUI.theme.columnTheme.columnHeight);
                rect.localPosition = new Vector3(ownerTransform.rect.width / -2f + offsetX, ownerTransform.rect.height / 2f, 0);
                offsetX += data.width;

                if (dataGridUI.isColumnResize && data.moverUI!=null)
                {
                    go= data.moverUI;
                    rect = go.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(4, dataGridUI.theme.columnTheme.columnHeight);
                    rect.localPosition = new Vector3(dataGridUI.columnMoverContent.rect.width / -2f + offsetX, dataGridUI.columnMoverContent.rect.height / 2f - rect.rect.height / 2f, 0);
                }
            }


            Vector2 dataSize = dataGridUI.dataContent.sizeDelta;

            RectTransform gridRect = dataGridUI.GetComponent<RectTransform>();

            dataGridUI.columnWidth = offsetX;
            dataGridUI.dataContent.sizeDelta = new Vector2(offsetX - gridRect.rect.width, dataSize.y);

        }

        public void ClearAllSortState()
        {
            foreach(DataGridColumnItemUI itemUI in itemUIs)
            {
                itemUI.ClearSortState();
            }
        }
    }
}