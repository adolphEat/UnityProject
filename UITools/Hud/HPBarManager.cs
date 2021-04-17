using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// 血条管理 简单版本
/// HpBarManager -> IHpBarBase  
/// </summary>
public abstract class HPBarManager
{
    //缓存
    protected Dictionary<HPType, Queue<IHpBarBase>> hpCache = new Dictionary<HPType, Queue<IHpBarBase>>();
    
    //血条数据
    protected readonly Dictionary<object, IHpBarBase> hpData = new Dictionary<object, IHpBarBase>();

    protected abstract IHpBarBase CreateHpBar(HPType info);

    private bool TryGetValue(object data, out IHpBarBase bar)
    {
        return hpData.TryGetValue(data, out bar);
    }

    public virtual void HpUpdate()
    {

    }

    protected abstract void RecycleBar(IHpBarBase bar);

    /// <summary>
    /// 创建血条
    /// </summary>
    public void CreateHp(object owner)
    {
        HPType hpType = GetHpOwnerType(owner);
        if (!TryGetValue(owner, out var hpBar))
        {
            if (hpCache.ContainsKey(hpType))
            {
                var cache = hpCache[hpType];
                hpBar = cache.Count > 0 ? cache.Dequeue() : CreateHpBar(hpType);
            }
            else
            {
                hpBar = CreateHpBar(hpType);
            }
        }

        hpBar.Init(owner, hpType);
        hpData[owner] = hpBar;
    }

    /// <summary>
    /// 回收血条
    /// </summary>
    public void Recycle(object owner)
    {
        IHpBarBase hpBar = null;
        if (TryGetValue(owner, out hpBar))
        {
            RecycleBar(hpBar);
            hpBar.Recycle();
            hpData.Remove(owner);
            var hpType = GetHpOwnerType(owner);
            if (hpCache.ContainsKey(hpType))
            {
                hpCache[hpType].Enqueue(hpBar);
            }
            else
            {
                var cache = new Queue<IHpBarBase>();
                cache.Enqueue(hpBar);
                hpCache.Add(hpType, cache);
            }
        }
    }

    protected HPType GetHpOwnerType(object owner)
    {
        HPType hpType = HPType.People;
//        if (owner is CPlayer)
//        {
//            hpType = HPType.People;
//        }
//        else if (owner is Monster)
//        {
//            hpType = PassiveMgr.Ins.IsBoss(owner) ? HPType.Boss : HPType.Monster;
//        }
//        else
//        {
//            hpType = HPType.People;
//        }

        return hpType;
    }
    
    //设置类型状态
    public abstract void SetState(bool state);
}

//通用血条控制
public class HpBarControl : HPBarManager
{
    public static HpBarControl Ins = new HpBarControl();

    protected override IHpBarBase CreateHpBar(HPType info)
    {
        string loadPath = info == HPType.People ? "prefab/HpBar/CommonHpBar" : "prefab/HpBar/MonsterHpBar";
        var obj = ResourceMgr.Ins.LoadObject<GameObject>(loadPath);
        obj = Object.Instantiate(obj);

//        CommonHpBar target;
//        switch (info)
//        {
//            case HPType.Monster:
//                target = obj.TAddomponent<MonsterHpBar>();
//                break;
//            case HPType.People:
//                target = obj.TAddomponent<CommonHpBar>();
//                break;
//            case HPType.Boss:
//                target = obj.TAddomponent<BossHpBar>();
//                break;
//            default:
//                target = obj.TAddomponent<CommonHpBar>();
//                break;
//        }

//        return obj == null ? null : target;

        return null;
    }

    public override void HpUpdate()
    {
        foreach (var it in hpData)
        {
            it.Value.SetHpInfo(it.Key); //更新血条信息 或者相关位置
//            it.Value.HpUpdate(it.Key.creatureInfo); //更新位置
        }
    }

    protected override void RecycleBar(IHpBarBase bar)
    {
        bar.SetState(false);
    }

    public override void SetState(bool state)
    {
        foreach (var it in hpData)
        {
            it.Value.SetState(state);
        }
    }

    public void Dispose()
    {
        foreach (var it in hpData)
        {
            it.Value.Dispose();
        }

        foreach (var it in hpCache)
        {
            var queue = it.Value;
            foreach (var item in queue)
            {
                item.Dispose();
            }
        }
        
        hpData.Clear();
        hpCache.Clear();
    }
}