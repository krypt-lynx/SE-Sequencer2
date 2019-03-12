using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.ObjectBuilders;
using VRageMath;
using VRage.Collections;
using System.Diagnostics;
using console;
using System.IO;

namespace SETestEnv
{

    class TestCubeGrid : IMyCubeGrid
    {
        private List<TestBlock> blocks = new List<TestBlock>();
        public List<TestBlock> Blocks
        {
            get
            {
                return blocks;
            }
        }

        public void RegisterBlock(TestBlock block)
        {
            blocks.Add(block);
            block.OwnerGrid = this;
        }

        public MyEntityComponentContainer Components
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long EntityId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float GridSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MyCubeSize GridSizeEnum
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsStatic
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vector3I Max
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vector3I Min
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BoundingBoxD WorldAABB
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BoundingBoxD WorldAABBHr
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MatrixD WorldMatrix
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BoundingSphereD WorldVolume
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BoundingSphereD WorldVolumeHr
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string CustomName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DisplayName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool HasInventory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int InventoryCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CubeExists(Vector3I pos)
        {
            throw new NotImplementedException();
        }

        public IMySlimBlock GetCubeBlock(Vector3I pos)
        {
         //   try
            {
                return Blocks[(int)(pos.X)];
            }
          /*  catch
            {
                return null;
            }*/

        }

        public Vector3D GetPosition()
        {
            throw new NotImplementedException();
        }

        public Vector3D GridIntegerToWorld(Vector3I gridCoords)
        {
            throw new NotImplementedException();
        }

        public Vector3I WorldToGridInteger(Vector3D coords)
        {
            throw new NotImplementedException();
        }

        public IMyInventory GetInventory()
        {
            throw new NotImplementedException();
        }

        public IMyInventory GetInventory(int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSameConstructAs(IMyCubeGrid other)
        {
            throw new NotImplementedException();
        }
    }

    class TestBlockGroup : IMyBlockGroup
    {
        public string Name { get; set; }
        public List<IMyTerminalBlock> Blocks { get; set; }

        public void GetBlocks(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class
        {
            throw new NotImplementedException();
        }

        public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class
        {
            throw new NotImplementedException();
        }
    }

    class TestGridTerminalSystem : IMyGridTerminalSystem
    {
        public TestCubeGrid CubeGrid;
        public Dictionary<string, TestBlockGroup> Groups;

        public TestProgrammableBlock ownerBlock = null;

        public TestGridTerminalSystem(TestProgrammableBlock owner) : base()
        {
            CubeGrid = new TestCubeGrid();
            this.ownerBlock = owner;
            CubeGrid.RegisterBlock(ownerBlock);
        }

        public void GetBlockGroups(List<IMyBlockGroup> blockGroups, Func<IMyBlockGroup, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public IMyBlockGroup GetBlockGroupWithName(string name)
        {
            return Groups?.ContainsKey(name) ?? false ? Groups[name] : null;
        }

        public void GetBlocks(List<IMyTerminalBlock> blocks)
        {
            blocks.Clear();
            blocks.AddRange(CubeGrid.Blocks);
        }

        public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class
        {
            blocks.Clear();
            blocks.AddRange(CubeGrid.Blocks.Where(x => x is T).Cast<T>().Where(x => collect(x)));
        }


        public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class
        {
            blocks.Clear();
            blocks.AddRange(CubeGrid.Blocks.Where(x => x is T).Where(x => collect(x)));
        }

        public IMyTerminalBlock GetBlockWithId(long id)
        {
            throw new NotImplementedException();
        }

        public IMyTerminalBlock GetBlockWithName(string name)
        {
            return CubeGrid.Blocks.Where(x => x.CustomName == name).FirstOrDefault();
        }

        public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            throw new NotImplementedException();
        }
    }

    class TestGridProgramRuntimeInfo : IMyGridProgramRuntimeInfo
    {
        private int simInstructionCount = 0;

        int ticksPassed = 0;

        public void InitNewRun()
        {
            simInstructionCount = 0;
            if ((updateFrequency & (UpdateFrequency.Once | UpdateFrequency.Update1)) != 0)
            {
                ticksPassed = 1;
            }
            else if ((updateFrequency & UpdateFrequency.Update10) != 0)
            {
                ticksPassed = 10;
            }
            else if ((updateFrequency & UpdateFrequency.Update100) != 0)
            {
                ticksPassed = 100;
            }
            else
            {
                ticksPassed = 60;
            }

            updateFrequency = updateFrequency & ~updateFrequency;
        }

