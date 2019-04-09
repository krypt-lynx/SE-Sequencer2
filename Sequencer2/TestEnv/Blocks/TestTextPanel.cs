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

        ShowTextOnScreenFlag IMyTextPanel.ShowOnScreen
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte BackgroundAlpha
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

        public TextAlignment Alignment
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

        public string Script
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

        public ContentType ContentType
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

        public Vector2 SurfaceSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vector2 TextureSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool PreserveAspectRatio
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

        public float TextPadding
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

        public Color ScriptBackgroundColor
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

        Color IMyTextSurface.ScriptForegroundColor
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

        string IMyTextSurface.Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IMyTextSurface.DisplayName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IMyFunctionalBlock.Enabled
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

        bool IMyTextPanel.WritePublicTitle(string value, bool append)
        {
            throw new NotImplementedException();
        }

        string IMyTextPanel.GetPublicTitle()
        {
            throw new NotImplementedException();
        }

        bool IMyTextPanel.WritePrivateText(string value, bool append)
        {
            throw new NotImplementedException();
        }

        string IMyTextPanel.GetPrivateText()
        {
            throw new NotImplementedException();
        }

        bool IMyTextPanel.WritePrivateTitle(string value, bool append)
        {
            throw new NotImplementedException();
        }

        string IMyTextPanel.GetPrivateTitle()
        {
            throw new NotImplementedException();
        }

        void IMyTextPanel.ShowPrivateTextOnScreen()
        {
            throw new NotImplementedException();
        }

        bool IMyTextPanel.WritePublicText(string value, bool append)
        {
            throw new NotImplementedException();
        }

        string IMyTextPanel.GetPublicText()
        {
            throw new NotImplementedException();
        }

        bool IMyTextPanel.WritePublicText(StringBuilder value, bool append)
        {
            throw new NotImplementedException();
        }

        void IMyTextPanel.ReadPublicText(StringBuilder buffer, bool append)
        {
            throw new NotImplementedException();
        }

        void IMyTextPanel.ShowPublicTextOnScreen()
        {
            throw new NotImplementedException();
        }

        void IMyTextPanel.ShowTextureOnScreen()
        {
            throw new NotImplementedException();
        }

        void IMyTextPanel.SetShowOnScreen(ShowTextOnScreenFlag set)
        {
            throw new NotImplementedException();
        }

        bool IMyTextSurface.WriteText(string value, bool append)
        {
            throw new NotImplementedException();
        }

        string IMyTextSurface.GetText()
        {
            throw new NotImplementedException();
        }

        bool IMyTextSurface.WriteText(StringBuilder value, bool append)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.ReadText(StringBuilder buffer, bool append)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.AddImageToSelection(string id, bool checkExistence)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.AddImagesToSelection(List<string> ids, bool checkExistence)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.RemoveImageFromSelection(string id, bool removeDuplicates)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.RemoveImagesFromSelection(List<string> ids, bool removeDuplicates)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.ClearImagesFromSelection()
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.GetSelectedImages(List<string> output)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.GetFonts(List<string> fonts)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.GetSprites(List<string> sprites)
        {
            throw new NotImplementedException();
        }

        void IMyTextSurface.GetScripts(List<string> scripts)
        {
            throw new NotImplementedException();
        }

        MySpriteDrawFrame IMyTextSurface.DrawFrame()
        {
            throw new NotImplementedException();
        }

        Vector2 IMyTextSurface.MeasureStringInPixels(StringBuilder text, string font, float scale)
        {
            throw new NotImplementedException();
        }

        void IMyFunctionalBlock.RequestEnable(bool enable)
        {
            throw new NotImplementedException();
        }
    }

}
