using UnityEngine;

namespace MMGame
{
    /// <summary>
    /// Game object 的公共数据池。
    /// 在 Behavior Tree 中也被称为 blackboard。
    /// 通常使用 Set 和 Get<T> 即可。对性能要求苛刻时可以对 value 类型使用专用方法。
    /// 注意 TryGetValue() 的开销比拆装箱还要高一些。
    ///
    /// IDataPool 之所以放在 Core 中是为了性能考虑。在 IBlackborad 中正确的做法是将
    /// gameObject 传递给方法，但是每次都要用 GetComponent 取出 DataPool 开销较大，
    /// 故将 IDataPool 作为核心依赖。
    /// </summary>
    public interface IDataPool
    {
        // int
        IDataPool SetInt(string name, int value);
        int GetInt(string name, int defaultValue = 0);

        // float
        IDataPool SetFloat(string name, float value);
        float GetFloat(string name, float defaultValue = 0);

        // bool
        IDataPool SetBool(string name, bool value);
        bool GetBool(string name, bool defaultValue = false);

        // string
        IDataPool SetString(string name, string value);
        string GetString(string name, string defaultValue = "");

        // Vector2
        IDataPool SetVector2(string name, Vector2 value);
        Vector2 GetVector2(string name, Vector2 defaultValue = default(Vector2));

        // Vector3
        IDataPool SetVector3(string name, Vector3 value);
        Vector3 GetVector3(string name, Vector3 defaultValue = default(Vector3));

        // Quaternion
        IDataPool SetQuaternion(string name, Quaternion value);
        Quaternion GetQuaternion(string name, Quaternion defaultValue = default(Quaternion));

        // object
        IDataPool Set(string name, object value);
        T Get<T>(string name, object defaultValue = null);
    }
}