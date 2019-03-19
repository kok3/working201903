using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace MapEditor
{
    public class MapObjectLevelTwo : MonoBehaviour
    {
        public int id = 5;
        protected UIPanelLevelTwo _panel_level_two;

        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (_panel_level_two != null)
                {
                    _panel_level_two.OnCellClick(this);
                }
            });
        }

        void Awake()
        {
            _panel_level_two = UIRoot.ins._panel_level_two;
        }

        public bool InitData(int id)
        {
            this.id = id;
            var img = this.GetComponent<Image>();
            var tex = MapLoader.ins.LoadEdotorImageV1(MapEditor.MapEditorConfig.CurrentSelectTheme, id.ToString());
            if (tex == null)
            {
                return false;
            }
            var sp = Sprite.Create(tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
            img.sprite = sp;
            return true;
        }
    }

}


