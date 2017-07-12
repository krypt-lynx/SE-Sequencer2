﻿using SETestEnv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using ConsoleClassLibrary;

namespace console
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");

            TestProgrammableBlock owner = new TestProgrammableBlock();
            owner.CustomName = "Sequenser";
            /*    owner.CustomData = @"
    ##usetimer :timer 1

    %vtol_thrust contains :(vtol thrust)
    #%vtol_rotors head :Rotor (vtol
    %vtol_rotors head :Rotor (vtol

    #this is comment
    /action $vtol_thrust OnOff_Off
    /action $vtol_rotors Attach
    /action head ""Merge Block (vtol"" 
    #test
    OnOff_Off
    /action head ""Piston (vtol"" Extend
    /wait 1
    /action head ""Rotor (vtol"" Reverse
    /wait 4
    /action head ""Merge Block (vtol"" OnOff_On
    /action head ""Piston (vtol"" Retract
    /wait 4
    /action $vtol_rotors Detach
    /action $vtol_thrust OnOff_On  

    @prog2
    /wait 0.3
                    ";*/
        /*    owner.CustomData = @"
##usetimer :timer 1   
 
  
@switch_vtol   
  
%vtol_thrust contains ""(vtol thrust)""  
%vtol_rotors group ""vtol rotors""   
%vtol_merges match ""Merge Block (vtol base)"" 
%vtol_pistons head ""Piston (vtol"" 
 
/action $vtol_thrust OnOff_Off   
/action $vtol_rotors Attach   
/action $vtol_merges OnOff_Off   
/action $vtol_pistons Extend   
/wait 1   
/action $vtol_rotors Reverse   
/wait 4   
/action $vtol_merges OnOff_On   
/action $vtol_pistons Retract   
/wait 4   
/action $vtol_rotors Detach   
/action $vtol_thrust OnOff_On     

@test1
/action $vtol_rotors Reverse  

@test2 
/action $vtol_merges OnOff_On    
/action $vtol_pistons Retract   
 
@test3 
/action $vtol_rotors Detach    
/action $vtol_thrust OnOff_On      
  

 ";*/
    owner.CustomData = @"
@switch_array 
/switch array_action open_array close_array 
 
@open_array 
/action group ""Rotor SH"" Reverse 
/wait 17 
/action group ""Rotor ED"" Reverse 
/action group ""Rotor IN"" Reverse 
/wait 10 
/action group ""Rotor OT"" Reverse 
/action group ""Rotor SH"" Reverse	 
/action group ""Rotor SH"" OnOff_Off 
/wait 25 
/setvar array_action 1 
 
@close_array 
/action group ""Rotor IN"" Reverse 
/wait 8 
/action group ""Rotor ED"" Reverse 
/action group ""Rotor OT"" Reverse 
/wait 10 
/action group ""Rotor SH"" OnOff_On 
/wait 30 
/setvar array_action 0 


";

            Script.Program.FutureOwner = owner;
      /*      Script.Program.FutureStorage =
"i0;i2;d0;d0;i1;s10,open_array;i4;i0;s0,;bTrue;d0;i0;i0;s12,switch_array;bTrue;d0" +
";i1;s6,switch;i2;s12,array_action;i2;s10,open_array;s11,close_array;i0;s10,open_" +
"array;bTrue;d0;i10;s6,action;i3;i3;s8,Rotor SH;s7,Reverse;s4,wait;i1;d17;s6,acti" +
"on;i3;i3;s8,Rotor ED;s7,Reverse;s6,action;i3;i3;s8,Rotor IN;s7,Reverse;s4,wait;i" +
"1;d10;s6,action;i3;i3;s8,Rotor OT;s7,Reverse;s6,action;i3;i3;s8,Rotor SH;s7,Reve" +
"rse;s6,action;i3;i3;s8,Rotor SH;s9,OnOff_Off;s4,wait;i1;d25;s6,setvar;i2;s12,arr" +
"ay_action;d1;i0;s11,close_array;bTrue;d0;i8;s6,action;i3;i3;s8,Rotor IN;s7,Rever" +
"se;s4,wait;i1;d8;s6,action;i3;i3;s8,Rotor ED;s7,Reverse;s6,action;i3;i3;s8,Rotor" +
" OT;s7,Reverse;s4,wait;i1;d10;s6,action;i3;i3;s8,Rotor SH;s8,OnOff_On;s4,wait;i1" +
";d30;s6,setvar;i2;s12,array_action;d0;i0;";
*/
            var test = new Script.Program();

            TestTextPanel panel1 = new TestTextPanel
            {
                IsWorking = true,
                CustomName = "TestLCD",
                DetailedInfo = "Type: LCD Panel\nMax Required Input: 100 W\nCurrent Input: 100 W",

                PrivateTitle = "Working;Time Global Time: ;Cargo;Power;Inventory;Echo;Center << Damage >>;Damage;BlockCount",

                BuildIntegrity = 7200,
                MaxIntegrity = 7200
            };


            panel1.SetProperty(new TestProp<float>("FontSize", 0.8f));
            panel1.SetProperty(new TestProp<Color>("FontColor", new Color(255, 255, 255)));
            panel1.SetProperty(new TestProp<Color>("BackgroundColor", new Color(0, 0, 0)));

            test.TestGridTerminalSystem.CubeGrid.RegisterBlock(panel1);

            TestTimerBlock timer = new TestTimerBlock
            {
                CustomName = "Sequenser Timer"
            };

            test.TestGridTerminalSystem.CubeGrid.RegisterBlock(timer);


            TestBatteryBlock battery = new TestBatteryBlock
            {
                CustomName = "Battery",
                DetailedInfo =
                    "Type: Battery\n" +
                    "Max Output: 12.00 MW\n" +
                    "Max Required Input: 12.00 MW\n" +
                    "Max Stored Power: 3.00 MWh\n" +
                    "Current Input: 0 W\n" +
                    "Current Output: 1.43 MWh\n" +
                    "Stored power: 2.47 MWh\n" +
                    "Fully depleted in: 1 days",

                BuildIntegrity = 7200,
                MaxIntegrity = 7200,
                CurrentDamage = 200
            };
            test.TestGridTerminalSystem.CubeGrid.RegisterBlock(battery);


            //Console2.CreateBuffers();

            Console2.ForegroundColor = ConsoleColor.Yellow;
            string arg = Console2.ReadLine();
            while (arg != "q")
            {
                test.RunMain(arg);
                Console2.ForegroundColor = ConsoleColor.Yellow;
                Console2.WriteLine("execution finished");
                arg = Console2.ReadLine();
                // Console2.SwitchBuffer();
            }

            test.Save();
            Console2.ReadLine();
        }

    }
}
