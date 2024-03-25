using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.HelperClasses
{
    [Serializable]
    public class StringTextTuple : SerializableKeyValuePair<string, TextAsset> 
    {
        public StringTextTuple(string item1, TextAsset item2) : base(item1, item2) { }
    }

    [Serializable]
    public class StringTextDictionary : SerializableDictionary<string, TextAsset>
    {
        [SerializeField] private List<StringTextTuple> _pairs = new List<StringTextTuple>();

        protected override List<SerializableKeyValuePair<string, TextAsset>> _keyValuePairs
        {
            get
            {
                var list = new List<SerializableKeyValuePair<string, TextAsset>>();
                foreach (var pair in _pairs)
                {
                    list.Add(new SerializableKeyValuePair<string, TextAsset>(pair.Key, pair.Value));
                }
                return list;
            }

            set
            {
                _pairs.Clear();
                foreach (var kvp in value)
                {
                    _pairs.Add(new StringTextTuple(kvp.Key, kvp.Value));
                }
            }
        }
    }

    [Serializable]
    public class StringGameObjectTuple : SerializableKeyValuePair<string, GameObject>
    {
        public StringGameObjectTuple(string item1, GameObject item2) : base(item1, item2) { }
    }

    [Serializable]
    public class StringGameObjectDictionary : SerializableDictionary<string, GameObject>
    {
        [SerializeField] private List<StringGameObjectTuple> _pairs = new List<StringGameObjectTuple>();

        protected override List<SerializableKeyValuePair<string, GameObject>> _keyValuePairs
        {
            get
            {
                var list = new List<SerializableKeyValuePair<string, GameObject>>();
                foreach (var pair in _pairs)
                {
                    list.Add(new SerializableKeyValuePair<string, GameObject>(pair.Key, pair.Value));
                }
                return list;
            }

            set
            {
                _pairs.Clear();
                foreach (var kvp in value)
                {
                    _pairs.Add(new StringGameObjectTuple(kvp.Key, kvp.Value));
                }
            }
        }
    }

    // Taken from https://gist.github.com/bellicapax/0838bfa4cff863d07baf78644a6a6b9b
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        protected abstract List<SerializableKeyValuePair<TKey, TValue>> _keyValuePairs { get; set; }

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            _keyValuePairs.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _keyValuePairs.Add(new SerializableKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            for (int i = 0; i < _keyValuePairs.Count; i++)
            {
                this[_keyValuePairs[i].Key] = _keyValuePairs[i].Value;
            }
        }
    }

    // Based on SerializableTuple from https://github.com/neuecc/SerializableDictionary
    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue> : IEquatable<SerializableKeyValuePair<TKey, TValue>>
    {
        [SerializeField]
        TKey _key;
        public TKey Key { get { return _key; } }

        [SerializeField]
        TValue _value;
        public TValue Value { get { return _value; } }

        public SerializableKeyValuePair()
        {

        }

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this._key = key;
            this._value = value;
        }

        public bool Equals(SerializableKeyValuePair<TKey, TValue> other)
        {
            var comparer1 = EqualityComparer<TKey>.Default;
            var comparer2 = EqualityComparer<TValue>.Default;

            return comparer1.Equals(_key, other._key) &&
                comparer2.Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            var comparer1 = EqualityComparer<TKey>.Default;
            var comparer2 = EqualityComparer<TValue>.Default;

            int h0;
            h0 = comparer1.GetHashCode(_key);
            h0 = (h0 << 5) + h0 ^ comparer2.GetHashCode(_value);
            return h0;
        }

        public override string ToString()
        {
            return String.Format("(Key: {0}, Value: {1})", _key, _value);
        }

    }
}
