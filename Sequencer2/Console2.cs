using ConsoleClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace console
{
    public static class Console2
    {
        static Console2()
        {

            defaultHandle = NativeConsole.GetStdHandle(NativeConsole.STD_OUTPUT_HANDLE);
            currentHandle = defaultHandle;


            NativeConsole.CONSOLE_SCREEN_BUFFER_INFO info;
            NativeConsole.GetConsoleScreenBufferInfo(currentHandle, out info);


            foregroundColor = (ConsoleColor)(info.wAttributes & 0x0F);
            backgroundColor = (ConsoleColor)((info.wAttributes & 0xF0) >> 4);

            bufferSize = new NativeConsole.COORD(80, 35);
        }

        static ConsoleColor backgroundColor;
        static ConsoleColor foregroundColor;
        static NativeConsole.COORD bufferSize;
        static IntPtr[] buffers;
        static int currentBuffer;
        static IntPtr currentHandle;
        static IntPtr defaultHandle;

        public static ConsoleColor BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                NativeConsole.SetConsoleTextAttribute(currentHandle, (ushort)((ushort)foregroundColor + ((ushort)backgroundColor << 4)));
            }
        }

        public static void CreateBuffers()
        {
            // NativeConsole.GetConsoleScreenBufferInfo(NativeConsole.GetStdHandle(NativeConsole.STD_OUTPUT_HANDLE))
            buffers = new IntPtr[] {
                CreateBuffer(),
                CreateBuffer()
            };

            currentBuffer = 0;

            currentHandle = buffers[currentBuffer];

            // NativeConsole.SetConsoleActiveScreenBuffer(screenBuffer);
        }

        private static IntPtr CreateBuffer()
        {
            var buffer = NativeConsole.CreateConsoleScreenBuffer(
                                NativeConsole.GENERIC_READ | NativeConsole.GENERIC_WRITE,
                                1, IntPtr.Zero, NativeConsole.CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero);
            NativeConsole.CONSOLE_SCREEN_BUFFER_INFO_EX info = NativeConsole.CONSOLE_SCREEN_BUFFER_INFO_EX.Create();
           // info.cbSize = sizeof(NativeConsole.CONSOLE_SCREEN_BUFFER_INFO_EX);
            NativeConsole.GetConsoleScreenBufferInfoEx(defaultHandle, ref info);
            info.dwSize = bufferSize;
            NativeConsole.SetConsoleScreenBufferInfoEx(buffer, info);
            //NativeConsole.SetConsoleScreenBufferSize(buffer, new NativeConsole.COORD(80, 40));

            return buffer;
        }

        public static void SwitchBuffer()
        {
            NativeConsole.SetConsoleActiveScreenBuffer(buffers[currentBuffer]);

            currentBuffer = (currentBuffer + 1) % 2;
            currentHandle = buffers[currentBuffer];

            NativeConsole.CHAR_INFO[] ci = new NativeConsole.CHAR_INFO[bufferSize.X * bufferSize.Y];
            NativeConsole.SMALL_RECT rect = new NativeConsole.SMALL_RECT(0, 0, (short)(bufferSize.X - 1), (short)(bufferSize.Y - 1));
            NativeConsole.WriteConsoleOutput(currentHandle, ci, bufferSize, new NativeConsole.COORD(), ref rect);

            NativeConsole.SetConsoleCursorPosition(currentHandle, new NativeConsole.COORD(0, 0));
        }

        public static int BufferHeight { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int BufferWidth { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static bool CapsLock { get { throw new NotImplementedException(); } }
        public static int CursorLeft { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int CursorSize { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int CursorTop { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static bool CursorVisible { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public static TextWriter Error { get { throw new NotImplementedException(); } }
        public static ConsoleColor ForegroundColor
        {
            get
            {
                return foregroundColor;
            }
            set
            {
                foregroundColor = value;
                NativeConsole.SetConsoleTextAttribute(currentHandle, (ushort)((ushort)foregroundColor + ((ushort)backgroundColor << 4)));
            }
        }

        public static TextReader In { get { throw new NotImplementedException(); } }
        public static Encoding InputEncoding { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static bool IsErrorRedirected { get { throw new NotImplementedException(); } }
        public static bool IsInputRedirected { get { throw new NotImplementedException(); } }
        public static bool IsOutputRedirected { get { throw new NotImplementedException(); } }
        public static bool KeyAvailable { get { throw new NotImplementedException(); } }
        public static int LargestWindowHeight { get { throw new NotImplementedException(); } }
        public static int LargestWindowWidth { get { throw new NotImplementedException(); } }
        public static bool NumberLock { get { throw new NotImplementedException(); } }
        public static TextWriter Out { get { throw new NotImplementedException(); } }
        public static Encoding OutputEncoding { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static string Title { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static bool TreatControlCAsInput { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int WindowHeight { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int WindowLeft { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int WindowTop { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static int WindowWidth { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public static event ConsoleCancelEventHandler CancelKeyPress;

        public static void Beep() { throw new NotImplementedException(); }
        public static void Beep(int frequency, int duration) { throw new NotImplementedException(); }
        public static void Clear() { throw new NotImplementedException(); }
        public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop) { throw new NotImplementedException(); }
        public static void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor) { throw new NotImplementedException(); }
        public static Stream OpenStandardError() { throw new NotImplementedException(); }
        public static Stream OpenStandardError(int bufferSize) { throw new NotImplementedException(); }
        public static Stream OpenStandardInput() { throw new NotImplementedException(); }
        public static Stream OpenStandardInput(int bufferSize) { throw new NotImplementedException(); }
        public static Stream OpenStandardOutput() { throw new NotImplementedException(); }
        public static Stream OpenStandardOutput(int bufferSize) { throw new NotImplementedException(); }
        public static int Read() { throw new NotImplementedException(); }
        public static ConsoleKeyInfo ReadKey()
        {
             NativeConsole.INPUT_RECORD[] keyInfo = new NativeConsole.INPUT_RECORD[1];
            //var test = Console.ReadKey();

            uint nRead;
            while (NativeConsole.ReadConsoleInput(NativeConsole.GetStdHandle(NativeConsole.STD_INPUT_HANDLE), keyInfo, 1, out nRead) && nRead != 0)
            {
                //Debug.WriteLine("{0} {1} {2}", keyInfo[0].EventType, keyInfo[0].KeyEvent.bKeyDown, (int)keyInfo[0].KeyEvent.UnicodeChar);

                if (keyInfo[0].EventType == NativeConsole.InputEventType.Key &&
                    keyInfo[0].KeyEvent.bKeyDown &&
                    keyInfo[0].KeyEvent.UnicodeChar != 0)
                {
                    var test1 = new ConsoleKeyInfo(keyInfo[0].KeyEvent.UnicodeChar,
                        (ConsoleKey)keyInfo[0].KeyEvent.wVirtualKeyCode,
                        (keyInfo[0].KeyEvent.dwControlKeyState & NativeConsole.ControlKeyStates.ShiftPressed) != 0,
                        (keyInfo[0].KeyEvent.dwControlKeyState & (NativeConsole.ControlKeyStates.LeftAltPressed | NativeConsole.ControlKeyStates.RightAltPressed)) != 0,
                        (keyInfo[0].KeyEvent.dwControlKeyState & (NativeConsole.ControlKeyStates.LeftCtrlPressed | NativeConsole.ControlKeyStates.RightAltPressed)) != 0
                        );
                    return test1;
                }
            }
            
            throw new NotImplementedException();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept) { throw new NotImplementedException(); }
        public static string ReadLine()
        {
            return Console.ReadLine();
        }
        public static void ResetColor() { throw new NotImplementedException(); }
        public static void SetBufferSize(int width, int height) { throw new NotImplementedException(); }
        public static void SetCursorPosition(int left, int top) { throw new NotImplementedException(); }
        public static void SetError(TextWriter newError) { throw new NotImplementedException(); }
        public static void SetIn(TextReader newIn) { throw new NotImplementedException(); }
        public static void SetOut(TextWriter newOut) { throw new NotImplementedException(); }
        public static void SetWindowPosition(int left, int top) { throw new NotImplementedException(); }
        public static void SetWindowSize(int width, int height) { throw new NotImplementedException(); }
        public static void Write(long value) { throw new NotImplementedException(); }

        public static void Write(string value)
        {
            uint written;
            NativeConsole.WriteConsoleW(currentHandle, value, value.Length, out written, IntPtr.Zero);
        }

        public static void Write(ulong value) { throw new NotImplementedException(); }
        public static void Write(uint value) { throw new NotImplementedException(); }
        public static void Write(object value)
        {
            Write(value.ToString());
        }
        public static void Write(float value) { throw new NotImplementedException(); }
        public static void Write(decimal value) { throw new NotImplementedException(); }
        public static void Write(double value) { throw new NotImplementedException(); }
        public static void Write(char[] buffer) { throw new NotImplementedException(); }
        public static void Write(char value)
        {
            Write(value.ToString());
        }
        public static void Write(bool value) { throw new NotImplementedException(); }
        public static void Write(int value) { throw new NotImplementedException(); }

        public static void Write(string format, object arg0)
        {
            Write(string.Format(format, arg0));
        }

        public static void Write(string format, params object[] arg) { throw new NotImplementedException(); }
        public static void Write(string format, object arg0, object arg1) { throw new NotImplementedException(); }
        public static void Write(char[] buffer, int index, int count) { throw new NotImplementedException(); }
        public static void Write(string format, object arg0, object arg1, object arg2) { throw new NotImplementedException(); }
        public static void Write(string format, object arg0, object arg1, object arg2, object arg3) { throw new NotImplementedException(); }
        public static void WriteLine()
        {
            Write("\n");
        }

        public static void WriteLine(bool value) { throw new NotImplementedException(); }
        public static void WriteLine(float value) { throw new NotImplementedException(); }
        public static void WriteLine(int value) { throw new NotImplementedException(); }
        public static void WriteLine(uint value) { throw new NotImplementedException(); }
        public static void WriteLine(long value) { throw new NotImplementedException(); }
        public static void WriteLine(ulong value) { throw new NotImplementedException(); }
        public static void WriteLine(object value)
        {
            Write(value);
            WriteLine();
        }
        public static void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }
        public static void WriteLine(double value) { throw new NotImplementedException(); }
        public static void WriteLine(decimal value) { throw new NotImplementedException(); }
        public static void WriteLine(char[] buffer) { throw new NotImplementedException(); }
        public static void WriteLine(char value)
        {
            Write(value);
            WriteLine();
        }
        public static void WriteLine(string format, params object[] arg) { throw new NotImplementedException(); }
        public static void WriteLine(string format, object arg0)
        {
            Write(format, arg0);
            WriteLine();
        }

        public static void WriteLine(string format, object arg0, object arg1) { throw new NotImplementedException(); }
        public static void WriteLine(char[] buffer, int index, int count) { throw new NotImplementedException(); }
        public static void WriteLine(string format, object arg0, object arg1, object arg2) { throw new NotImplementedException(); }
        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3) { throw new NotImplementedException(); }
    }
}
