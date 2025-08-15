using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Maything.UI.DataGridUI
{
    public class SpriteHelper
    {
        public static Sprite GetResourceSprite(string aResName)
        {
            Sprite sp = Resources.Load<Sprite>(aResName);//for spriteMode single
            if (sp == null && !string.IsNullOrEmpty(aResName))
            {
                // Load all sprites in atlas (on spriteMode multiple)
                string[] folders = aResName.Split('/');
                string spriteAtlasName = aResName.Remove(aResName.Length - folders[folders.Length - 1].Length - 1);
                aResName = folders[folders.Length - 1];

                Sprite[] spriteAtlas = Resources.LoadAll<Sprite>(spriteAtlasName);
                // Get specific sprite
                foreach (var item in spriteAtlas)
                    if (item.name == aResName)
                    {
                        sp = item;
                        break;
                    }
            }
            return sp;
        }

        public static Sprite GetStreamingAssetsSprite(string aResName)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            tex.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + "/" + aResName));
            Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            return sp;
        }

    }
}