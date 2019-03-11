using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorMapCaptureTest : MonoBehaviour
{
    [SerializeField]
    Image img;
    void Start()
    {
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("87306", (object sp) =>
        {
            if (sp != null)
            {
             //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0,0,1136,640), new Vector2(0.5f,0.5f));
            }
        });

        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("8815", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("16562", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("35346", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("46771", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("75733", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("87306", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });
        MapEditor.MapRuntimeCapture.AsyncGetSpriteMapCapture("95519", (object sp) =>
        {
            if (sp != null)
            {
                //   Sprite sp1 = img.sprite;
                img.sprite = Sprite.Create((Texture2D)sp, new Rect(0, 0, 1136, 640), new Vector2(0.5f, 0.5f));
            }
        });

    }
}
