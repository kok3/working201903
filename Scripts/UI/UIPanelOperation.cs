using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MapEditor
{
    public enum OpTypeEnum
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public class UIPanelOperation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //包围盒高宽比
        public float boundBoxRatio = 1.0f;
        public const float MaxScale = 4;
        public const float Offset = 0.0f;
        private RectTransform curRecTran;
        public GameObject[] childObjs = new GameObject[4];
        private RectTransform[] childRecTrans = new RectTransform[4];
        private Vector3[] childOriginPostions = new Vector3[4];

        //目标
        private Transform targetTs;
             
        private float rotatedAngle = 0f;

        //当前缩放
        //private Vector3 scale = Vector3.one;
        //当前旋转
        private Vector3 originRotate = Vector3.zero;
        //
        private Vector3 originPos = Vector3.zero;

        private Vector3 originScale = Vector3.one;

        //两个旋转点之间的距离
        private float pivotDis = 1.0f;

        public static UIPanelOperation ins = null;

        UIRoot root;

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        //初始化数据
        public void InitData(Transform ts)
        {
            Show();

            Bounds objBoundingBox = ts.gameObject.CalculateBounds();
            //rotate = ts.eulerAngles;

            Debug.Log("111111111111111111111111 ts.eulerAngles: " + ts.eulerAngles);
            Debug.Log("111111111111111111111111 ts.rotation.eulerAngles: " + ts.rotation.eulerAngles);

            targetTs = ts;

            originScale = targetTs.localScale;
            originRotate = targetTs.rotation.eulerAngles;
            originPos = targetTs.position;

            boundBoxRatio = objBoundingBox.size.y / objBoundingBox.size.z;
            Debug.Log("@@@@@@@@@@@@@@  boundBoxRatio: " + boundBoxRatio);

            Vector3 leftPos = targetTs.position + new Vector3(0, 0, objBoundingBox.extents.z + Offset);
            Vector3 rightPos = targetTs.position - new Vector3(0, 0, objBoundingBox.extents.z + Offset);
            Vector3 upPos = targetTs.position + new Vector3(0, objBoundingBox.extents.y + Offset, 0);
            Vector3 downPos = targetTs.position - new Vector3(0, objBoundingBox.extents.y + Offset, 0);

            //世界坐标转屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTs.position);

            Vector3 screenLeftPos = Camera.main.WorldToScreenPoint(leftPos);
            Vector3 screenRightPos = Camera.main.WorldToScreenPoint(rightPos);
            Vector3 screenUpPos = Camera.main.WorldToScreenPoint(upPos);
            Vector3 screenDownPos = Camera.main.WorldToScreenPoint(downPos);

            //UI用Overlay,屏幕坐标就是UI的世界坐标
            {
                curRecTran.position = new Vector3(screenPos.x, screenPos.y, 0);

                childRecTrans[0].position = new Vector3(screenLeftPos.x, screenLeftPos.y, 0);
                childRecTrans[1].position = new Vector3(screenRightPos.x, screenRightPos.y, 0);
                childRecTrans[2].position = new Vector3(screenUpPos.x, screenUpPos.y, 0);
                childRecTrans[3].position = new Vector3(screenDownPos.x, screenDownPos.y, 0);

                Vector3 center = (childRecTrans[0].position + childRecTrans[1].position) / 2;
                //旋转
                float angle = targetTs.rotation.eulerAngles.x;
                for (int i = 0; i< 4; i++)
                {
                    childRecTrans[i].position = RotateRound(childRecTrans[i].position, center, new Vector3(0, 0, 1), angle);

                    childOriginPostions[i] = childRecTrans[i].position;
                }
                
            }

            pivotDis = Vector3.Distance(childRecTrans[0].position, childRecTrans[1].position);

        }

        //子控件开始拖动
        public void OnBeginChildDrag(int index)
        {
            //InitData(targetTs);

            originScale = targetTs.localScale;
            originRotate = targetTs.rotation.eulerAngles;
            originPos = targetTs.position;

            childObjs[2].SetActive(false);
            childObjs[3].SetActive(false);

            for (int i=0; i<4; i++)
            {
                childOriginPostions[i] = childRecTrans[i].position;
            }
        }

        //子控件拖动
        public void OnChildDrag(int index, Vector3 pos)
        {
            childRecTrans[index].position = pos;

            Vector3 originDir = Vector3.one;
            Vector3 newDir = Vector3.one;

            Vector3 pivotPos = Vector3.one;

            if (index == 1)
            {
                originDir = childOriginPostions[1] - childOriginPostions[0];
                newDir = pos - childOriginPostions[0];

                pivotPos = childOriginPostions[0];
            }
            else if (index == 0)
            {
                originDir = childOriginPostions[0] - childOriginPostions[1];
                newDir = pos - childOriginPostions[1];

                pivotPos = childOriginPostions[1];
            }

            Quaternion qt = Quaternion.FromToRotation(originDir, newDir);
            Debug.Log("=============================EluerAngles: " + qt.eulerAngles);

            //rotatedAngle = Vector3.Angle(originDir, newDir);

            rotatedAngle = qt.eulerAngles.z;

            //Vector3 cross = Vector3.Cross(originDir, newDir);
            ////顺时针角度用负值
            //if (cross.z < 0)
            //{
            //    rotatedAngle = -rotatedAngle;
            //}
            Debug.Log("@@@@@@@@@@@@@@@@@@@@@angle: " + rotatedAngle);
            //Debug.Log("@@@@@@@@@@@@@@@@@@@@@Cross: " + cross);

            //Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@originDir: " + originDir);
            //Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@newDir: " + newDir);

            float tmpDis = Vector3.Distance(childRecTrans[0].position, childRecTrans[1].position);
            float scale = tmpDis / pivotDis;
            if (scale >= MaxScale)
                scale = MaxScale;

            //float angle = Mathf.Asin((childRecTrans[1].position.y - childRecTrans[0].position.y) / (childRecTrans[1].position.x - childRecTrans[0].position.x));
            //57.3= 180 /3.14
            //targetTs.rotation = Quaternion.Euler(rotatedAngle, 0, 0);

            //UI世界坐标转屏幕坐标 (Overlay模式 两种坐标一致)
            Vector3 screenPos = new Vector3(pivotPos.x, pivotPos.y, 0);
            //屏幕坐标转世界坐标
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            //worldPos.x = 0;
            //这里的旋转是总的角度，旋转时要重置到旋转前的矩阵
            targetTs.rotation = Quaternion.Euler(originRotate);
            targetTs.localScale = originScale * scale;
            targetTs.position = originPos;
            targetTs.RotateAround(worldPos, new Vector3(1, 0, 0), rotatedAngle);
        }

        //拖动结束，按需重置拖动点
        public void OnEndChildDrag(int index)
        {
            Debug.Log("@@@@@@@@@@@@@@@@@@@@@  OnEndChildDrag: " + index);
            float tmpDis = Vector3.Distance(childRecTrans[0].position, childRecTrans[1].position);
            float scale = tmpDis / pivotDis;
            if (scale >= MaxScale)
            {
                scale = MaxScale;
                if (index == 1)
                {
                    childRecTrans[1].position = childRecTrans[0].position + (childRecTrans[1].position - childRecTrans[0].position) / tmpDis * MaxScale;
                    
                }
                else if (index == 0)
                {
                    childRecTrans[0].position = childRecTrans[1].position + (childRecTrans[0].position - childRecTrans[1].position) / tmpDis * MaxScale;
                }

            }

            Vector3 centerPos = (childRecTrans[0].position + childRecTrans[1].position) / 2;
            Vector3 rotatedPos = RotateRound(childRecTrans[1].position, centerPos, new Vector3(0, 0, 1), 90);
            childRecTrans[2].position = centerPos + (rotatedPos - centerPos) * boundBoxRatio;
            childRecTrans[3].position = childRecTrans[0].position + childRecTrans[1].position - childRecTrans[2].position;

            childObjs[2].SetActive(true);
            childObjs[3].SetActive(true);

        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 globalMousePos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(curRecTran, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                curRecTran.position = globalMousePos;

                Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, 0);
                var pos = Camera.main.ScreenToWorldPoint(screenPos);
                pos.x = 0f;

                if (EditorSelection.activeGameObject != null)
                {
                    EditorSelection.activeGameObject.transform.position = pos;
                }
            }
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("333333333333333  OnEndDrag");
        }

        //刷新位置
        void RefreshPos()
        {
            for (int i = 0; i < 4; i++)
            {
                childOriginPostions[i] = childRecTrans[i].position;
            }
        }

        void Awake()
        {
            ins = this;

            this.root = this.GetComponentInParent<UIRoot>();

            curRecTran = transform.GetComponent<RectTransform>();
            for (int i=0; i<4; i++)
            {
                childRecTrans[i] = childObjs[i].transform.GetComponent<RectTransform>();
                childOriginPostions[i] = childRecTrans[i].position;
            }

            //初始不显示
            Hide();
        }
        void Start()
        {

        }

        public Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
            Vector3 resultVec3 = center + point;
            return resultVec3;
        }

    }
}