using System;
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
                A(",;");
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
            A('}');

            return this;
        }

        public Serializer WriteEnum(Enum v)
        {
            var e = v.ToString();
            A('e');
            return WriteStringInternal(e);
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
                WriteEnum(v as Enum);
            } else if (v is ISerializable)
            {
                Write((ISerializable)v);
            } else if (cases.ContainsKey(type))
            {
                cases[type]();
            }
            else 
            {
                throw new InvalidCastException();
            }

            return this;
        }

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

        void T(char t) // TestType
        {
            if (!e.MoveNext() || e.Current != t)
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
            while (e.MoveNext() && e.Current != ',')
            {
                s.Append(e.Current);
            }

            if (s.Length == 0)
            {
                e.MoveNext();
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
        
        private TEnum ReadEnum<TEnum>()
        {
            T('e');
            var name = ReadStringInternal();
            return (TEnum)Enum.Parse(typeof(TEnum), name);
        }

        private bool TryReadEnum<TEnum>(out object e)
        {
            T('e');
            var name = ReadStringInternal();
            try {
                e = Enum.Parse(typeof(TEnum), name);
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

        public S ReadObject<S>(out S o) where S : ISerializable
        {
            o = default(S);
            ReadObject(o);
            return o;
        }

        public S ReadObject<S>(S o) where S : ISerializable
        {
            T('o');
            e.MoveNext();
            o.Deserialize(this);
            e.MoveNext();
            return o;
        }

        public T Read<T>() {
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

            if (cases.ContainsKey(type))
            {
                t = cases[type]();
            }
            else if (typeof(ISerializable).IsAssignableFrom(type)) // <-- there
            {
                ISerializable test = default(T) as ISerializable;
                ReadObject(test);
             // throw new NotImplementedException();
             // t = ReadObject(new T()); // <-- and cast it there somehow 
            }
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


        public void Dispose()
        {
            e.Dispose();
            e = null;
            d = null;
        }
    }

    class InvalidFormatException : Exception
    {
        public InvalidFormatException(string message) : base(message) { }
    }

    public interface ISerializable
    {
        void Serialize(Serializer encoder);
        void Deserialize(Deserializer decoder);
    }

    public class SD<Key, Value> : Dictionary<Key, Value>, ISerializable
    {
        public void Deserialize(Deserializer decoder)
        {
            int count = decoder.ReadInt();
            while (count-- > 0)
            {
                var key = decoder.Read<Key>();
                var value = decoder.Read<Value>();

                this[key] = value;
            }
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(Count);
            foreach (var kvp in this)
            {
                encoder.Write(kvp.Key)
                       .Write(kvp.Value);
            }
        }
    }

    public class SL<T> : List<T>, ISerializable
    {
        public void Deserialize(Deserializer decoder)
        {
            int count = decoder.ReadInt();
            while (count-- > 0)
            {
                Add(decoder.Read<T>());
            }
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(Count);
            foreach (var v in this)
            {
                encoder.Write(v);
            }
        }
    }

    public class SQ<T> : Queue<T>, ISerializable
    {
        public void Deserialize(Deserializer decoder)
        {
            int count = decoder.ReadInt();
            while (count-- > 0)
            {
                Enqueue(decoder.Read<T>());
            }
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(Count);
            foreach (var v in this)
            {
                encoder.Write(v);
            }
        }
    }
    #endregion // ingame script end
}
