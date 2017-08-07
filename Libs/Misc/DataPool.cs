using UnityEngine;
using System.Collections.Generic;

namespace MMGame
{
    /// <summary>
    /// Game object 的公共数据池，对 IDataPool 的 MMGame 实现。
    /// </summary>
    public class DataPool : MonoBehaviour, IDataPool
    {
        private Dictionary<string, int> intDic = new Dictionary<string, int>();
        private Dictionary<string, float> floatDic = new Dictionary<string, float>();
        private Dictionary<string, string> stringDic = new Dictionary<string, string>();
        private Dictionary<string, bool> boolDic = new Dictionary<string, bool>();
        private Dictionary<string, Vector2> v2Dic = new Dictionary<string, Vector2>();
        private Dictionary<string, Vector3> v3Dic = new Dictionary<string, Vector3>();
        private Dictionary<string, Quaternion> quatDic = new Dictionary<string, Quaternion>();

        private Dictionary<string, object> objDic = new Dictionary<string, object>();

        // int
        public IDataPool SetInt(string name, int value)
        {
            if (intDic.ContainsKey(name))
            {
                intDic[name] = value;
            }
            else
            {
                intDic.Add(name, value);
            }

            return this;
        }

        public int GetInt(string name, int defaultValue = 0)
        {
            int result;

            if (intDic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // float
        public IDataPool SetFloat(string name, float value)
        {
            if (floatDic.ContainsKey(name))
            {
                floatDic[name] = value;
            }
            else
            {
                floatDic.Add(name, value);
            }

            return this;
        }

        public float GetFloat(string name, float defaultValue = 0)
        {
            float result;

            if (floatDic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // bool
        public IDataPool SetBool(string name, bool value)
        {
            if (boolDic.ContainsKey(name))
            {
                boolDic[name] = value;
            }
            else
            {
                boolDic.Add(name, value);
            }

            return this;
        }

        public bool GetBool(string name, bool defaultValue = false)
        {
            bool result;

            if (boolDic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // string
        public IDataPool SetString(string name, string value)
        {
            if (stringDic.ContainsKey(name))
            {
                stringDic[name] = value;
            }
            else
            {
                stringDic.Add(name, value);
            }

            return this;
        }

        public string GetString(string name, string defaultValue = "")
        {
            string result;

            if (stringDic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // Vector2
        public IDataPool SetVector2(string name, Vector2 value)
        {
            if (v2Dic.ContainsKey(name))
            {
                v2Dic[name] = value;
            }
            else
            {
                v2Dic.Add(name, value);
            }

            return this;
        }

        public Vector2 GetVector2(string name, Vector2 defaultValue = default(Vector2))
        {
            Vector2 result;

            if (v2Dic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // Vector3
        public IDataPool SetVector3(string name, Vector3 value)
        {
            if (v3Dic.ContainsKey(name))
            {
                v3Dic[name] = value;
            }
            else
            {
                v3Dic.Add(name, value);
            }

            return this;
        }

        public Vector3 GetVector3(string name, Vector3 defaultValue = default(Vector3))
        {
            Vector3 result;

            if (v3Dic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // Quaternion
        public IDataPool SetQuaternion(string name, Quaternion value)
        {
            if (quatDic.ContainsKey(name))
            {
                quatDic[name] = value;
            }
            else
            {
                quatDic.Add(name, value);
            }

            return this;
        }

        public Quaternion GetQuaternion(string name, Quaternion defaultValue = default(Quaternion))
        {
            Quaternion result;

            if (quatDic.TryGetValue(name, out result))
            {
                return result;
            }

            return defaultValue;
        }

        // object
        public IDataPool Set(string name, object value)
        {
            if (objDic.ContainsKey(name))
            {
                objDic[name] = value;
            }
            else
            {
                objDic.Add(name, value);
            }

            return this;
        }

        public T Get<T>(string name, object defaultValue = null)
        {
            object result;

            if (objDic.TryGetValue(name, out result))
            {
                return (T) result;
            }

            return (T) defaultValue;
        }
    }
}