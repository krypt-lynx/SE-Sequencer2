using System;

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
using VRage.Utils;

namespace SETestEnv
{
    abstract class TestProp
    {
        public string Id { get; set; }
        public abstract ITerminalProperty Prop();
    }

    class TestProp <T> : TestProp
    {
        public T Value { get; set; }

        public TestProp(string id, T value)
        {
            this.Id = id;
            this.Value = value;
        }

        public override ITerminalProperty Prop()
        {
            return new TestTerminalProperty<T>(Id, Value);
        }

        //TestTerminalProperty<float>(id, 0.6f);
    }

    abstract class TestBlock : IMyTerminalBlock, IMySlimBlock, IMyTextSurfaceProvider
    {
        public TestCubeGrid OwnerGrid { get; internal set; }
        private Dictionary<string, TestProp> properties = new Dictionary<string, TestProp>();
        public void SetProperty(TestProp prop)
        {
            properties[prop.Id] = prop;
        }

        #region IMyTerminalBlock
        public SerializableDefinitionId BlockDefinition
        {
            get
            {
                return new SerializableDefinitionId(new MyObjectBuilderType(this.GetType()), "LargeTextPanel"); // todo
            }
        }

        public bool CheckConnectionAllowed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public MyEntityComponentContainer Components
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IMyCubeGrid CubeGrid
        {
            get
            {
                return OwnerGrid;
            }
        }

        public string CustomInfo { get; set; }
        public string CustomName { get; set; }

        public string CustomNameWithFaction
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DefinitionDisplayNameText { get { return CustomName; } }
        public string DetailedInfo { get; set; }

        public float DisassembleRatio
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DisplayNameText { get { return this.CustomName; } }

        public long EntityId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsBeingHacked
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFunctional
        {
            get
            {
                return true;
            }
        }

        public bool IsWorking { get; set; }

        public float Mass
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

        public int NumberInGrid
        {
            get
            {
                return OwnerGrid.Blocks.IndexOf(this);
            }
        }

        public MyBlockOrientation Orientation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long OwnerId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vector3I Position
        {
            get
            {
                return new Vector3I(OwnerGrid.Blocks.IndexOf(this), 0, 0);
            }
        }

        public bool ShowOnHUD
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

        public void GetActions(List<ITerminalAction> resultList, Func<ITerminalAction, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public ITerminalAction GetActionWithName(string name)
        {
            return new TestTerminalAction(name);
        }

        public string GetOwnerFactionTag()
        {
            throw new NotImplementedException();
        }

        public MyRelationsBetweenPlayerAndBlock GetPlayerRelationToOwner()
        {
            throw new NotImplementedException();
        }

        public Vector3D GetPosition()
        {
            return new Vector3D(0);
        }

        public void GetProperties(List<ITerminalProperty> resultList, Func<ITerminalProperty, bool> collect = null)
        {
            resultList.Clear();            
        }

        public ITerminalProperty GetProperty(string id)
        {
            var prop = this.properties[id];
            return prop.Prop();
        }

        public MyRelationsBetweenPlayerAndBlock GetUserRelationToOwner(long playerId)
        {
            throw new NotImplementedException();
        }

        public bool HasLocalPlayerAccess()
        {
            throw new NotImplementedException();
        }

        public bool HasPlayerAccess(long playerId)
        {
            throw new NotImplementedException();
        }

        public void SearchActionsOfName(string name, List<ITerminalAction> resultList, Func<ITerminalAction, bool> collect = null)
        {
            throw new NotImplementedException();
        }

        public void SetCustomName(StringBuilder text)
        {
            throw new NotImplementedException();
        }

        public void SetCustomName(string text)
        {
            throw new NotImplementedException();
        }

        public void UpdateIsWorking()
        {
            throw new NotImplementedException();
        }

        public void UpdateVisual()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMySlimBlock
        public float AccumulatedDamage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float BuildIntegrity { get; set; }

        public float BuildLevelRatio
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float CurrentDamage { get; set; }

        public float DamageRatio
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IMyCubeBlock FatBlock
        {
            get
            {
                return this;
            }
        }

        public bool HasDeformation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsDestroyed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFullIntegrity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsFullyDismounted
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float MaxDeformation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float MaxIntegrity { get; set; }

        public bool ShowParts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool StockpileAllocated
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool StockpileEmpty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string CustomData { get; set; }

        bool IMyTerminalBlock.ShowOnHUD
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

        public bool ShowInTerminal
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

        public bool ShowInToolbarConfig
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

        public bool ShowInInventory
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

        public Vector3 ColorMaskHSV
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TestBlock()
        {

        }
        public TestBlock(string CustomName)
        {

        }

        public void GetMissingComponents(Dictionary<string, int> addToDictionary)
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

        public bool IsSameConstructAs(IMyTerminalBlock other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMyTextSurfaceProvider

        public int SurfaceCount { get { return 0; } }

        public MyStringHash SkinSubtypeId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IMyTextSurface GetSurface(int index) { return null; }

        #endregion
    }

}
