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

    class CMCommandImpl
    {
        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
                new CommandRef("addinput", new ParamRef[] {
                    new ParamRef(ParamType.String), // input
                    new ParamRef(ParamType.InputEvent, true, InputEvent.Press), // action
                    new ParamRef(ParamType.String), // method
                }, AddInput, SqRequirements.ControlModule),
                new CommandRef("removeinput", new ParamRef[] {
                    new ParamRef(ParamType.String), // input
                    new ParamRef(ParamType.InputEvent, true, null), // action
                    new ParamRef(ParamType.String, true, null), // method
                }, RemoveInput, SqRequirements.ControlModule),
                new CommandRef("clearinputs", new ParamRef[] {
                }, ClearInputs, SqRequirements.ControlModule),
                new CommandRef("setCMFilter", new ParamRef[] {
                    new ParamRef (ParamType.String), // filter
                }, SetFilter, SqRequirements.ControlModule),
                };
        }

        public static void AddInput(IList args, IMethodContext context)
        {
            var action = (string)args[0];
            var _event = (InputEvent)args[1];
            var method = (string)args[2];

            CMMapper.Shared.Add(action, _event, method);

        }

        public static void RemoveInput(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("cm_removeInput", args);
            var action = (string)args[0];
            var _event = (InputEvent?)args[1];
            var method = (string)args[2];

            CMMapper.Shared.Remove(action, _event, method);
        }

        public static void ClearInputs(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("cm_clearinputs", args);

            CMMapper.Shared.Clear();
        }
        

        public static void SetFilter(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("cm_setFilter", args);
            Program.Current.Me.SetValue("ControlModule.CockpitFilter", (string)args[0]);
        }
    }


    #endregion // ingame script end
}
