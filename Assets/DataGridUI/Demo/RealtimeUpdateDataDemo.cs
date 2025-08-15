using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{
    public class RealtimeUpdateDataDemo : MonoBehaviour
    {
        public DataGridUI dataGridUI;
        public InputField rowInput;
        public InputField columnInput;
        public InputField valueInput;

        public string[] photoFiles = new string[]{ "bird.png", "Cloud.png", "Food.png", "island.png", "Mountain.png", "Night.png" };

        public void SetData()
        {
            int row;
            int col;

            if (Int32.TryParse(rowInput.text, out row)==false) return;
            if (Int32.TryParse(columnInput.text,out col)==false) return;

            dataGridUI.SetCellData(row, col, valueInput.text);
        }

        public void ChangePercentage()
        {
            int v = UnityEngine.Random.Range(0, 100);
            dataGridUI.SetCellData(0, 5, v.ToString());
            
            v = UnityEngine.Random.Range(0, 100); 
            dataGridUI.SetCellData(1, 5, v.ToString());

            v = UnityEngine.Random.Range(0, 100); 
            dataGridUI.SetCellData(2, 5, v.ToString());

            v = UnityEngine.Random.Range(0, 100); 
            dataGridUI.SetCellData(3, 5, v.ToString());
            
            v = UnityEngine.Random.Range(0, 100); 
            dataGridUI.SetCellData(4, 5, v.ToString());
        }

        public void ChangeDropdown()
        {
            dataGridUI.SetCellData(0, 3, "item 2");
            dataGridUI.SetCellData(1, 3, "item 3");
            dataGridUI.SetCellData(2, 3, "item 4");
            dataGridUI.SetCellData(3, 3, "item 5");
            dataGridUI.SetCellData(4, 3, "item 1");
        }

        public void ChangePhotos()
        {
            int id = UnityEngine.Random.Range(0, photoFiles.Length);
            dataGridUI.SetCellData(0, 1, "SpriteStreaming:Photos/" + photoFiles[id]);
            id = UnityEngine.Random.Range(0, photoFiles.Length);
            dataGridUI.SetCellData(1, 1, "SpriteStreaming:Photos/" + photoFiles[id]);
            id = UnityEngine.Random.Range(0, photoFiles.Length);
            dataGridUI.SetCellData(2, 1, "SpriteStreaming:Photos/" + photoFiles[id]);
            id = UnityEngine.Random.Range(0, photoFiles.Length);
            dataGridUI.SetCellData(3, 1, "SpriteStreaming:Photos/" + photoFiles[id]);
            id = UnityEngine.Random.Range(0, photoFiles.Length);
            dataGridUI.SetCellData(4, 1, "SpriteStreaming:Photos/" + photoFiles[id]);
        }
    }
}