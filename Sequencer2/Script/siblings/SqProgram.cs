using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    class SqProgram : IMethodContext, ISerializable 
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

        public RuntimeTask Runtime { get; set; }

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
            Commands = decoder.ReadCollection(() => new List<SqCommand>(), () => decoder.ReadObject<SqCommand>());            
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(currentCommand)
                .Write(Name)
                .Write(TimeToWait)
                .Write(_cycle)
                .Write(Commands, i => encoder.Write(i));            
        }

        public void Wait(int seconds)
        {
            //System.Diagnostics.Debug.Assert(TimeToWait == 0);
            TimeToWait = seconds;
        }

        public void Goto(int line)
        {
            currentCommand = line - 1; // todo: remove -1
        }

        public void Set(string name, double value)
        {
            VariablesStorage.Shared.SetVariable(name, value);
        }

        public double Get(string name)
        {
            return VariablesStorage.Shared.GetVariable(name) ?? 0;
        }
    } 
   
    #endregion // ingame script end
}
