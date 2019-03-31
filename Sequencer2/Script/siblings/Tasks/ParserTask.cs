using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace Script
{

    #region ingame script start


    class ParserTask : InterruptibleTask<Tuple<List<SqProgram>, string>>
    {
        Parser parser;

		public ParserTask(string src) : base("ParserTask")
        {
            priority = 2;
            parser = new Parser(src);
        }

        public override bool DoWork()
        {
            bool done = parser.Parse(Timeout);
            if (done)
            {
                if (parser.Finalize())
                {
                    var validator = new SqValidator();
                    validator.Validate(parser.Programs, SqRequirements.Timer);
                }

                result = new Tuple<List<SqProgram>, string>(parser.Programs, parser.ErrorMessage);
            }

            return done;
        }
    }

    #endregion // ingame script end
}
