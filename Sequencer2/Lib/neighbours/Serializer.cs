using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start

    // I don't want to write serializetion also! Lets try to use half measures...

    public class Serializer
    {
        StringBuilder data = new StringBuilder();

        public Serializer Write(int value)
        {
            data.Append('i')
                .Append(value.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(';');

            return this;
        }

        public Serializer Write(string value)
        {
            data.Append('s')
                .Append(value.Length.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(',')
                .Append(value.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(';');

            return this;
        }

        public Serializer Write(bool value)
        {
            data.Append('b')
                .Append(value.ToString())
                .Append(';');

            return this;
        }

        public Serializer Write(double value)
        {
            data.Append('d')
                .Append(value.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Append(';');

            return this;
        }


        public override string ToString()
        {
            return data.ToString();
        }
    }

    public class Deserializer : IDisposable
    {

        private string data;
        private IEnumerator<char> e;

        public Deserializer(string data)
        {
            this.data = data; 
            e = data.GetEnumerator();
        }

        private void TestType(char type)
        {
            if (!e.MoveNext() || e.Current != type)
            {
                throw new InvalidFormatException();
            }
        }

        private StringBuilder ReadValue(char type)
        {
            TestType(type);

            StringBuilder str = new StringBuilder();
            while (e.MoveNext() && e.Current != ';')
            {
                str.Append(e.Current);
            }

            return str;
        }

        public int ReadInt()
        {
            StringBuilder str = ReadValue('i');

            return int.Parse(str.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }


        public double ReadDouble()
        {
            StringBuilder str = ReadValue('d');

            return double.Parse(str.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        }

        internal bool ReadBool()
        {
            StringBuilder str = ReadValue('b');

            return bool.Parse(str.ToString());
        }

        public string ReadString()
        {
            TestType('s');

            StringBuilder str = new StringBuilder();
            while (e.MoveNext() && e.Current != ',')
            {
                str.Append(e.Current);
            }
            int len = int.Parse(str.ToString(), System.Globalization.CultureInfo.InvariantCulture);

            str.Clear();
            while (len > 0 && e.MoveNext())
            {
                str.Append(e.Current);
                len--;
            }

            e.MoveNext();

            return str.ToString();
        }

        public void Dispose()
        {
            e.Dispose();
            e = null;
            data = null;
        }
    }

    class InvalidFormatException : Exception
    {

    }

    #endregion // ingame script end
}
