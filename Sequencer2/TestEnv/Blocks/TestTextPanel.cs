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
using console;

namespace SETestEnv
{

    class TestTextPanel : TestBlock, IMyTextPanel
    {
        public string CurrentlyShownImage { get; set; }

        public ShowTextOnScreenFlag ShowOnScreen { get; set; }

        public bool ShowText { get; set; }

        public bool Enabled { get; set; }

        public TestTextPanel() : base() { }

        public bool WritePublicText(string value, bool append = false)
        {
            float fontSize = (this.GetProperty("FontSize") as ITerminalProperty<float>).GetValue(this);
            int visibleCount = (int)((33 * 0.8 / fontSize) + 0.0001); // rounding magic

            ConsoleColor fg = Console2.ForegroundColor;
            ConsoleColor bg = Console2.BackgroundColor;

            Console2.ForegroundColor = ConsoleColor.Gray;
            Console2.WriteLine("LCD:");


            string[] lines = value.Split('\n').Select(x => x.Trim('\r')).ToArray();
            foreach (string line in lines)
            {
                string visiblePart = line.Substring(0, Math.Min(line.Length, visibleCount));
                string hiddenPart = line.Substring(Math.Min(line.Length, visibleCount));

                Console2.ForegroundColor = ConsoleColor.Cyan;
                Console2.BackgroundColor = ConsoleColor.Black;
                Console2.Write(visiblePart);

                //Console2.ForegroundColor = ConsoleColor.DarkBlue;
                //Console2.BackgroundColor = ConsoleColor.DarkGray;

                if (hiddenPart.Length > 0)
                {
                    Console2.Write(hiddenPart);
                }
                else
                {
                    Console2.CursorLeft = Console2.CursorLeft + visibleCount - visiblePart.Length;
                    Console2.Write(" ");
                }
                Console2.BackgroundColor = ConsoleColor.Black;
                Console2.WriteLine();
            }

            Console2.ForegroundColor = fg;
            Console2.BackgroundColor = bg;

            return true;
            //throw new NotImplementedException();
        }

        public string GetPublicText()
        {
            throw new NotImplementedException();
        }

        public bool WritePublicTitle(string value, bool append = false)
        {
            throw new NotImplementedException();
        }

        public string GetPublicTitle()
        {
            throw new NotImplementedException();
        }

        public bool WritePrivateText(string value, bool append = false)
        {
            throw new NotImplementedException();
        }

        public string GetPrivateText()
        {
            throw new NotImplementedException();
        }

        public bool WritePrivateTitle(string value, bool append = false)
        {
            throw new NotImplementedException();
        }

        public string PrivateTitle { get; set; }

        public float FontSize
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

        public Color FontColor
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

        public Color BackgroundColor
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

        public float ChangeInterval
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

        public string Font
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

        public string GetPrivateTitle()
        {
            return PrivateTitle;
        }

        public void AddImageToSelection(string id, bool checkExistence = false)
        {
            throw new NotImplementedException();
        }

        public void AddImagesToSelection(List<string> ids, bool checkExistence = false)
        {
            throw new NotImplementedException();
        }

        public void RemoveImageFromSelection(string id, bool removeDuplicates = false)
        {
            throw new NotImplementedException();
        }

        public void RemoveImagesFromSelection(List<string> ids, bool removeDuplicates = false)
        {
            throw new NotImplementedException();
        }

        public void ClearImagesFromSelection()
        {
            throw new NotImplementedException();
        }

        public void GetSelectedImages(List<string> output)
        {
            throw new NotImplementedException();
        }

        public void ShowPublicTextOnScreen() { }

        public void ShowPrivateTextOnScreen()
        {
            throw new NotImplementedException();
        }

        public void ShowTextureOnScreen() { }

        public void SetShowOnScreen(ShowTextOnScreenFlag set)
        {
            throw new NotImplementedException();
        }

        public void RequestEnable(bool enable)
        {
            throw new NotImplementedException();
        }

        public bool WritePublicText(StringBuilder value, bool append = false)
        {
            throw new NotImplementedException();
        }

        public void ReadPublicText(StringBuilder buffer, bool append = false)
        {
            throw new NotImplementedException();
        }

        public void GetFonts(List<string> fonts)
        {
            throw new NotImplementedException();
        }
    }

}
