using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridUI : MonoBehaviour
    {
        public enum enumDataSource
        {
            DataEntry,
            FromCSVText,
            FromJsonText,
            StreamingAssets,
        }

        public enum enumDataType
        {
            Column,
            ColumnAndRow,
            Row,
        }

        [Header("Config")]
        public DataGridTheme theme;
        public bool isColumnResize = false;
        public bool isColumnSort = false;

        public float rowHeight = 0;
        public bool isMultipleSelected = false;
        public RectTransform border;

        [Header("Paging")]
        public bool isPaging = false;
        public int pageSize = 15;

        int pageIndex = 0;
        int pageCount = 0;
        [HideInInspector]
        public int PageCount { get { return pageCount; } }
        [HideInInspector]
        public int CurrentPage { get { return pageIndex; } }

        [Header("Template")]
        public GameObject rowTemplate;
        public GameObject rowTextItemTemplate;
        public GameObject rowCheckBoxTemplate;
        public GameObject rowPhotoTemplate;
        public GameObject rowDropdownTemplate;
        public GameObject rowInputFieldTemplate;
        public GameObject rowButtomTemplate;
        public GameObject rowMultilineInputFieldTemplate;
        public GameObject rowPercentageTemplate;
        public GameObject rowIntTemplate;

        [Header("Data View")]
        public RectTransform dataScrollView;
        public RectTransform dataContent;

        [Header("Data Source")]
        public enumDataSource dataSource = enumDataSource.DataEntry;
        public enumDataType dataType = enumDataType.ColumnAndRow;

        public string streamingFile = "";
        [Multiline(5)]
        public string dataText = "";

        [Header("Column")]
        public RectTransform titleColumn;
        public DataGridColumnUI columnUI;
        public RectTransform columnContent;
        public RectTransform columnMoverContent;
        public List<DataGridColumnData> columnData = new List<DataGridColumnData>();

        [Header("Row")]
        public List<DataGridRowData> rowData = new List<DataGridRowData>();
        List<DataGridRowData> pageRowData = new List<DataGridRowData>();

        [Serializable]
        public class DataGridEvent : UnityEvent<List<DataGridColumnData>, DataGridRowData>
        {

        }
        [SerializeField]
        private DataGridEvent m_OnRowClick = new DataGridEvent();
        public DataGridEvent onRowClick
        {
            get { return m_OnRowClick; }
            set { m_OnRowClick = value; }
        }

        [Serializable]
        public class DataGridMultipleEvent : UnityEvent<List<DataGridColumnData>, List<DataGridRowData>>
        {

        }
        [SerializeField]
        private DataGridMultipleEvent m_OnRowsClick = new DataGridMultipleEvent();
        public DataGridMultipleEvent onRowsClick
        {
            get { return m_OnRowsClick; }
            set { m_OnRowsClick = value; }
        }



        [Serializable]
        public class DataGridButtonClickEvent :UnityEvent<DataGridRowItemData>
        {

        }
        [SerializeField]
        private DataGridButtonClickEvent m_OnRowButtonClick = new DataGridButtonClickEvent();
        public DataGridButtonClickEvent onRowButtonClick
        {
            get { return m_OnRowButtonClick; }
            set { m_OnRowButtonClick = value; }
        }

        [Serializable]
        public class DataGridContentChangedEvent : UnityEvent<string,DataGridRowItemData>
        {

        }
        [SerializeField]
        private DataGridContentChangedEvent m_OnRowContentChange = new DataGridContentChangedEvent();
        public DataGridContentChangedEvent onRowContentChange
        {
            get { return m_OnRowContentChange; }
            set { m_OnRowContentChange = value; }
        }

        [HideInInspector]
        public float columnWidth = 0;
        RectTransform ownerTransform;

        [HideInInspector]
        public List<DataGridRowUI> selectedRowUIs = new List<DataGridRowUI>();

        // Start is called before the first frame update
        void Start()
        {
            Initialization();
            InitializationData();
            InitializationPaging();
            InitializationColumn();
            InitializationRow(true);
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Initialization
        void Initialization()
        {
            if (border != null)
            {
                if (theme.isBorder == false)
                {
                    border.gameObject.SetActive(false);
                }
                else
                {
                    border.gameObject.SetActive(true);
                    for (int i = 0; i < border.childCount; i++)
                    {
                        Image imgBorder = border.GetChild(i).GetComponent<Image>();
                        if (imgBorder != null)
                        {
                            imgBorder.color = theme.borderColor;
                        }
                    }
                }
            }

            if (dataScrollView != null)
            {
                ScrollRect rect = dataScrollView.GetComponent<ScrollRect>();
                Image scrollImage = dataScrollView.GetComponent<Image>();
                if (scrollImage != null)
                {
                    scrollImage.color = theme.rowTheme.backgroundColor;
                }

                if (rect != null)
                {
                    UpdateScrollBarTheme(rect.horizontalScrollbar);
                    UpdateScrollBarTheme(rect.verticalScrollbar);


                    rect.horizontalScrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(0, theme.scrollBarSize);
                    rect.verticalScrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(theme.scrollBarSize, 0);
                }

            }
        }
        #endregion

        #region InitializationData
        void InitializationData()
        {
            switch (dataSource)
            {
                case enumDataSource.DataEntry:
                    break;
                case enumDataSource.FromCSVText:
                    CSVDataHelper.CSVToDataGridData(this,dataText);
                    break;
                case enumDataSource.FromJsonText:
                    JsonDataHelper.JsonToDataGridData(this,dataText);
                    break;
                case enumDataSource.StreamingAssets:
                    InitializationFile();
                    break;
            }
        }

        void InitializationFile()
        {
            string dataFile = Application.streamingAssetsPath + "\\" + streamingFile;
            if (File.Exists(dataFile) == false) return;

            switch (Path.GetExtension(dataFile).ToLower())
            {
                case ".csv":
                    CSVDataHelper.LoadCSVFile(this,dataFile);
                    break;
                case ".json":
                    JsonDataHelper.LoadJsonFile(this,dataFile);
                    break;
            }
        }

        #endregion

        #region InitializationPaging
        public void InitializationPaging()
        {
            pageIndex = 0;
            if (isPaging == false)
            {
                //pageRowData.Clear();
                //foreach(DataGridRowData data in rowData)
                //{
                //    pageRowData.Add(data);
                //}
                pageRowData = rowData;
            }
            else
            {
                pageCount = Mathf.CeilToInt(rowData.Count / pageSize);
                setPageData(pageIndex);
            }
        }

        void setPageData(int newPageIndex)
        {
            if (newPageIndex * pageSize > rowData.Count) return;
            int startIndex = newPageIndex * pageSize;
            int endIndex = newPageIndex * pageSize + pageSize;
            if (endIndex > rowData.Count) endIndex = rowData.Count;


            pageIndex = newPageIndex;
            pageRowData.Clear();

            for (int i = startIndex; i < endIndex; i++)
            {
                pageRowData.Add(rowData[i]);
            }
        }
        #endregion

        #region UpdateScrollBarTheme
        void UpdateScrollBarTheme(Scrollbar scrollbar)
        {
            ColorBlock colorBlock = scrollbar.colors;

            colorBlock.normalColor = theme.scrollBarNormalColor;
            colorBlock.selectedColor = theme.scrollBarSelectedColor;
            colorBlock.disabledColor = theme.scrollBarDisabledColor;
            colorBlock.pressedColor = theme.scrollBarPressedColor;
            colorBlock.disabledColor = theme.scrollBarDisabledColor;

            scrollbar.colors = colorBlock;

            Image img = scrollbar.gameObject.GetComponent<Image>();
            if (img != null)
            {
                img.color = theme.scrollBarBackgroundColor;
            }
        }
        #endregion

        #region InitializationColumn
        public void InitializationColumn()
        {
            ownerTransform = GetComponent<RectTransform>();

            ColumnClear();
            columnWidth = 0;

            if (columnUI != null)
            {
                columnUI.dataGridUI = this;
                columnUI.columnData = columnData;
                columnUI.content = columnContent;
                columnUI.InitializationColumn();
                if (isColumnResize)
                    columnUI.InitializationResizeBar();

                //Check Box!
                //if (isMultiple && theme.multipleCheckBoxTheme.isCheckBox)
                //    columnWidth += theme.multipleCheckBoxTheme.rowWidth;

                foreach (DataGridColumnData data in columnData)
                {
                    columnWidth += data.width;
                }
            }

            if (titleColumn != null)
            {
                titleColumn.sizeDelta = new Vector2(0, theme.columnTheme.columnHeight);
            }

            if (dataScrollView != null)
            {
                dataScrollView.sizeDelta = new Vector2(0, -theme.columnTheme.columnHeight);
                dataScrollView.localPosition = new Vector3(0, -theme.columnTheme.columnHeight / 2f, 0);
            }

            if (dataContent != null && columnWidth > 0)
            {
                dataContent.sizeDelta = new Vector2(columnWidth - ownerTransform.rect.width, 200);
            }
        }
        #endregion


        #region InitializationRow
        public void InitializationRow(bool isRemoveExistData)
        {
            int rowIndex = 0;

            if (rowData == null) return;
            if (rowData.Count == 0) return;
            if (pageRowData == null) return;
            if (pageRowData.Count == 0) return;
            if (columnWidth <= 0) return;

            selectedRowUIs.Clear();

            float rowOffsetY = 0;
            int alternating = 0;

            float rHeight = rowHeight;
            if (rHeight == 0) rHeight = theme.rowTheme.rowHeight;

            //float totalHeight = theme.rowTheme.rowHeight * rowData.Count;
            //float totalHeight = rHeight * rowData.Count;
            float totalHeight = rHeight * pageRowData.Count;
            if (dataContent != null && columnWidth > 0)
            {
                dataContent.sizeDelta = new Vector2(columnWidth - ownerTransform.rect.width, totalHeight);
            }

            //foreach(DataGridRowData data in rowData)
            foreach (DataGridRowData data in pageRowData)
            {
                alternating += 1;

                bool isCreateNew = true;

                if (data.objectUI != null)
                {
                    if (isRemoveExistData)
                    {
                        Destroy(data.objectUI);
                    }
                    else
                    {
                        isCreateNew = false;
                    }
                }

                GameObject go = null;
                if (isCreateNew)
                {
                    go = Instantiate(rowTemplate, dataContent);
                }
                else
                {
                    go = data.objectUI;
                }
                RectTransform rect = go.GetComponent<RectTransform>();
                //rect.sizeDelta = new Vector2(columnWidth, theme.rowTheme.rowHeight);
                rect.sizeDelta = new Vector2(columnWidth, rHeight);
                rect.localPosition = new Vector3(0, -1 * rowOffsetY, 0);

                //rowOffsetY += theme.rowTheme.rowHeight;
                rowOffsetY += rHeight;

                DataGridRowUI rowUI = go.GetComponent<DataGridRowUI>();
                if (rowUI != null)
                {
                    rowUI.dataGridUI = this;

                    foreach(DataGridRowItemData rowData in data.rowData)
                    {
                        rowData.rowIndex = rowIndex;
                    }
                    rowUI.rowData = data;

                    rowUI.isAlternating = (alternating % 2 == 0);

                    rowUI.InitializationRow();

                    if (data.isSelected)
                    {
                        selectedRowUIs.Add(rowUI);
                    }
                }

                data.objectUI = go;

                rowIndex += 1;
            }

            if (isMultipleSelected)
            {
                foreach (DataGridRowUI rowUI in selectedRowUIs)
                {
                    rowUI.UpdateSelectState(DagaGridRowContentUI.enumItemState.Select, true);
                }
            }
            else
            {
                //ֻѡ�����һ��
                if (selectedRowUIs.Count > 0)
                {
                    DataGridRowUI rowUI = selectedRowUIs[selectedRowUIs.Count - 1];

                    rowUI.UpdateSelectState(DagaGridRowContentUI.enumItemState.Select, true);

                    selectedRowUIs.Clear();
                    selectedRowUIs.Add(rowUI);
                }
            }

        }

        public void ResizeRows()
        {
            if (rowData == null) return;
            if (rowData.Count == 0) return;
            if (pageRowData == null) return;
            if (pageRowData.Count == 0) return;
            if (columnWidth <= 0) return;

            foreach (DataGridRowData data in pageRowData)
            {
                GameObject go = data.objectUI;
                DataGridRowUI rowUI = go.GetComponent<DataGridRowUI>();
                if (rowUI != null)
                {
                    rowUI.ResizeRow();
                }
            }
        }
        #endregion

        #region RowClick
        public void WhenRowClick(DataGridRowUI selectRowUI,bool isReverseSelected)
        {
            if (isMultipleSelected)
            {
                //��ѡ
                bool isFind = false;
                foreach (DataGridRowUI ui in selectedRowUIs)
                {
                    if (ui == selectRowUI)
                    {
                        //ȡ��ѡ��
                        isFind = true;

                        if (isReverseSelected == false)
                            return;

                        selectRowUI.UpdateSelectState(DagaGridRowContentUI.enumItemState.Normal, false);
                        break;
                    }
                }

                if (isFind == true)
                {
                    //�Ƴ�
                    selectedRowUIs.Remove(selectRowUI);
                }
                else
                {
                    //����
                    selectedRowUIs.Add(selectRowUI);
                    selectRowUI.UpdateSelectState(DagaGridRowContentUI.enumItemState.Select, true);
                }

                List<DataGridRowData> data = new List<DataGridRowData>();
                foreach (DataGridRowUI ui in selectedRowUIs)
                {
                    data.Add(ui.rowData);
                }

                onRowsClick.Invoke(columnData, data);

            }
            else
            {
                //��ѡ
                bool isFind = false;
                if (selectedRowUIs.Count > 0)
                {
                    foreach (DataGridRowUI ui in selectedRowUIs)
                    {
                        if (ui == selectRowUI)
                        {
                            isFind = true;
                            if (isReverseSelected == false)
                                return;
                        }


                        ui.UpdateSelectState(DagaGridRowContentUI.enumItemState.Normal, false);
                    }
                }
                selectedRowUIs.Clear();

                if (isFind == false)
                {
                    selectedRowUIs.Add(selectRowUI);
                    selectRowUI.UpdateSelectState(DagaGridRowContentUI.enumItemState.Select, true);
                    onRowClick.Invoke(columnData, selectRowUI.rowData);
                }
                else
                {
                    onRowClick.Invoke(columnData, null);
                }

            }

        }
        #endregion

        public void horizontalScrollbarChange(float value)
        {
            float scrollWidth = columnWidth - ownerTransform.rect.width;
            if (columnUI != null)
            {

                //columnContent.localPosition = new Vector2(scrollWidth  - scrollWidth * value  , theme.columnTheme.columnHeight /2f *-1 );
                //columnContent.localPosition = new Vector2(scrollWidth * (1f-value), theme.columnTheme.columnHeight / 2f * -1);
                //columnContent.localPosition = new Vector2((columnWidth - ownerTransform.rect.width) * (1f - value)  , theme.columnTheme.columnHeight / 2f * -1);
                columnUI.GetComponent<RectTransform>().localPosition = new Vector2(scrollWidth * (1f - value) - scrollWidth - ownerTransform.rect.width / 2f, 0);
            }

            if (isColumnResize)
            {
                //ҲҪͬ���ƶ�Mover
                columnMoverContent.GetComponent<RectTransform>().localPosition = new Vector2(
                    //scrollWidth * (1f - value) - scrollWidth - ownerTransform.rect.width / 2f, 0);
                    scrollWidth * (1f - value) - scrollWidth, 0);

            }
        }

        #region Multi Selected

        public bool IsRowSelected(DataGridRowUI checkRowUI)
        {
            foreach (DataGridRowUI ui in selectedRowUIs)
            {
                if (ui == checkRowUI)
                    return true;
            }

            return false;
        }

        public void PageRowClear()
        {
            foreach (DataGridRowData data in pageRowData)
            {
                if (data.objectUI != null)
                {
                    Destroy(data.objectUI);
                    data.objectUI = null;
                }
            }

            //���¼���ߴ�
            dataContent.sizeDelta = new Vector2(columnWidth - ownerTransform.rect.width, 0);
        }

        public void RowClear()
        {
            foreach (DataGridRowData data in rowData)
            {
                if (data.objectUI != null)
                {
                    Destroy(data.objectUI);
                    data.objectUI = null;
                }
            }

            rowData.Clear();

        }

        public void ColumnClear()
        {
            columnWidth = 0;
            foreach (DataGridColumnData data in columnData)
            {
                Destroy(data.objectUI);
                data.objectUI = null;
            }
        }

        public void ChangePage(int newPageIndex)
        {
            if (isPaging == false) return;
            PageRowClear();
            setPageData(newPageIndex);
            InitializationRow(true);
        }

        public void FirstPage()
        {
            ChangePage(0);
        }


        public void PrevPage()
        {
            if (isPaging == false) return;

            pageIndex -= 1;
            if (pageIndex < 0) pageIndex = 0;
            ChangePage(pageIndex);
        }

        public void NextPage()
        {
            if (isPaging == false) return;

            pageIndex += 1;
            if (pageIndex > pageCount) pageIndex = pageCount;
            ChangePage(pageIndex);
        }

        public void LastPage()
        {
            if (isPaging == false) return;
            ChangePage(pageCount);
        }

        public int GetCurrentStartIndex()
        {
            return pageIndex * pageSize;
        }

        public int GetCurrentLastIndex()
        {
            return pageIndex * pageSize + pageRowData.Count;

        }
        #endregion

        public DataGridRowUI GetLastSelectItem()
        {
            if (selectedRowUIs.Count <= 0) return null;

            return selectedRowUIs[selectedRowUIs.Count - 1];
        }

        public void ClearAllRows()
        {
            PageRowClear();
            RowClear();
            InitializationRow(true);
        }

        public void RemoveSelectedItem()
        {
            if (selectedRowUIs.Count == 0) return;

            foreach(DataGridRowUI row in selectedRowUIs)
            {
                if (row.rowData.objectUI!=null)
                    Destroy(row.rowData.objectUI);

                rowData.Remove(row.rowData);
            }
            selectedRowUIs.Clear();
            InitializationRow(true);

        }

        public void ResizeColumn()
        {
            columnUI.ResizeColumn();
            ResizeRows();
        }

        public void SetCellData(int row,int column,string value)
        {
            if (column >= columnData.Count) return;
            if (row >= rowData.Count) return;

            DataGridRowUI rowUI = rowData[row].objectUI.GetComponent<DataGridRowUI>(); 
            if (rowUI != null)
            {
                if (column >= rowUI.rowItems.Count) return;

                rowUI.rowItems[column].SetValue(value);
            }
        }

        public void SortByColumn(DataGridColumnData column, bool isDescending)
        {
            for (int i=0;i<columnData.Count;i++)
            {
                if (columnData[i]==column)
                {
                    SortByColumn(i, isDescending);
                }
            }
        }

        public void SortByColumn(int orderIndex, bool isDescending)
        {
            //The sorting function uses Linq.
            //If the platform you output does not support Linq, you can delete the DataSortHelper.cs file and annotate SortByColumn
            rowData = DataSortHelper.SortRowData(rowData, orderIndex, isDescending);
            pageRowData = DataSortHelper.SortRowData(pageRowData, orderIndex, isDescending);

            InitializationPaging();
            InitializationRow(true);
        }
    }
}