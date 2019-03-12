using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    class VariablesStorage : ISerializable
    {
        public VariablesStorage() { }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(variables, i => encoder.Write(i.Key).Write(i.Value));            
        }

        public void Deserialize(Deserializer decoder)
        {
            variables = decoder.ReadCollection(
                () => variables,
                () => new KeyValuePair<string, double>(decoder.ReadString(), decoder.ReadDouble())
            );
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
