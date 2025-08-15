using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DagaGridRowContentUI : MonoBehaviour
    {
        public enum enumItemState
        {
            Normal,
            Select,
            Enter,
        }

        public RectTransform border;


        [HideInInspector]
        public DataGridUI dataGridUI;

        [HideInInspector]
        public DataGridRowUI rowUI;

        [HideInInspector]
        public DataGridColumnData columnData;

        [HideInInspector]
        public DataGridRowItemData rowItemData;
        //public string rowName;

        [HideInInspector]
        public bool isAlternating = false;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void UpdateTheme()
        {
            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                if (isAlternating == false)
                    imgBG.color = dataGridUI.theme.rowTheme.backgroundColor;
                else
                    imgBG.color = dataGridUI.theme.rowTheme.alternatingBackgroundColor;

            }

            if (border != null)
            {
                if (dataGridUI.theme.rowTheme.isColumnBorder == false &&
                    dataGridUI.theme.rowTheme.isRowBorder == false)
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
                            imgBorder.color = dataGridUI.theme.rowTheme.borderColor;
                        }

                        imgBorder.gameObject.SetActive(false);

                        switch (imgBorder.gameObject.name)
                        {
                            case "BorderLeft":
                            case "BorderRight":
                                if (dataGridUI.theme.rowTheme.isColumnBorder)
                                    imgBorder.gameObject.SetActive(true);
                                break;
                            case "BorderTop":
                            case "BorderBottom":
                                if (dataGridUI.theme.rowTheme.isRowBorder)
                                    imgBorder.gameObject.SetActive(true);
                                break;
                        }
                    }
                }
            }
        }

        public virtual void UpdateSelectState(enumItemState newState)
        {
            Image imgBG = GetComponent<Image>();

            switch (newState)
            {
                case enumItemState.Normal:
                    if (isAlternating == false)
                    {
                        imgBG.color = dataGridUI.theme.rowTheme.backgroundColor;
                    }
                    else
                    {
                        imgBG.color = dataGridUI.theme.rowTheme.alternatingBackgroundColor;
                    }
                    break;
                case enumItemState.Select:
                    imgBG.color = dataGridUI.theme.rowTheme.selectedColor;
                    break;
                case enumItemState.Enter:
                    imgBG.color = dataGridUI.theme.rowTheme.enterColor;
                    break;

            }

        }

        public virtual void OnResize()
        {

        }

        public virtual string GetValue()
        {
            return rowItemData.value;
        }

        public virtual void SetValue(string value)
        {
            rowItemData.value = value;
        }
    }
}