/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MapEditor
{   //因为
    //5.6.4 版本 Physics.CheckBox android崩溃几率极大，50%以上
    //2017.3版本 几率低了许多 但是还是相对出现几率很大，10%
    //因此用AABB重写  重新实现 碰撞检测 ,代价是性能， 待优化.....

    //在优化过程中 效果并不好，取消这种方式 还是用原来的Physics.CheckBox 
    //但是MapEditor独立加载 Preview的时候销毁 重建MapEditor（状态保存是MapEditorStroageData）  因为BUG在多场景后 在恢复单场景后出现，单场景没有出现
    public class AABBCollider : MonoBehaviour
    {
        public bool isTrigger
        {
            get
            {
                return EnableCollider;
            }
            set
            {
                EnableCollider = !value;
            }
        }
        BoxCollider box = null;
        void Awake()
        {
            var p = this.GetComponent<BoxCollider>();
            if (p != null)
            {
                this.box = p;
                isBoxCollider = true;
            }
            else
            {
                isBoxCollider = false;
            }
        }
        void Start()
        {
            if (box != null)
            {
                //由于美术资源不规范 比如 箱子 却是一个长方体  并不能任意旋转  
                //和其他模型 还有点不一样 因此需要处理是否出发swap
                Vector3 size = (box.center + box.size).Multiply(transform.lossyScale);
                if (transform.rotation.eulerAngles.NeedRotateYSwapXZ())
                {
                    size = size.SwapXZ();
                }
                // exchange z and x axis in 90 because of Camera.main           
                _inner = new Bounds(transform.position, size);
            }
        }
        Dictionary<AABBCollider, bool> _cache = new Dictionary<AABBCollider, bool>();

        public bool Intersects(AABBCollider other)
        {
            if (isBoxCollider == false || !EnableCollider || other == this) return false;
            bool dirty = false;
            if (transform.hasChanged)
            {
                dirty = true;
                _inner.center = transform.position;
                transform.hasChanged = false;
            }
            if (other.transform.hasChanged)
            {
                dirty = true;
                other._inner.center = other.transform.position;
                other.transform.hasChanged = false;
            }
            return _inner.Intersects(other._inner);
            if (dirty)
            {
                bool ok = _inner.Intersects(other._inner);
                if (_cache.ContainsKey(other))
                {
                    _cache[other] = ok;
                    return ok;
                }
                else
                {// not exist
                    _cache.Add(other, ok);
                    return ok;
                }
            }
            else
            {
                if (_cache.ContainsKey(other))
                {
                    return _cache[other];
                }
                else
                {// not exist
                    bool ok = _inner.Intersects(other._inner);
                    _cache.Add(other, ok);
                    return ok;
                }
            }
        }

        Bounds _inner;
        bool isBoxCollider = true;
        bool EnableCollider = true;
    }
}