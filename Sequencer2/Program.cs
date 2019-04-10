using SETestEnv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using ConsoleClassLibrary;
using VRage.Game.GUI.TextPanel;

namespace console
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");

            TestProgrammableBlock owner = new TestProgrammableBlock();
            owner.CustomName = "Sequencer";
            owner.SetProperty(new TestProp<Int64>("Content", (Int64)ContentType.NONE));

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
            /*  owner.CustomData = @"
          @switch_array 
          /switch array_action open_array close_array 

          @test2
          /wait 5
          /echo ""Done!""

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


          ";*/
            /*
                        owner.CustomData = @"
            @_load

            @all
            /start light1
            /wait 1
            /start light2 
            /wait 1 
            /start light3

            @light1
            /action match ""light 1"" OnOff
            /echo light1 
            /wait 15
            /repeat

            @light2
            /action match ""light 2"" OnOff 
            /echo light2
            /wait 15
            /repeat

            @light3 
            /action match ""light 3"" OnOff 
            /echo light3
            /wait 15
            /repeat

            ";*/

            owner.CustomData = @"
#this program will called after ""cold"" start 
@_load 
/start enable_broadcast 


@lcd_inventory
/data ""Text panel[LCD]"" :Mass * 100 M;Cargo;InvListX;echo;echo;echo;echo;echo;echo
@lcd_power
/data ""Text panel[LCD]"" :Power

#vtol
   
@switch_vtol       
      