        public int CurrentInstructionCount
        {
            get
            {
                simInstructionCount += 10;
                return simInstructionCount;
            }
        }

        public int CurrentMethodCallCount
        {
            get
            {
                return 0;
            }
        }

        public double LastRunTimeMs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int MaxInstructionCount
        {
            get
            {
                return 50000;
            }
        }

        public int MaxMethodCallCount
        {
            get
            {
                return 50000;
            }
        }

        public TimeSpan TimeSinceLastRun
        {
            get
            {
                return new TimeSpan(0, 0, 0, 0, ticksPassed * 17);
            }
        }

        public int MaxCallChainDepth
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int CurrentCallChainDepth
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        private UpdateFrequency updateFrequency = UpdateFrequency.None;
        public UpdateFrequency UpdateFrequency
        {
            get
            {
                return updateFrequency;
            }

            set
            {
                updateFrequency = value;
                Console2.ForegroundColor = ConsoleColor.Yellow;
                Console2.WriteLine("new UpdateFrequency value: {0}", value);                
            }
        }
    }
    
    abstract class TestGridProgram : MyGridProgram
    {
        static string appRoot = System.AppDomain.CurrentDomain.BaseDirectory;
        const string StorageFile = "Storage.txt";

        public TestGridTerminalSystem TestGridTerminalSystem
        {
            get
            {
                return this.GridTerminalSystem as TestGridTerminalSystem;
            }
        }

        public void SetStorage(string value)
        {
            Storage = value;
        }

        public override string Storage
        {
            get
            {
                string path = Path.Combine(appRoot, StorageFile);
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
                else
                {
                    return "";
                }
            }

            protected set
            {
                Console2.ForegroundColor = ConsoleColor.Yellow;
                Console2.WriteLine("new Storage value:\n{0}", value);
                string path = Path.Combine(appRoot, StorageFile);
                System.IO.File.WriteAllText(path, value);
            }
        }

        public static TestProgrammableBlock FutureOwner = null;
      //  public static string FutureStorage = "";

        public TestGridProgram()
        {
            this.Echo = EchoImpl;
#pragma warning disable 618
            this.ElapsedTime = new TimeSpan(0);
#pragma warning restore 618
            this.GridTerminalSystem = new TestGridTerminalSystem(FutureOwner);

            this.Runtime = new TestGridProgramRuntimeInfo();
            this.Me = FutureOwner;

           // this.Storage = System.IO.File.ReadAllText(@"D:\Storage.txt");
        }


        private void EchoImpl(string str)
        {
            ConsoleColor fg = Console2.ForegroundColor;
            Console2.ForegroundColor = ConsoleColor.White;
            Console2.WriteLine("Echo: " + str);
            Console2.ForegroundColor = fg;
        }

        public abstract void RunMain(string argument);

    }

    class TestTerminalProperty<T> : ITerminalProperty<T>
    {
        private string id;
        private T value;

        public TestTerminalProperty(string id, T value) : base()
        {
            this.id = id;
            this.value = value;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public string TypeName
        {
            get
            {
                return "System.Single"; // todo
            }
        }

        public T GetDefaultValue(IMyCubeBlock block)
        {
            throw new NotImplementedException();
        }

        public T GetMaximum(IMyCubeBlock block)
        {
            throw new NotImplementedException();
        }

        public T GetMinimum(IMyCubeBlock block)
        {
            throw new NotImplementedException();
        }

        public T GetMininum(IMyCubeBlock block)
        {
            throw new NotImplementedException();
        }

        public T GetValue(IMyCubeBlock block)
        {
            return value;
        }

        public void SetValue(IMyCubeBlock block, T value)
        {
            this.value = value;
        }
    }


    class TestTerminalAction : ITerminalAction
    {
        public TestTerminalAction(string name)
        {

        }

        public string Icon
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public StringBuilder Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Apply(IMyCubeBlock block)
        {

        }

        public void Apply(IMyCubeBlock block, ListReader<TerminalActionParameter> terminalActionParameters)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(IMyCubeBlock block)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(IMyCubeBlock block, StringBuilder appendTo)
        {
            throw new NotImplementedException();
        }
    }

}