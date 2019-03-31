using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start

    class Parser
    {
        public const string LOG_CAT = "prs";
        static HashSet<char> hexdigits = new HashSet<char>("1234567890ABCDEFabcdef");

        enum ParserState
        {
            AwaitingEntity,
            InFuncName,
            InCommand,
            InArg,
            InTextArg,
            InTextAwaitingEscaped,
            InTextEscapedUnicode,
            InTailArg,
            InComment,
            InVarDef,
            InVar,

            SyntaxError = -1
        }

        enum EntityType
        {
            Command,
            Var,
            Nothing
        }

        public List<SqProgram> Programs
        {
            get
            {
                if (LastError == null)
                {
                    return programs;
                }
                else
                {
                    return null;
                }
            }
        }

        public string ErrorMessage { get { return LastError; } }

           List<SqProgram> programs = new List<SqProgram>();
        List<SqCommand> commands = new List<SqCommand>();

        Dictionary<string, List<string>> vars = new Dictionary<string, List<string>>();
        List<string> argList = new List<string>();
        StringBuilder funcName = new StringBuilder();
        StringBuilder entityName = new StringBuilder();
        StringBuilder entityValue = new StringBuilder();
        StringBuilder escapedValue = new StringBuilder();

        int line = 0;
        int column = 0;
        string LastError = null;

        EntityType entityType = EntityType.Nothing;
        ParserState state;


        char[] chars;

        int nextPos;
        char ch;

        Dictionary<ParserState, Func<char, ParserState>> transitions;

        public Parser(string src)
        {
            chars = src.ToCharArray();
            nextPos = 0;
            ch = '\0';

            state = ParserState.AwaitingEntity;
            transitions = new Dictionary<ParserState, Func<char, ParserState>>{
                { ParserState.AwaitingEntity, ProcessAwaitingEntity },
                { ParserState.InFuncName, ProcessInFuncName },
                { ParserState.InCommand, ProcessInCommand },
                { ParserState.InArg, ProcessInArg },
                { ParserState.InTextArg, ProcessInTextArg },
                { ParserState.InTextAwaitingEscaped, ProcessInTextAwaitingEscaped },
                { ParserState.InTextEscapedUnicode, ProcessInTextEscapedUnicode },
                { ParserState.InTailArg, ProcessInTailArg },
                { ParserState.InComment, ProcessInComment },
                { ParserState.InVarDef, ProcessInVarDef },
                { ParserState.InVar, ProcessInVar },
            };
        }

        public bool Parse(Func<bool> timeout)
        {
            Log.Write(LOG_CAT, LogLevel.Verbose, "Parsing started");
            while ((state != ParserState.SyntaxError) && (nextPos < chars.Length))
            {
                ch = chars[nextPos];
                nextPos++;

                if (ch == '\n')
                {
                    column = 0;
                    line++;
                }
                else
                {
                    column++;
                }

                state = transitions[state](ch);

                if (timeout())
                {
                    return false;
                }                        
            }
            Log.Write(LOG_CAT, LogLevel.Verbose, "Parsing finished");
            return true;
        }


        public bool Finalize()
        { 
            state = FinalizeParsing(state);
            FinalizeLastFunction();

            if (state == ParserState.SyntaxError)
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Error, "Syntax Error: {0} at line {1} column {2}", LastError, line + 1, column);
            }
            return state != ParserState.SyntaxError;
        }


        private ParserState FinalizeParsing(ParserState state)
        {
            switch (state)
            {
                case ParserState.AwaitingEntity:
                    return FinalizeLastEntity() ? state : ParserState.SyntaxError;

                case ParserState.InArg:
                case ParserState.InTailArg:
                    argList.Add(entityValue.ToString());
                    return FinalizeLastEntity() ? state : ParserState.SyntaxError;

                case ParserState.InVar:
                    if (TryAppendArgs())
                    {
                        return FinalizeLastEntity() ? state : ParserState.SyntaxError;
                    }
                    else
                    {
                        LastError = ErrorMessages.UnknownNamedParameter;
                        return ParserState.SyntaxError;
                    }
                case ParserState.InVarDef:
                case ParserState.InCommand:
                    return FinalizeLastEntity() ? state : ParserState.SyntaxError;
                case ParserState.InComment:
                    return state;

                case ParserState.InTextArg:
                    LastError = ErrorMessages.UnexpectedEndOfScript;
                    return ParserState.SyntaxError;
                case ParserState.InFuncName:
                    return state;

                default:
                    return state;
            }

        }

        private ParserState ProcessInFuncName(char ch)
        {
            return TestNameOrWhitespace(ch, () => {
                funcName.Append(ch);
                return ParserState.InFuncName;
            }, () => {
                return ParserState.AwaitingEntity;
            }, () => {
                LastError = ErrorMessages.InvalidCharacterFunc;
                return ParserState.SyntaxError;
            });
        }

        private ParserState ProcessInCommand(char ch)
        {
            return TestNameOrWhitespace(ch, () => {
                entityName.Append(ch);
                return ParserState.InCommand;
            }, () => {
                return ParserState.AwaitingEntity;
            }, () => {
                LastError = ErrorMessages.InvalidCharacterCommand;
                return ParserState.SyntaxError;
            });
        }

        private ParserState ProcessInVarDef(char ch)
        {
            return TestNameOrWhitespace(ch, () => {
                entityName.Append(ch);
                return ParserState.InVarDef;
            }, () => {
                return ParserState.AwaitingEntity;
            }, () => {
                LastError = ErrorMessages.InvalidCharacterVarDef;
                return ParserState.SyntaxError;
            });
        }


        private ParserState ProcessInTailArg(char ch)
        {
            switch (ch)
            {
                case '\r':
                    return ParserState.InTailArg;
                case '\n':
                    argList.Add(entityValue.ToString());
                    return ParserState.AwaitingEntity;
                default:
                    entityValue.Append(ch);
                    return ParserState.InTailArg;
            }
        }

        private ParserState ProcessInTextArg(char ch)
        {
            switch (ch)
            {
                case '\"':
                    argList.Add(entityValue.ToString());
                    return ParserState.AwaitingEntity;
                case '\\':
                    return ParserState.InTextAwaitingEscaped;
                default:
                    entityValue.Append(ch);
                    return ParserState.InTextArg;
            }
        }

        private ParserState ProcessInTextAwaitingEscaped(char ch)
        {
            // http://en.cppreference.com/w/cpp/language/escape
            switch (ch)
            {
                case '\'':
                case '\"':
                case '?':
                case '\\':
                    entityValue.Append(ch);
                    return ParserState.InTextArg;
                case 'n':
                    entityValue.Append('\n');
                    return ParserState.InTextArg;
                case 'r':
                    entityValue.Append('\r');
                    return ParserState.InTextArg;
                case 't': // I have no idea what the hell Keen's text engine will do with tabulation char :D
                    entityValue.Append('\t');
                    return ParserState.InTextArg;
                case 'a': 
                    entityValue.Append('\a');
                    return ParserState.InTextArg;
                case 'b':
                    entityValue.Append('\b');
                    return ParserState.InTextArg;
                case 'f':
                    entityValue.Append('\f');
                    return ParserState.InTextArg;
                case 'v':
                    entityValue.Append('\v');
                    return ParserState.InTextArg;
                // case 'x': Too compex to implement, too unobvious for users. We have \u instead.
                case 'u':
                    escapedValue.Clear();
                    expectedEscapedSequenceLength = 4;
                    return ParserState.InTextEscapedUnicode;
                case 'U': // probably does not work because SpaceEngineers uses 2 bytes long chars.
                    escapedValue.Clear();
                    expectedEscapedSequenceLength = 8;
                    return ParserState.InTextEscapedUnicode;
                default:
                    LastError = string.Format(ErrorMessages.InvalidEscapeSequence, ch);
                    return ParserState.SyntaxError;
            }
        }

        int expectedEscapedSequenceLength = 0;

        private ParserState ProcessInTextEscapedUnicode(char ch)
        {
            if (hexdigits.Contains(ch))
            {
                escapedValue.Append(ch);
                if (escapedValue.Length >= expectedEscapedSequenceLength)
                {
                    uint ucharcode;
                    uint.TryParse(escapedValue.ToString(), System.Globalization.NumberStyles.HexNumber, C.I, out ucharcode);
                    entityValue.Append(ucharcode);
                    return ParserState.InTextArg;
                }
                else
                {
                    return ParserState.InTextEscapedUnicode;
                }
            }
            else
            {
                escapedValue.Append(ch);
                LastError = string.Format(ErrorMessages.InvalidEscapeSequence, escapedValue.ToString());
                return ParserState.SyntaxError;
            }
        }

        private ParserState ProcessInArg(char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                argList.Add(entityValue.ToString());
                return ParserState.AwaitingEntity;
            }
            else
            {
                entityValue.Append(ch);
                return ParserState.InArg;
            }
        }


        private ParserState ProcessInVar(char ch)
        {
            return TestNameOrWhitespace(ch, () => {
                entityValue.Append(ch);
                return ParserState.InVar;
            }, () =>
            {
                if (TryAppendArgs())
                {
                    return ParserState.AwaitingEntity;
                }
                else
                {
                    LastError = ErrorMessages.UnknownNamedParameter;
                    return ParserState.SyntaxError;
                }
            }, () => {
                LastError = ErrorMessages.InvalidCharacterVar;
                return ParserState.SyntaxError;
            });
        }

        private bool TryAppendArgs()
        {
            if (vars.ContainsKey(entityValue.ToString()))
            {
                argList.AddList(vars[entityValue.ToString()]);
                return true;
            }
            else
            {
                return false;
            }
        }

        private ParserState ProcessInComment(char ch)
        {
            if (ch == '\n')
            {
                return ParserState.AwaitingEntity;
            }
            else
            {
                return ParserState.InComment;
            }
        }

        ParserState ProcessAwaitingEntity(char ch)
        {
            switch (ch)
            {
                case '@':
                    if (!FinalizeLastEntity())
                    {
                        // todo: fix error location
                        return ParserState.SyntaxError;
                    }
                    FinalizeLastFunction();
                    entityName.Clear();
                    funcName.Clear();
                    return ParserState.InFuncName;
                case '#':
                    return ParserState.InComment;
                case '%':
                    if (!FinalizeLastEntity())
                    {
                        // todo: fix error location
                        return ParserState.SyntaxError;
                    }
                    entityType = EntityType.Var;
                    entityName.Clear();
                    argList.Clear();
                    return ParserState.InVarDef;
                case '/':
                    if (!FinalizeLastEntity())
                    {
                        // todo: fix error location
                        return ParserState.SyntaxError;
                    }
                    entityType = EntityType.Command;
                    entityName.Clear();
                    argList.Clear();
                    return ParserState.InCommand;
                case '$':
                    entityValue.Clear();
                    return ParserState.InVar;
                case ':':
                    if (TestHasEntity())
                    {
                        entityValue.Clear();
                        return ParserState.InTailArg;
                    }
                    else
                    {
                        LastError = ErrorMessages.OrphanParameter;
                        return ParserState.SyntaxError;
                    }
                case '\"':
                    if (TestHasEntity())
                    {
                        entityValue.Clear();
                        return ParserState.InTextArg;
                    }
                    else
                    {
                        LastError = ErrorMessages.OrphanParameter;
                        return ParserState.SyntaxError;
                    }
                default:
                    if (char.IsWhiteSpace(ch))
                    {
                        return ParserState.AwaitingEntity;
                    }
                    else
                    {
                        if (TestHasEntity())
                        {
                            entityValue.Clear();
                            entityValue.Append(ch);
                            return ParserState.InArg;
                        }
                        else
                        {
                            LastError = ErrorMessages.OrphanParameter;
                            return ParserState.SyntaxError;
                        }
                    }
            }
        }

        private void FinalizeLastFunction()
        {
            programs.Add(new SqProgram(funcName.ToString(), commands));
            commands = new List<SqCommand>();
        }

        private bool FinalizeLastEntity()
        {
            switch (entityType)
            {
                case EntityType.Command:
                    return FinalizeCommand();
                case EntityType.Var:
                    return FinalizeVarDef();
                default:
                    return true;
            }
        }


        private bool FinalizeCommand()
        {
            if (entityName.Length != 0)
            {
                SqCommand cmd;
                string errmsg;
                if (SqCommandBuilder.TryCreateCommand(entityName.ToString(), argList, out cmd, out errmsg))
                {
                    commands.Add(cmd);
                }
                else
                {
                    LastError = errmsg;
                    return false;
                }
            }
            return true;
        }

        private bool FinalizeVarDef()
        {
            if (entityName.Length != 0)
            {
                vars[entityName.ToString()] = new List<string>(argList);
            }
            return true;
        }

        bool TestHasEntity()
        {
            if (entityName.Length == 0)
            {
                //ReportException("Command expected");
                return false;
            }
            //currentArg.Clear();
            return true;
        }



        private ParserState TestNameOrWhitespace(char ch, Func<ParserState> alphanum, Func<ParserState> whitespace, Func<ParserState> noneof)
        {
            if (IsValidNameChar(ch))
            {
                return alphanum();
            }
            else if (char.IsWhiteSpace(ch))
            {
                return whitespace();
            }
            else
            {
                return noneof();
            }
        }

        bool IsValidNameChar(char ch)
        {
            return char.IsLetterOrDigit(ch) || char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.ConnectorPunctuation; // underline and some another stuff
        }
    }

    static class ErrorMessages
    {
        public const string OrphanParameter = "Abandoned parameter found";

        public const string UnexpectedEndOfScript = "Unexpected end of script";

        public const string InvalidCharacterCommand = "Invalid character in command name";
        public const string InvalidCharacterFunc = "Invalid character in function name";
        public const string InvalidCharacterVarDef = "Invalid Character in named parameter definition";
        public const string InvalidCharacterVar = "Invalid Character in parameter name";

        public const string UnknownNamedParameter = "Unknown named parameter";
        public const string UnknownCommand = "Unknown command \"{0}\"";

        public const string TooManyArgumentsIn = "Command \"{0}\" has too many arguments: expected {1}, given {2}";
        public const string TooFewArgumentsIn = "Command \"{0}\" has too few arguments: expected {1}, given {2}";
        public const string InvalidTypeValue = "\"{0}\" is invalid value of type {1}";

        public const string InvalidEscapeSequence = "\"{0} is invalid escape sequence";

        public const string ExtraCharacterAfterEnd = "extra symbol after command: \"{0}\"";
    }

    #endregion // ingame script end
}
