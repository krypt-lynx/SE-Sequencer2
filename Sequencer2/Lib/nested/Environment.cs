#define Simulation
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    partial class Program 
    {
#if !Simulation

        #region ingame script start

        Exception lastException = null;
        public static MyGridProgram Current;

        public void IsolatedRun(Action work)
        {
            if (lastException == null)
            {
                try
                {
                    Current = this;
                    Log.NewFrame();

                    work();

                    Current = null;
                }
                catch (Exception e)
                {
                    lastException = e;
                }
            }

            if (lastException != null)
            {
                EchoException();
                return;
            }
        }

        private void EchoException()
        {
            Echo("Exception handled!");
            Echo("Please, make screenshot of this message and report the issue to developer");
            Echo("To recover recompile the script");
            Echo(lastException.GetType().Name);
            Echo(lastException.Message);
            Echo(lastException.StackTrace);
        }

        #endregion // ingame script end

#else
        Exception lastException = null;
        public static MyGridProgram Current;

        public void IsolatedRun(Action work)
        {
            Current = this;
            Log.NewFrame();

            work();

            Current = null;
        }
#endif
    }

}
