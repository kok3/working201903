/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    // render a gamaobject to a tecture to show in UI.RawImage
    public class GameObjectTextureRenderer : MonoBehaviour
    {
        static List<Texture> textures = new List<Texture>();

        public RawImage[] img;
        public Camera renderer = null;
        public bool RenderDone = false;
        void Start()
        {
            textures.Clear();
            RenderDone = false;

            this.Run();

        }

        public void Run()
        {
            RenderDone = false;
            //   textures.Clear();
            //load info  and gameobject and render to list

            List<int> ids = new List<int> { 1, 2, 4, 5, 5 };
            foreach (var id in ids)
            {
                GameObject obj = //= PrefabsMgr.LoadMapObject("1/" + id.ToString());
                        MapLoader.ins.LoadMapObjectV1(1, id.ToString());

                obj = GameObject.Instantiate<GameObject>(obj, transform);
                // render to texture prepare for UI.RawImage
                var tex = Render(obj);
                textures.Add(tex);
                GameObject.DestroyImmediate(obj);
            }
            RenderDone = true;
        }
        void OnDestroy()
        {
            foreach (var p in img)
            {
                p.texture = null;
            }
        }
        public Texture Render(GameObject obj)
        {
            renderer.enabled = true;
            //reset base info
            obj.GetComponent<RenderToTextureInfo>().ResetTransform(this.transform.Find("Root"));

            var rend = new RenderTexture(100, 100, 100);
            rend.antiAliasing = 1;
            rend.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            rend.format = RenderTextureFormat.ARGB32;

            renderer.targetTexture = rend;
            renderer.Render();
            renderer.enabled = false;
            return rend;
        }
    }
}