using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowContentPhotoUI : DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image imagePhoto;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void UpdateTheme()
        {
            imagePhoto.sprite = rowItemData.photoData;
            imagePhoto.preserveAspect = true;
            imagePhoto.gameObject.SetActive(true);

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

        public override void SetValue(string value)
        {
            string s = "";
            if (value.IndexOf("SpriteResource:") == 0)
            {
                //s hold resource name like "images/5Smileys/Smiley0"
                //(images folder must be located in Asstes/Resources folder
                s = value.Remove(0, "SpriteResource:".Length);
                rowItemData.photoData= SpriteHelper.GetResourceSprite(s);
                imagePhoto.sprite = rowItemData.photoData;
            }
            if (value.IndexOf("SpriteStreaming:") == 0)
            {
                //s hold resource name like "images/5Smileys/Smiley0"
                //(images folder must be located in StreamingAssets folder
                s = value.Remove(0, "SpriteStreaming:".Length);
                rowItemData.photoData = SpriteHelper.GetStreamingAssetsSprite(s);
                imagePhoto.sprite = rowItemData.photoData;
            }

            base.SetValue(value);
        }
    }
}