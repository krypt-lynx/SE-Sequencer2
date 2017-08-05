using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    class VariablesStorage
    {
        public VariablesStorage() { }

        public VariablesStorage(Deserializer decoder)
        {
            variables = new Dictionary<string, double>();

            int count = decoder.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = decoder.ReadString();
                double value = decoder.ReadDouble();

                variables[key] = value;
            }
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(variables.Count);

            foreach (var kvp in variables)
            {
                encoder.Write(kvp.Key);
                encoder.Write(kvp.Value);
            }
        }

        public static void Deserialize(Deserializer decoder)
        {
            _shared = new VariablesStorage(decoder);
        }

        static VariablesStorage _shared = new VariablesStorage();
        public static VariablesStorage Shared { get { return _shared; } }

        Dictionary<string, double> variables = new Dictionary<string, double>();

        public double? GetVariable(string name)
        {
            return variables.ContainsKey(name) ? variables[name] : (double?)null;
        }

        public void SetVariable(string name, double value)
        {
            variables[name] = value;
        }

        internal static void Clear()
        {
            _shared = new VariablesStorage();
        }
    }

    #endregion // ingame script end
}
