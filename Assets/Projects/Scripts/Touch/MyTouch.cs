using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*挂在canvas下面*/
public class MyTouch : MonoBehaviour {

       /// <summary>  
    /// 定义的一个手指类  
    /// </summary>  
    class MyFinger
    {
        public int id = -1;
        public Touch touch;
        public Transform touchTrans;
         
        static private List<MyFinger> fingers = new List<MyFinger>();
        /// <summary>  
        /// 手指容器  
        /// </summary>  
        static public List<MyFinger> Fingers
        {
            get
            {
                if (fingers.Count == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        MyFinger mf = new MyFinger();
                        mf.id = -1;
                        mf.touchTrans = null;
                        fingers.Add(mf);
                    }
                }
                return fingers;
            }
        }
    }

   public List<TouchItem> currentTouchList = new List<TouchItem>();
    
    // 存储注册进来的Touch 物体
    public List<GameObject> myTouchs;

    private void Awake()
    {
        RaycastInCanvas = transform.GetComponent<GraphicRaycaster>();
    }

    Touch[] touches;
    MyFinger mf;
    void Update()
    {
        touches = Input.touches;
        if (touches.Length <= 0)
            return;
        // 遍历所有的已经记录的手指  
        // --掦除已经不存在的手指  

        for (int i = 0; i < MyFinger.Fingers.Count; i++)
        {
            if(MyFinger.Fingers[i].id == -1)
            {
                continue;
            }
            bool stillExit = false;
            for (int j = 0; j < touches.Length; j++)
            {
                if(MyFinger.Fingers[i].id == touches[j].fingerId)
                {
                    stillExit = true;
                    break;
                }

                if (stillExit == false)
                {
                    MyFinger.Fingers[i].id = -1;
                    MyFinger.Fingers[i].touchTrans = null;
                }
            }
        }

        // 遍历当前的touches  
        // --并检查它们在是否已经记录在AllFinger中  
        // --是的话更新对应手指的状态，不是的加进去  
        for (int k = 0; k < touches.Length; k++)
        {
            bool stillExit = false;
            for (int n = 0; n < MyFinger.Fingers.Count; n++)
            {
                if (touches[k].fingerId == MyFinger.Fingers[n].id)
                {
                    stillExit = true;
                    MyFinger.Fingers[n].touch = touches[k];
                    break;
                }
            }
            if (!stillExit)
            {
                for (int m = 0; m < MyFinger.Fingers.Count; m++)
                {
                    if(MyFinger.Fingers[m].id == -1)
                    {
                        MyFinger.Fingers[m].id = touches[k].fingerId;
                        MyFinger.Fingers[m].touch = touches[k];
                        break;
                    }
                }
            }
        }

        // 记录完手指信息后，就是响应相应和状态记录了  
        for (int i = 0; i < MyFinger.Fingers.Count; i++)
        {
            mf = MyFinger.Fingers[i];
            if (mf.id != -1)
            {
                if (mf.touchTrans == null)
                {
                    mf.touchTrans = CheckGuiRaycastObjects(mf.touch.position);
                }

                if (mf.touchTrans != null)
                {
                    TouchItem touchItem = mf.touchTrans.GetComponent<TouchItem>();
                    if (mf.touch.phase == TouchPhase.Began)
                    {
                        if (!touchItem.touchList.Contains(mf.touch))
                        {
                            touchItem.touchList.Add(mf.touch);
                            if (touchItem.touchList.Count == 2)
                            {
                                touchItem.SecondFingerBeganDrag();
                            }
                            if (mf.touch.fingerId == touchItem.touchList[0].fingerId)
                            {
                                touchItem.OnTouchBegin(touchItem.touchList[0]);
                            }
                        }
                    }
                    else if (mf.touch.phase == TouchPhase.Moved)
                    {
                        if (touchItem.touchList.Count > 0 && mf.touch.fingerId == touchItem.touchList[0].fingerId)
                        {
                            touchItem.OnTouchDrag(mf.touch);
                        }
                    }
                    else if (mf.touch.phase == TouchPhase.Ended)
                    {
                        touchItem.OnTouchEnd(mf.touch);
                        mf.id = -1;
                        mf.touchTrans = null;
                    }
                }
            }
        }
    }


    EventSystem eventSystem;
    public GraphicRaycaster RaycastInCanvas;//Canvas上有这个组件
    Transform CheckGuiRaycastObjects(Vector3 point)
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.pressPosition = point;
        eventData.position = point;
        List<RaycastResult> list = new List<RaycastResult>();
        RaycastInCanvas.Raycast(eventData, list);
        Transform thistrnas = null;
        if (list.Count > 0)
        {
            bool isEnable = true;
            for (int i = 0; i < list.Count; i++)
            {
                //当被遮挡物体名字为Scroll View时，不可拖拽
                if (list[i].gameObject.name == "Scroll View")
                {
                    isEnable = false;
                    break;
                }
            }
            if (isEnable)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].gameObject.tag == "Player")
                    {
                        thistrnas = list[i].gameObject.transform;
                        break;
                    }
                }
            }
        }
        return thistrnas;
    }

    public Vector3 GetWorldPos(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 10));
    }


}