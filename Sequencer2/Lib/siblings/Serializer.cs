using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    // check git history for unminified version

    /* #override
     * Minify: true
     */

    #region ingame script start
                

    public class Serializer
    {
        StringBuilder d = new StringBuilder();
        
        void A<T>(T c) { d.Append(c); }

        public Serializer Write(int v)
        {
            A('i');
            A(v.ToString(C.I));
            A(';');

            return this;
        }

        public Serializer Write(string v)
        {
            A('s');
            return WriteStringInternal(v);
        }

        public Serializer WriteStringInternal(string v)
        {
            if (v == null)
            {
                A(";");
            }
            else
            {
                A(v.Length.ToString(C.I));
                A(',');
                A(v.ToString(C.I));
                A(';');
            }
            return this;
        }

        public Serializer Write(bool v)
        {
            A('b');
            A(v.ToString(C.I));
            A(';');

            return this;
        }

        public Serializer Write(double v)
        {
            A('d');
            A(v.ToString(C.I));
            A(';');

            return this;
        }

        public Serializer Write(float v)
        {
            A('f');
            A(v.ToString(C.I));
            A(';');

            return this;
        }

        public Serializer Write(uint v)
        {
            A('u');
            A(v.ToString(C.I));
            A(';');

            return this;
        }

        public Serializer Write(ISerializable v)
        {
            A("o{");
            v.Serialize(this);
            A("};");

            return this;
        }

        public Serializer Write(Enum v)
        {
            A('e');
            A(Convert.ToUInt64(v));
            A(';');
            return this;
        }

        public Serializer Write(ICollection v)
        {
            A('c');
            if (v != null)
            {
                A('{');
                Write(v.Count);
                foreach (var i in v)
                {
                    Write(i);
                }
                A('}');
            }
            A(';');
            return this;
        }

        public Serializer Write<K, V>(Dictionary<K, V> v)
        {
            A('c');
            if (v != null)
            {
                A('{');
                Write(v.Count);
                foreach (var kvp in v)
                {
                    Write(kvp.Key);
                    Write(kvp.Value);
                }
                A('}');
            }
            A(';');
            return this;
        }

        public Serializer Write(object v)
        {
            var cases = new Dictionary<Type, Action> {
                { typeof(int), () => Write((int)v) },
                { typeof(string), () => Write((string)v) },
                { typeof(bool), () => Write((bool)v) },
                { typeof(double), () => Write((double)v) },
                { typeof(float), () => Write((float)v) },
                { typeof(uint), () => Write((uint)v) },
            };             

            var type = v.GetType();

            if (v is Enum)
            {
                Write(v as Enum);
            } else if (v is ISerializable)
            {
                Write((ISerializable)v);
            } else if (cases.ContainsKey(type))
            {
                cases[type]();
            }
            else 
            {
                throw new UnsupportedOperationException();
            }

            return this;
        }
  
        //public Ser
        public override string ToString()
        {
            return d.ToString();
        }
    }

    public class Deserializer : IDisposable
    {
        string d;
        IEnumerator<char> e;

        public Deserializer(string s)
        {
            d = s; 
            e = s.GetEnumerator();
        }

        bool a = false; // Automatic type resolving
        void T(char t) // TestType
        {
            if (!a && (!e.MoveNext() || e.Current != t))
            {
                throw new InvalidFormatException($"expected '{t}' got '{e.Current}'");
            }
        }

        StringBuilder R(char t) // ReadValue
        {
            T(t);

            var s = new StringBuilder();
            while (e.MoveNext() && e.Current != ';')
            {
                s.Append(e.Current);
            }

            return s;
        }

        public int ReadInt()
        {
            var s = R('i');

            return int.Parse(s.ToString(), C.I);
        }

        public uint ReadUInt()
        {
            var s = R('u');

            return uint.Parse(s.ToString(), C.I);
        }

        public double ReadDouble()
        {
            var s = R('d');

            return double.Parse(s.ToString(), C.I);
        }

        public float ReadFloat()
        {
            var s = R('f');

            return float.Parse(s.ToString(), C.I);
        }

        public bool ReadBool()
        {
            var s = R('b');

            return bool.Parse(s.ToString());
        }

        public string ReadString()
        {
            T('s');
            return ReadStringInternal();
        }

        public string ReadStringInternal()
        {
            var s = new StringBuilder();
            while (e.MoveNext() && e.Current != ',' && e.Current != ';')
            {
                s.Append(e.Current);
            }

            if (s.Length == 0)
            {
                return null;
            }
            int l = int.Parse(s.ToString(), C.I);

            s.Clear();
            while (l > 0 && e.MoveNext())
            {
                s.Append(e.Current);
                l--;
            }

            e.MoveNext();

            return s.ToString();
        }
        
        private object ReadEnum(Func<Type> t)
        {
            var v = R('e');
            return Enum.ToObject(t.Invoke(), ulong.Parse(v.ToString(), C.I));
        }

        private bool TryReadEnum<TEnum>(out object e)
        {
            try
            {
                var v = R('e');
                e = Enum.ToObject(typeof(TEnum), ulong.Parse(v.ToString(), C.I));
                return true;
            } catch
            {
                e = null;
                return false;
            }            
        }

        public S ReadObject<S>() where S : ISerializable, new()
        {
            S o = new S();
            return ReadObject(o);
        }
        
        public S ReadObject<S>(S o) where S : ISerializable
        {
            T('o');
            e.MoveNext();
            o.Deserialize(this);
            e.MoveNext();
            e.MoveNext();
            return o;
        }

        public T Read<T>(Func<Type> innerType = null) {
            var cases = new Dictionary<Type, Func<object>> {
                { typeof(int), () => ReadInt() },
                { typeof(string), () => ReadString() },
                { typeof(bool), () => ReadBool() },
                { typeof(double), () => ReadDouble() },
                { typeof(float), () => ReadFloat() },
                { typeof(uint), () => ReadUInt() },
            };

            var type = typeof(T);
            object t = null;

            if (type == typeof(object))
            {
                t = ReadAuto<T>(innerType);
            } else if (cases.ContainsKey(type))
            {
                t = cases[type]();
            }/*
            else if (typeof(ISerializable).IsAssignableFrom(type)) // <-- there
            {
                throw new InvalidOperationException();

                ISerializable test = default(T) as ISerializable;
                ReadObject(test);
             // 
             // t = ReadObject(new T()); // <-- and cast it there somehow 
            }*/
            else //if (type.IsEnum) // ???
            {
                bool r = TryReadEnum<T>(out t);
                if (!r)
                {
                    throw new InvalidCastException();
                }
                //t = ReadEnum<T>();
            }
            /*else
            {
                throw new InvalidCastException();
            }*/

            return (T)t;
        }

        public object ReadAuto<T>(Func<Type> t = null)
        {
            e.MoveNext();
            var typeCode = e.Current;
            var cases = new Dictionary<char, Func<object>> {
                { 'i' , () => ReadInt() },
                { 'u' , () => ReadUInt() },
                { 'd' , () => ReadDouble() },
                { 'f' , () => ReadFloat() },
                { 'b' , () => ReadBool() },
                { 's' , () => ReadString() },
                { 'e' , () => ReadEnum(t) },
                // 'o'
                // 'c'
            };

            if (!cases.ContainsKey(typeCode))
            {
                throw new UnsupportedOperationException();
            }
            a = true;
            var result = cases[typeCode]();
            a = false;
            return result;
        }


        public void Dispose()
        {
            e.Dispose();
            e = null;
            d = null;
        }

        bool ReadCollection(Action clean, Action readItem)
        {
            T('c');
            e.MoveNext();
            if (e.Current != '{')
            {
                return false;
            }

            clean();

            int count = ReadInt();
            while (count-- > 0)
            {
                readItem();
            }
            e.MoveNext();
            e.MoveNext();

            return true;
        }

        public Dictionary<K, V> ReadDictionary<K, V>(Dictionary<K, V> o, Func<K> createKey = null, Func<V> createValue = null, Func<Type> innerType = null)
        {
            return ReadCollection(() =>
            {
                o?.Clear();
                o = o ?? new Dictionary<K, V>();
            }, () =>
            {
                K k = createKey != null ? (K)ReadObject((ISerializable)createKey()) : Read<K>(innerType);
                V v = createValue != null ? (V)ReadObject((ISerializable)createValue()) : Read<V>(innerType);
                o[k] = v;
            }) ? o : null;
        }

        internal List<T> ReadList<T>(List<T> o, Func<T> createItem = null, Func<Type> innerType = null)
        {
            return ReadCollection(() =>
            {
                o?.Clear();
                o = o ?? new List<T>();
            }, () =>
            {
                o.Add(createItem != null ? (T)ReadObject((ISerializable)createItem()) : Read<T>(innerType));
            }) ? o : null;
        }

        internal Queue<T> ReadQueue<T>(Queue<T> o, Func<T> createItem = null)
        {
            return ReadCollection(() =>
            {
                o?.Clear();
                o = o ?? new Queue<T>();
            }, () =>
            {
                o.Enqueue(createItem != null ? (T)ReadObject((ISerializable)createItem()) : Read<T>());
            }) ? o : null;
        }
    }

    class UnsupportedOperationException : Exception { }

    class InvalidFormatException : Exception
    {
        public InvalidFormatException(string message) : base(message) { }
    }

    public interface ISerializable
    {
        void Serialize(Serializer encoder);
        void Deserialize(Deserializer decoder);
    }
    
    #endregion // ingame script end
}
