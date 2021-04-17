//血条类型 后续区分
public enum HPType
{
    Monster,
    People,
    Boss,
    //Trap,
}

//Hp显示接口
public interface IHpBarBase
{
    void Init(object info, HPType type);
    void Recycle();
    void SetHpInfo(object info);
    void HpUpdate(object info);
    void SetState(bool state);
    void Dispose();
}