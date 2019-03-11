using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class UISelectTheme : MonoBehaviour
    {
        int[] theme = null;
        public SpriteRenderer sprite;
        // Use this for initialization
        void Start()
        {
            this.theme = ConfigLoader.ins.GetConfig<ConfigMapEditorThemeRoot>().themes_ids;

            themeAyyayLength = this.theme.Length;   // MapEditorConfig.theme.Length;
            if (themeAyyayLength <= 0)
            {
                Debug.LogError("MapEditor.MapEditorConfig.theme.Length is zero");
            }
            ChangeSelectTheme(Random.Range(0, themeAyyayLength));
        }

        public void Awake()
        {
            ins = this;
            selectedTheme = 1;
        }

        public void OnDestroy()
        {
            ins = null;
        }

        // 传入index 的改变量 读取MapEditorConfig.theme的值
        public void ChangeSelectTheme(int Dvalue)
        {
            {
                if (themeAyyayLength <= 0)
                {
                    Debug.LogError("MapEditor.MapEditorConfig.theme.Length is zero");
                    return;
                }
                currentIndex = (currentIndex + Dvalue + themeAyyayLength) % themeAyyayLength;
                selectedTheme = this.theme[currentIndex];
                var tex = MapLoader.ins.LoadEdotorImageThemeV1(selectedTheme);  //(Texture2D)PrefabsMgr.Load<Object>("Map/Image/theme/" + selectedTheme);
                if (tex == null)
                {
                    Debug.LogError("fail to load sprite, function is ChangeSelectTheme ");
                    return;
                }

                ShowImage.sprite = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));

            }



            {
                /*    var tex = (Texture2D)PrefabsMgr.Load<Object>("Map/Image/theme_bg/" + selectedTheme);
                    if (tex == null)
                    {
                        Debug.LogError("fail to load sprite, function is ChangeSelectTheme ");
                        return;
                    }

                    sprite.sprite = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
                    GameObject.DestroyImmediate(sprite.GetComponent<BackGround>());
                    sprite.color = Color.white;

                    sprite.gameObject.AddComponent<BackGround>();*/
            }
        }


        public void GoNextScene()
        {
            Application.LoadLevel("MapEditor");
        }
        // Update is called once per frame

        public static int selectedTheme
        {
            get
            {
                return MapEditor.MapEditorConfig.CurrentSelectTheme;
            }
            set
            {
                MapEditor.MapEditorConfig.CurrentSelectTheme = value;
            }
        }
        public static int CurrentSelectedTheme
        {
            get
            {
                return selectedTheme;
            }
        }
        public static UISelectTheme ins;

        public Image ShowImage;
        private int currentIndex;
        private int themeAyyayLength;

    }

}

