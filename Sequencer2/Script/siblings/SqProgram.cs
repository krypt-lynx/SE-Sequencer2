using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    public class SqProgram : ISerializable
    {
        public int currentCommand = 0;
        public string Name;
        public float TimeToWait = 0;
        public int _cycle = 0;

        public List<SqCommand> Commands;

        public bool IsExecuting {
            get
            {
                return currentCommand != 0; //todo?
            }
        }

        public SqProgram(string name, IEnumerable<SqCommand> commands)
        {
            Name = name;
            Commands = new List<SqCommand>(commands);
        }

        public SqProgram() { }

        public void Deserialize(Deserializer decoder)
        {
            currentCommand = decoder.ReadInt();
            Name = decoder.ReadString();
            TimeToWait = decoder.ReadFloat();
            _cycle = decoder.ReadInt();
            Commands = decoder.ReadList(Commands, () => new SqCommand());            
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(currentCommand)
                .Write(Name)
                .Write(TimeToWait)
                .Write(_cycle)
                .Write(Commands);            
        }
    } 
   
    #endregion // ingame script end
}
