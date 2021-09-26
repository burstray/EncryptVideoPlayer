
using MTFrame.MTPool;
using UnityEngine;

/// <remarks>对象池管理类</remarks>
public class PoolManager:MonoBehaviour
{
    public static PoolManager Instance;

    public static ButtonPool buttonPool;
    public GameObject ButtonPrefabs;

    public static TextPool textPool;
    public GameObject TextPrefabs;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        buttonPool = new ButtonPool();
        buttonPool.Init();

        textPool = new TextPool();
        textPool.Init();
    }

    /// <summary>
    /// 放回对象池
    /// </summary>
    public void AddPool(PoolType poolType,GameObject go)
    {
        switch (poolType)
        {
            case PoolType.Button:
                buttonPool.AddPool(go);
                break;
            case PoolType.Text:
                textPool.AddPool(go);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 获取对象
    /// </summary>
    public GameObject GetPool(PoolType poolType)
    {
        GameObject t;
        switch (poolType)
        {
            case PoolType.Button:
                t = buttonPool.GetPool();
                if (t == null)
                {
                    t = Instantiate(ButtonPrefabs);
                    buttonPool.UsePool.Add(t);
                }
                break;
            case PoolType.Text:
                t = textPool.GetPool();
                if (t == null)
                {
                    t = Instantiate(TextPrefabs);
                    textPool.UsePool.Add(t);
                }
                break;
            default:
                t = null;
                break;
        }
        return t;
    }

    private void OnDestroy()
    {
        buttonPool.Clear();
        textPool.Clear();
    }
}