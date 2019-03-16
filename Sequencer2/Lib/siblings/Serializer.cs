using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

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

        public Serializer Write<T>(ICollection v, Action<T> item)
        {
            if (v != null)
            {
                A("c{");
                Write(v.Count);
                foreach (var i in v)
                {
                    item((T)i);
                }
                A("};");
            }
            else
            {
                A("c;");
            }
            return this;
        }

        public Serializer Write<T>(ICollection<T> v, Action<T> item)
        {
            if (v != null)
            {
                A("c{");
                Write(v.Count);
                foreach (var i in v)
                {
                    item(i);
                }
                A("};");
            }
            else
            {
                A("c;");
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
        IEnumerator<char> e;

        public Deserializer() { }

        public Deserializer(string s)
        {
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

        public TEnum ReadEnum<TEnum>()
        {
            var v = R('e');
            return (TEnum)Enum.ToObject(typeof(TEnum), ulong.Parse(v.ToString(), C.I));
        }

        public S ReadObject<S>() where S : ISerializable, new()
        {
            S o = new S();
            return ReadObject(o);
        }

        public T ReadObject<T>(T o) where T : ISerializable
        {
            this.T('o');
            e.MoveNext();
            o.Deserialize(this);
            e.MoveNext();
            e.MoveNext();
            return o;
        }

        public void Dispose()
        {
            e.Dispose();
            e = null;
        }

        public C ReadCollection<C, T>(Func<C> c, Func<T> i) where C : ICollection<T>
        {
            return ReadCollection((s) => {
                var o = c();
                o.Clear();
                return o;
            }, (o, k) => o.Add(i()));
        }

        public C ReadCollection<C>(Func<C> c, Action<C> i)
        {
            return ReadCollection(s => c(), (o, k) => i(o));
        }

        public C ReadCollection<C>(Func<int, C> c, Action<C, int> i)
        {
            T('c');
            e.MoveNext();
            if (e.Current != '{')
            {
                return default(C);
            }

            int count = ReadInt();

            var o = c(count);

            for (int k = 0; k < count; k++)
            {
                i(o, k);
            }
            e.MoveNext();
            e.MoveNext();

            return o;
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

    #endregion // ingame script end
}
