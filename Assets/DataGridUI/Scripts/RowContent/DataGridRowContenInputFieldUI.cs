using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowContenInputFieldUI : DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public InputField inputField;
        public bool isMultiline = false;

        // Start is called before the first frame update
        void Start()
        {
            if (inputField!=null)
            {
                inputField.onValueChanged.AddListener(onInputFieldChanged);
                if (isMultiline)
                    AddInputNameClickEvent();
            }
        }

        void onInputFieldChanged(string newText)
        {
            dataGridUI.onRowContentChange.Invoke(newText, rowItemData);
            rowItemData.value = newText;
        }


        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            inputField.onValueChanged.RemoveAllListeners();
        }

        public override void UpdateTheme()
        {
            inputField.text = rowItemData.textData;
            rowItemData.value=rowItemData.textData;

            //inputField.textComponent.fontSize = dataGridUI.theme.rowTheme.textFontSize;
            for (int i = 0; i < inputField.transform.childCount; i++)
            {
                Text textControl = inputField.transform.GetChild(i).GetComponent<Text>();
                if (textControl!= null )
                {
                    textControl.fontSize = dataGridUI.theme.rowTheme.textFontSize;
                }

            }

            if (isMultiline == false)
            {
                RectTransform inputTransform = inputField.gameObject.GetComponent<RectTransform>();
                inputTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.rowTheme.controlHeight / -2f);
                inputTransform.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.rowTheme.controlHeight / 2f);
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

        public void UpdateInputField(string newText)
        {
            rowItemData.textData = newText;
        }

        private void AddInputNameClickEvent() //可以在Awake中调用
        {
            EventTrigger eventTrigger = inputField.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger==null)
                eventTrigger = inputField.gameObject.AddComponent<EventTrigger>();

            UnityAction<BaseEventData> selectEvent = OnInputFieldClicked;
            EventTrigger.Entry onClick = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerClick
            };

            onClick.callback.AddListener(selectEvent);
            eventTrigger.triggers.Add(onClick);
        }

        private void OnInputFieldClicked(BaseEventData data)
        {
            if (rowUI != null)
            {
                //rowUI.OnRowClick();
                dataGridUI.WhenRowClick(rowUI, false);

            }
        }

        public override void SetValue(string value)
        {
            inputField.text = value;
            base.SetValue(value);
        }
    }
}