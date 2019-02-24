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

            int count = decoder.ReadInt();
            Commands = new List<SqCommand>(count);

            while (count-- > 0)
            {
                Commands.Add(new SqCommand(decoder));
            }
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(currentCommand)
                .Write(Name)
                .Write(TimeToWait)
                .Write(_cycle);

            encoder.Write(Commands.Count);

            foreach (var cmd in Commands)
            {
                cmd.Serialize(encoder);
            }
        }
    } 
   
    #endregion // ingame script end
}
