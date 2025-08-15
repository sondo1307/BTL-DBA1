using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maything.UI.DataGridUI
{
    public class ColumnSplitMover : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject moverBar;

        public Texture2D normalCursor;
        public Texture2D moveCursor;

        Vector3 resizeStartPosition = Vector3.zero;
        Vector2 resizeStartSize = Vector2.zero;
        Vector3 resizeStartOffset = Vector3.zero;

        Vector3 cursorOffset = Vector3.zero;

        bool isFirst = false;

        [HideInInspector]
        public DataGridUI dataGridUI;

        [HideInInspector]
        public DataGridColumnItemUI columnItemUI;

        RectTransform ownerTransform;
        // Start is called before the first frame update
        void Start()
        {
            ownerTransform = GetComponent<RectTransform>();

        }

        // Update is called once per frame
        void Update()
        {

        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            UnityEngine.Cursor.SetCursor(moveCursor, new Vector2(moveCursor.width / 2f, moveCursor.height / 2f), CursorMode.Auto);
        }
        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            UnityEngine.Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }

        void IBeginDragHandler.OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            resizeStartPosition = ownerTransform.position;
            resizeStartOffset = eventData.position;
            cursorOffset = Vector2.zero;

            moverBar.SetActive(true);
            //moverBar.GetComponent<RectTransform>().SetAsLastSibling();
            UnityEngine.Cursor.SetCursor(moveCursor, new Vector2(moveCursor.width / 2f, moveCursor.height / 2f), CursorMode.Auto);
            isFirst = true;
        }

        void IDragHandler.OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            UnityEngine.Cursor.SetCursor(moveCursor, new Vector2(moveCursor.width / 2f, moveCursor.height / 2f), CursorMode.Auto);
            var pos = eventData.position;
            resizeVertical(pos);
            
        }

        void resizeVertical(Vector3 pos)
        {
            Vector3 movePos = pos - resizeStartOffset;
            //Vector3 resizePos = resizeStartOffset - pos;

            if (isFirst && movePos.x != 0)
            {
                if (movePos.x > 0)
                {
                    cursorOffset = new Vector3(moveCursor.width / 3f, 0);
                    //cursorOffset = Vector2.zero;
                }
                else
                    cursorOffset = new Vector3(moveCursor.width / -3f, 0);


                isFirst = false;
            }

            ownerTransform.position = resizeStartPosition + new Vector3(movePos.x, 0, 0) + cursorOffset;// - new Vector3(startPoint.x - eventData.position.x, 0, 0);

        }

        void IEndDragHandler.OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            moverBar.SetActive(false);

            //ownerTransform.position = moverBar.transform.position;

            float newWidth = columnItemUI.data.width + (ownerTransform.position.x - resizeStartPosition.x);
            if (newWidth < 0) 
                newWidth = 0f;

            columnItemUI.data.width = newWidth;
            dataGridUI.ResizeColumn();

            //splitPanelUI.UpdatePanelPosition();
            UnityEngine.Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }

    }
}