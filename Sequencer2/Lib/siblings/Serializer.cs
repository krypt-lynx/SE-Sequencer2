using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    // check git history for unminified version

    /* #override
     * Squeeze: true
     */

    #region ingame script start


    // I don't want to write serializetion also! Lets try to use half measures...


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
            A(v.Length.ToString(C.I));
            A(',');
            A(v.ToString(C.I));
            A(';');

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
                throw new InvalidFormatException();
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


        public double ReadDouble()
        {
            var s = R('d');

            return double.Parse(s.ToString(), C.I);
        }

        internal bool ReadBool()
        {
            var s = R('b');

            return bool.Parse(s.ToString());
        }

        public string ReadString()
        {
            T('s');

            var s = new StringBuilder();
            while (e.MoveNext() && e.Current != ',')
            {
                s.Append(e.Current);
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

        public void Dispose()
        {
            e.Dispose();
            e = null;
            d = null;
        }
    }

    class InvalidFormatException : Exception
    {

    }

    #endregion // ingame script end
}
