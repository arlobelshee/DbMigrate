using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Args;
using Args.Help;

namespace DbMigrate.UI
{
    public class TerminateAndShowHelp : TerminateProgramWithMessageException
    {
        public TerminateAndShowHelp(IModelBindingDefinition<MigrationParameters> helpMessage)
            : base(FormatHelp(helpMessage), 0)
        {
        }

        protected TerminateAndShowHelp(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private static string FormatHelp(IModelBindingDefinition<MigrationParameters> commandLineParser)
        {
            var helpData = new HelpProvider().GenerateModelHelp(commandLineParser);
            var message = new StringBuilder();
            message.AppendLine();
            message.AppendLine(helpData.HelpText);
            message.AppendLine();
            foreach (var member in helpData.Members)
            {
                message.AppendFormat("{0} (--{1})\r\n", member.Name,
                    member.Switches.Aggregate((lhs, rhs) => String.Join(", --", lhs, rhs)).
                        ToLowerInvariant());
                message.AppendFormat("  {0}\r\n\r\n", member.HelpText);
            }
            return message.ToString();
        }
    }
}