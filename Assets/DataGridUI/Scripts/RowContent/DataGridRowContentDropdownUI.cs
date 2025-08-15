using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowContentDropdownUI : DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Dropdown dropdown;

        // Start is called before the first frame update
        void Start()
        {
            if (dropdown != null)
            {
                dropdown.onValueChanged.AddListener(onDropdownChanged);
            }
        }

        void onDropdownChanged(int value)
        {
            if (value>=0 && value<=dropdown.options.Count)
            {
                dataGridUI.onRowContentChange.Invoke(dropdown.options[value].text, rowItemData);
                rowItemData.value = dropdown.options[value].text;
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            dropdown.onValueChanged.RemoveAllListeners();
        }

        public override void UpdateTheme()
        {
            //ColorBlock cb = new ColorBlock();

            //cb.normalColor = dataGridUI.theme.rowTheme.backgroundColor;
            //cb.
            RectTransform rectTransform = dropdown.gameObject.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(10, dataGridUI.theme.rowTheme.controlHeight / -2f);
            rectTransform.offsetMax = new Vector2(-10, dataGridUI.theme.rowTheme.controlHeight / 2f);
            dropdown.captionText.fontSize= dataGridUI.theme.rowTheme.textFontSize;
            dropdown.itemText.fontSize= dataGridUI.theme.rowTheme.textFontSize;

            rectTransform = dropdown.itemText.transform.parent.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.rowTheme.dropdownHeight / -2f);
            rectTransform.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.rowTheme.dropdownHeight / 2f);

            rowItemData.value = rowItemData.textData;

            if (rowItemData.dropdownData != null && rowItemData.dropdownData.Length > 0)
            {
                for (int i = 0; i < rowItemData.dropdownData.Length; i++)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = rowItemData.dropdownData[i];
                    dropdown.options.Add(data);

                    if (rowItemData.textData != "" && rowItemData.textData == rowItemData.dropdownData[i])
                    {
                        dropdown.value = i;
                    }
                }

                if (rowItemData.value == "")
                    rowItemData.value = rowItemData.dropdownData[0];

            }
            else if (columnData.dropdownData != null && columnData.dropdownData.Length > 0)
            {
                for (int i = 0; i < columnData.dropdownData.Length; i++)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = columnData.dropdownData[i];
                    dropdown.options.Add(data);

                    if (rowItemData.textData != "" && rowItemData.textData == columnData.dropdownData[i])
                        dropdown.value = i;
                }

                if (rowItemData.value == "")
                    rowItemData.value = columnData.dropdownData[0];
            }


            base.UpdateTheme();
        }

        public override void UpdateSelectState(enumItemState newState)
        {
            base.UpdateSelectState(newState);
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (rowUI != null)
                rowUI.UpdateSelectState(enumItemState.Enter, true);
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (rowUI != null)
                rowUI.UpdateSelectState(enumItemState.Normal, true);
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (rowUI != null)
                rowUI.OnRowClick();
        }

        public void DropdownValueChange(int newValue)
        {
            rowItemData.textData = dropdown.options[newValue].text;
        }

        public override void SetValue(string value)
        {
            for(int i=0;i<dropdown.options.Count;i++)
            {
                if (dropdown.options[i].text== value)
                {
                    dropdown.value = i;
                    break;
                }
            }

            base.SetValue(value);
        }

    }
}