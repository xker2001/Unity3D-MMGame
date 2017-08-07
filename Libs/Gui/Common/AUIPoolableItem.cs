namespace MMGame.UI
{
    /// <summary>
    /// 可根据外部数据从对象池创建的 UI 控件。
    /// </summary>
    abstract public class AUIPoolableItem : PoolBehaviour
    {
        abstract public void SetData(UIPoolableItemData data);
    }
}