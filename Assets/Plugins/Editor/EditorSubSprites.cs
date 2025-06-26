// system
using System.Collections;
using System.Collections.Generic;

// unity
using UnityEngine;
using UnityEditor;

namespace CODEHUB
{
    public class ExportSubSprites : Editor
    {

        [MenuItem("Assets/Export Sub-Sprites")]
        public static void DoExportSubSprites()
        {
            var folder = EditorUtility.OpenFolderPanel("Export subsprites into what folder?", "", "");

            foreach (var obj in Selection.objects)
            {
                var sprite = obj as Sprite;

                if (sprite == null)
                {
                    continue;
                }

                var extracted = ExtractAndName(sprite);

                SaveSubSprite(extracted, folder);
            }

        }

        [MenuItem("Assets/Export Sub-Sprites", true)]
        private static bool CanExportSubSprites()
        {
            return Selection.activeObject is Sprite;
        }

        // Since a sprite may exist anywhere on a tex2d, this will crop out the sprite's claimed region and return a new, cropped, tex2d.
        private static Texture2D ExtractAndName(Sprite sprite)
        {
            var texture = sprite.texture;

            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);

            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;

            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;

            // Create a new readable Texture2D to copy the pixels to it
            Texture2D myTexture2D = new Texture2D(texture.width, texture.height);

            // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();

            // Reset the active RenderTexture
            RenderTexture.active = previous;

            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            // "myTexture2D" now has the same pixels from "texture" and it's re
            var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var r = sprite.textureRect;
            var pixels = myTexture2D.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);

            output.SetPixels(pixels);
            output.Apply();
            output.name = myTexture2D.name + " " + sprite.name;

            return output;
        }

        private static void SaveSubSprite(Texture2D tex, string saveToDirectory)
        {
            if (!System.IO.Directory.Exists(saveToDirectory))
            {
                System.IO.Directory.CreateDirectory(saveToDirectory);
            }

            System.IO.File.WriteAllBytes(System.IO.Path.Combine(saveToDirectory, tex.name + ".png"), tex.EncodeToPNG());
        }
    }
}