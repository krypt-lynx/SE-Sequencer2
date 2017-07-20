using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start


    class ParserTask : FastTask<Tuple<List<SqProgram>, string>>
    {
        string src;
        Parser parser;

		public ParserTask(string src) : base("ParserTask")
        {
            this.src = src;
            parser = new Parser();
        }

        public override int InstructionsLimit()
        {
            return 15000;
        }

        public override bool DoWork()
        {
            if (parser.Parse(src))
            {
                var validator = new SqValidator();
                validator.Validate(parser.Programs, SqRequirements.Timer);
            }

            result = new Tuple<List<SqProgram>, string>(parser.Programs, parser.ErrorMessage);

            return true;
        }
    }

    #endregion // ingame script end
}