%vtol_thrust contains ""(vtol thrust)""      
%vtol_rotors group ""vtol rotors""       
%vtol_merges match ""Merge Block (vtol base)""     
%vtol_pistons head ""Piston (vtol""     
%vtol_lights contains ""(merge light)""   
   
/set $vtol_lights Color Red   
/set $vtol_lights ""Blink Interval"" 1   
/set $vtol_lights ""Blink Lenght"" 3   
   
/set $vtol_pistons ""Force weld"" False    
/action $vtol_rotors Attach        
/wait 0.3    
/action $vtol_thrust OnOff_Off       
/action $vtol_merges OnOff_Off       
/action $vtol_pistons Extend       
/wait 1       
/action $vtol_rotors Reverse       
/wait 4       
/action $vtol_merges OnOff_On       
/action $vtol_pistons Retract       
/wait 4       
/action $vtol_rotors Detach       
/set $vtol_pistons ""Force weld"" True     
/action $vtol_thrust OnOff_On         
/set $vtol_lights Color White    
/set $vtol_lights ""Blink Interval"" 0   
   
   
#workarounds for keen's bugs
   
@switch_hthrust    
/action contains ""Hydrogen Thrusters"" OnOff    
  
@enable_broadcast  
/set Antenna EnableBroadCast true  
/wait 30  
/repeat  
  

#airlock
   
@door_bridge   
/switch pressurised dbridge_slow dbridge_fast   
   
@dbridge_fast   
/action contains ""(airlock left)"" Open_Off   
/action contains ""(airlock right)"" Open_Off   
/action contains ""(medbay)"" Open_Off   
/action contains ""(bridge)"" Open   
   
@dbridge_slow   
/action contains ""(airlock left)"" Open_Off   
/action contains ""(airlock right)"" Open_Off   
/action contains ""(airlock vent)"" Depressurize_Off   
/wait 2   
/setvar pressurised 1   
/start update_airlock_lights  
/action contains ""(airlock left)"" OnOff_Off   
/action contains ""(airlock right)"" OnOff_Off   
/wait 3   
/action contains ""(medbay)"" OnOff_On   
/action contains ""(bridge)"" OnOff_On   
/action contains ""(bridge)"" Open_On   
   
@door_medbay   
/setvar medbay_from 0  
/switch pressurised dmedbay_slow dmedbay_fast   
   
@door_medbay_sensor  
/setvar medbay_from 1   
/switch pressurised dmedbay_slow dmedbay_fast    
   
@dmedbay_fast   
/switch medbay_from dmedbay_fast_door dmedbay_fast_sensor  
  
@dmedbay_fast_door  
/action contains ""(airlock left)"" Open_Off   
/action contains ""(airlock right)"" Open_Off   
/action contains ""(bridge)"" Open_Off   
/action contains ""(medbay)"" Open   
   
@dmedbay_fast_sensor  
/action contains ""(airlock left)"" Open_Off    
/action contains ""(airlock right)"" Open_Off    
/action contains ""(bridge)"" Open_Off    
  
@dmedbay_slow   
/action contains ""(airlock left)"" Open_Off   
/action contains ""(airlock right)"" Open_Off   
/action contains ""(airlock vent)"" Depressurize_Off   
/wait 2   
/setvar pressurised 1   
/start update_airlock_lights  
/action contains ""(airlock left)"" OnOff_Off   
/action contains ""(airlock right)"" OnOff_Off   
/wait 3   
/action contains ""(medbay)"" OnOff_On   
/action contains ""(bridge)"" OnOff_On   
/action contains ""(medbay)"" Open_On   
   
@door_airlock_left   
/switch pressurised dairlock_left_fast dairlock_left_slow   
   
@dairlock_left_fast   
/action contains ""(medbay)"" Open_Off   
/action contains ""(bridge)"" Open_Off   
/action contains ""(airlock right)"" Open_Off   
/action contains ""(airlock left)"" Open   
   
@dairlock_left_slow   
/action contains ""(medbay)"" Open_Off   
/action contains ""(bridge)"" Open_Off   
/action contains ""(airlock vent)"" Depressurize_On   
/wait 2   
/setvar pressurised 0   
/start update_airlock_lights  
/action contains ""(medbay)"" OnOff_Off   
/action contains ""(bridge)"" OnOff_Off   
/wait 3   
/action contains ""(airlock left)"" OnOff_On   
/action contains ""(airlock right)"" OnOff_On   
/action contains ""(airlock left)"" Open_On   
   
   
   
@door_airlock_right   
/switch pressurised dairlock_right_fast dairlock_right_slow   
   
@dairlock_right_fast   
/action contains ""(medbay)"" Open_Off   
/action contains ""(bridge)"" Open_Off   
/action contains ""(airlock left)"" Open_Off   
/action contains ""(airlock right)"" Open   
   
@dairlock_right_slow   
/action contains ""(medbay)"" Open_Off   
/action contains ""(bridge)"" Open_Off   
/action contains ""(airlock vent)"" Depressurize_On   
/wait 2   
/setvar pressurised 0   
/start update_airlock_lights  
/action contains ""(medbay)"" OnOff_Off   
/action contains ""(bridge)"" OnOff_Off   
/wait 3   
/action contains ""(airlock left)"" OnOff_On   
/action contains ""(airlock right)"" OnOff_On   
/action contains ""(airlock right)"" Open_On   
   
      
@update_airlock_lights   
/switch pressurised airlock_lights_off airlock_lights_on   
@airlock_lights_on   
/set contains ""(airlock light)"" Color AliceBlue #why Alice is blue? Oo 
@airlock_lights_off   
/set contains ""(airlock light)"" Color Orange   
  
   
#service access   
@service_door   
/switch service_door_opened open_service_door close_service_door    
   
@open_service_door   
/action ""Service Door"" OnOff_On   
/action ""Service Door"" Open_On   
/set ""Gravity Generator"" Width 26   
/set ""Gravity Generator"" Height 13.5   
/set ""Gravity Generator"" Depth 93   
/setvar service_door_opened 1   
/stop close_service_door   
   
@close_service_door   
/action ""Service Door"" Open_Off   
/set ""Gravity Generator"" Width 26   
/set ""Gravity Generator"" Height 13.5   
/set ""Gravity Generator"" Depth 48.5  
/setvar service_door_opened 0   
/wait 2   
/action ""Service Door"" OnOff_Off   
";

            /*
owner.CustomData = @"
@_load
/echo test
/start loop

@loop
/echo loop
/wait 5
/repeat

@test
/stop return
/text ""Test Result LCD"" ""Testing...""
/set Rotor Velocity 1
/wait 10
/set Rotor Velocity 0
/text ""Test Result LCD"" ""Done!\nCurrent: ""
#/test1 Rotor ""Test Result LCD""
/text match ""Test Result LCD"" true ""\nExpected: 1.085228""

@return
/stop test 
/text ""Test Result LCD"" """" 
/set Rotor Velocity -6 
/wait 5.1
/set group Rotor Velocity 0 
#/test1 Rotor ""Test Result LCD"" 

@info
/listprops ""Sequencer Timer""
/listactions ""Sequencer Timer""
";*/

            Script.Program.FutureOwner = owner;
  
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
            string arg = "";
            while (arg != "q")
            {
                if (test.Runtime.UpdateFrequency != Sandbox.ModAPI.Ingame.UpdateFrequency.None)
                {
                    Console2.WriteLine("auto");
                }
                else
                {
                    Console2.WriteLine("manual");
                }

                arg = Console2.ReadLine();
                test.RunMain(arg);
                Console2.ForegroundColor = ConsoleColor.Yellow;
                Console2.WriteLine("execution finished");

 
                // Console2.SwitchBuffer();
            }

            test.Save();
            Console2.ReadLine();
        }

    }
}
