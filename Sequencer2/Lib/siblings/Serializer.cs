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


    class C
    {
        public static System.Globalization.CultureInfo I = System.Globalization.CultureInfo.InvariantCulture;
    }

    public class Serializer
    {
        StringBuilder d = new StringBuilder();
        

        public Serializer Write(int v)
        {
            d.Append('i')
             .Append(v.ToString(C.I))
             .Append(';');

            return this;
        }

        public Serializer Write(string v)
        {
            d.Append('s')
             .Append(v.Length.ToString(C.I))
             .Append(',')
             .Append(v.ToString(C.I))
             .Append(';');

            return this;
        }

        public Serializer Write(bool v)
        {
            d.Append('b')
             .Append(v.ToString(C.I))
             .Append(';');

            return this;
        }

        public Serializer Write(double v)
        {
            d.Append('d')
             .Append(v.ToString(C.I))
             .Append(';');

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

            StringBuilder s = new StringBuilder();
            while (e.MoveNext() && e.Current != ';')
            {
                s.Append(e.Current);
            }

            return s;
        }

        public int ReadInt()
        {
            StringBuilder s = R('i');

            return int.Parse(s.ToString(), C.I);
        }


        public double ReadDouble()
        {
            StringBuilder s = R('d');

            return double.Parse(s.ToString(), C.I);
        }

        internal bool ReadBool()
        {
            StringBuilder s = R('b');

            return bool.Parse(s.ToString());
        }

        public string ReadString()
        {
            T('s');

            StringBuilder s = new StringBuilder();
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
