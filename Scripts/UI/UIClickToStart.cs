/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    public class UIClickToStart : MonoBehaviour
    {
        public Vector3 pos = Vector3.zero;

        [SerializeField]
        public GameObject[] clickToShow;

        [SerializeField]
        public GameObject[] clickToHide;


        public GameObject player;

        // Use this for initialization
        void Start()
        {
            foreach (var p in clickToShow)
            {
                p.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        public void OnClickStart()
        {
            ClientProfileCheck.Run();
            foreach (var p in clickToShow)
            {
                p.gameObject.SetActive(true);
            }
            GameObject.Destroy(this.gameObject);
            foreach (var p in clickToHide)
            {
                p.gameObject.SetActive(false);
            }

            var obj = GameObject.Instantiate<GameObject>(player);
            if (pos != Vector3.zero)
            {
                obj.transform.position = pos;
            }
            obj.SetActive(true);
            obj.layer = 20;
            foreach (var p in obj.GetComponentsInChildren<Collider>(true))
            {
                p.gameObject.layer = 20;
            }

            foreach (var p in obj.GetComponentsInChildren<SetLinePositions>())
            {
                p.SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
            }
            obj.GetComponent<CharacterInformation>().myLayer = 20;
            obj.GetComponentInChildren<HeadRenderer>().SetColor(new Color(230f / 255f, 190f / 255f, 0f / 255f));
            //   var sync = obj.GetComponent<SyncablePlayer>();
            //    GameObject.DestroyObject(sync as Component);

            obj.GetComponent<Controller>().TakeLocalControl();
        }
    }
}