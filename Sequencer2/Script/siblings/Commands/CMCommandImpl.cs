using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;

namespace Script
{
    #region ingame script start

    enum InputEvent
    {
        Press = 0,
        Release,
    }

    class CMMapper : ISerializable
    {
        public const string LOG_CAT = "cmh";

        public static CMMapper Shared = new CMMapper();

        //List<Tuple<string, InputEvent, string>> actions = new List<Tuple<string, InputEvent, string>>();
        Dictionary<string, Dictionary<InputEvent, HashSet<string>>> actions = new Dictionary<string, Dictionary<InputEvent, HashSet<string>>>();

        public bool HasBindings()
        {
            return actions.Count > 0;
        }

        public void RegisterCM()
        {
            Log.Write(LOG_CAT, LogLevel.Verbose, $"RegisterCM");
            Program.Current.Me.SetValue<int>("ControlModule.InputCheck", 1);
            Program.Current.Me.SetValue<int>("ControlModule.InputState", 1); 

        }

        public void UnregisterCM()
        {
            Log.Write(LOG_CAT, LogLevel.Verbose, $"UnregisterCM");

            var name = Program.Current.Me.CustomName;
            int index = name.IndexOf("{ControlModule:");
            if (index != -1)
            {
                name = name.Substring(0, index).TrimEnd(' ');
                Program.Current.Me.CustomName = name;
            }
        }

        public void Add(string action, InputEvent _event, string method)
        {
            Log.Write(LOG_CAT, LogLevel.Verbose, $"add action: {action} {_event}");
            if (actions.Count == 0)
            {
                RegisterCM();
            }

            if (!actions.ContainsKey(action))
            {
                actions[action] = new Dictionary<InputEvent, HashSet<string>>();
            }
            if (!actions[action].ContainsKey(_event))
            {
                actions[action][_event] = new HashSet<string>();
            }
            actions[action][_event].Add(method);

            Program.Current.Me.SetValue("ControlModule.AddInput", action); // todo
        }

        public void Remove(string action, InputEvent? _event, string method)
        {
            if (action == "all")
            {
                Clear();
                return;
            }

            if (actions.ContainsKey(action))
            {
                if (_event != null)
                {
                    if (actions[action].ContainsKey(_event.Value))
                    {
                        if (method != null)
                        {
                            var methods = actions[action][_event.Value];
                            methods.Remove(method);
                            if (methods.Count == 0)
                            {
                                actions[action].Remove(_event.Value);
                            }
                        }
                        else
                        {
                            actions[action].Remove(_event.Value);
                        }

                        if (actions[action].Count == 0)
                        {
                            actions.Remove(action);
                        }
                    }
                } 
                else
                {
                    actions.Remove(action);
                }
            }

            //Program.Current.Me.SetValue("ControlModule.RemoveInput", action);

            //    UnregisterCM();

        }

        public void Clear()
        {
            actions.Clear();
            Program.Current.Me.SetValue("ControlModule.RemoveInput", "all");
            UnregisterCM();
        }

        HashSet<string> lastActions = new HashSet<string>();

        internal void HandleInputs(RuntimeTask runtime)
        {
            var inputs = Program.Current.Me.GetValue<Dictionary<string, object>>("ControlModule.Inputs");

            foreach (var action in actions)
            {
                if (action.Value.ContainsKey(InputEvent.Press))
                {
                    var methods = action.Value[InputEvent.Press];
                    if (inputs.ContainsKey(action.Key) &&
                       !lastActions.Contains(action.Key))
                    {
                        foreach (var method in methods)
                        {
                            runtime.StartProgram(method);
                        }
                    }
                }
                if (action.Value.ContainsKey(InputEvent.Release))
                {
                    var methods = action.Value[InputEvent.Release];
                    if (!inputs.ContainsKey(action.Key) &&
                       lastActions.Contains(action.Key))
                    {
                        foreach (var method in methods)
                        {
                            runtime.StartProgram(method);
                        }
                    }
                }                  
            }

            lastActions = new HashSet<string>(inputs.Keys);            
        }

        public void Serialize(Serializer encoder)
        {
            encoder.Write(actions, i => encoder
                .Write(i.Key)
                .Write(i.Value, j => encoder
                    .Write(j.Key)
                    .Write(j.Value, k => encoder.Write(k))));
        }

        public void Deserialize(Deserializer decoder)
        {
            actions = decoder.ReadCollection(
                () => new Dictionary<string, Dictionary<InputEvent, HashSet<string>>>(),
                () => new KeyValuePair<string, Dictionary<InputEvent, HashSet<string>>>(
                    decoder.ReadString(),
                    decoder.ReadCollection(
                        () => new Dictionary<InputEvent, HashSet<string>>(),
                        () => new KeyValuePair<InputEvent, HashSet<string>>(
                            decoder.ReadEnum<InputEvent>(),
                            decoder.ReadCollection(
                                () => new HashSet<string>(),
                                () => decoder.ReadString()
                            )))));
        }
    }

    class CMCommandImpl
    {
        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
                new CommandRef("cm_addinput", new ParamRef[] {
                    new ParamRef(ParamType.String), // input
                    new ParamRef(ParamType.InputAction, true, InputEvent.Press), // action
                    new ParamRef(ParamType.String), // method
                }, CM_AddInput, SqRequirements.ControlModule),
                new CommandRef("cm_removeinput", new ParamRef[] {
                    new ParamRef(ParamType.String), // input
                    new ParamRef(ParamType.InputAction, true, null), // action
                    new ParamRef(ParamType.String, true, null), // method
                }, CM_RemoveInput, SqRequirements.ControlModule),
                new CommandRef("cm_clearinputs", new ParamRef[] {
                }, CM_ClearInputs, SqRequirements.ControlModule),
                new CommandRef("cm_setfilter", new ParamRef[] {
                    new ParamRef (ParamType.String), // filter
                }, CM_SetFilter, SqRequirements.ControlModule),
                };
        }

        public static void CM_AddInput(IList args, IMethodContext context)
        {
            var action = (string)args[0];
            var _event = (InputEvent)args[1];
            var method = (string)args[2];

            CMMapper.Shared.Add(action, _event, method);

        }

        public static void CM_RemoveInput(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("cm_removeInput", args);
            var action = (string)args[0];
            var _event = (InputEvent?)args[1];
            var method = (string)args[2];

            CMMapper.Shared.Remove(action, _event, method);
        }

        public static void CM_ClearInputs(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("cm_clearinputs", args);

            CMMapper.Shared.Clear();
        }
        

        public static void CM_SetFilter(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("cm_setFilter", args);
            Program.Current.Me.SetValue("ControlModule.CockpitFilter", (string)args[0]);
        }
    }


    #endregion // ingame script end
}
