using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace Script
{
    /* #override
     * IgnoreFile: true
     */

    #region ingame script start

    class TestCommandImpl
    {
        internal static CommandRef[] Defs()
        {
            return new CommandRef[] {
                new CommandRef("test1", new ParamRef[] {
                    new ParamRef (ParamType.MatchingType),
                    new ParamRef (ParamType.String),
                }, Test1),
                new CommandRef("test2", new ParamRef[] {
                }, Test2),
            };
        }

        internal static void Test2(IList args, IMethodContext context)
        {
            var surface = Program.Current.Me.GetSurface(0);
            var sprites = new List<string>();
            surface.GetSprites(sprites);
            //sprites.ForEach(x => Log.Write(x));
            var frame = surface.DrawFrame();
            
            frame.Add(new MySprite(SpriteType.TEXTURE, "Construction", new Vector2(0, 0), new Vector2(100, 100)));
            //frame.Dispose();
        }
        //        static blocks 

        internal static void Test1(IList args, IMethodContext context)
        {
            ImplLogger.LogImpl("test1", args);

            MatchingType type = (MatchingType)args[0];
            string filter = (string)args[1];

            List<IMyTextSurfaceProvider> blocks = new List<IMyTextSurfaceProvider>();
            BlockSelector.GetBlocksOfTypeWithQuery(type, filter, blocks);
            ImplLogger.LogBlocks(blocks);


            Log.Write("TypeName/SubtypeName \"Name\" [IntityId]");
            Log.WriteLine();
            foreach (var lcd in blocks)
            {
                var block = lcd as IMyTerminalBlock;
                Log.Write($"{block.GetType().Name}/{block.BlockDefinition.SubtypeName} \"{block.CustomName}\" [{block.EntityId}]");
                Log.WriteFormat("surfaces count: {0}", lcd.SurfaceCount);
            }

           
        }
    }

    #endregion // ingame script end
}
