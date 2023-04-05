using NamelessQuizness.Extensions;
using NamelessQuizness.Workers;
using System.Text;

namespace NamelessQuizness.Renderers
{
    public class FormattedConsoleQuizRenderer : IQuizRenderer
    {
        private const int ConsoleLineWidth = 120;
        private const int MessageLineWidth = ConsoleLineWidth - 4;
        private const char TitlePaddingChar = '-';

        public void DrawInput(string preInputMessage, Action<string?> callback)
        {
            Console.Write($"> {preInputMessage}: ");
            var result = Console.ReadLine();
            callback(result);
        }

        public void DrawMessage(string message, MessageFlags messageFlags)
        {
            DrawMessage(message, messageFlags, null);
        }

        public void DrawMessage(string message, MessageFlags messageFlags, params object?[]? args)
        {
            if (args is not null)
                message = string.Format(message, args);

            var fillerLine = string.Empty.PadRight(120, '-');
            List<string> messageStrings = SplitAndFormatMessage(message).ToList();

            if (!messageFlags.HasFlag(MessageFlags.NoClear))
                Console.Clear();

            Console.WriteLine(fillerLine);
            messageStrings.ForEach(s => Console.WriteLine(s));
            Console.WriteLine(fillerLine);

            if (!messageFlags.HasFlag(MessageFlags.NoWait))
                Console.ReadKey();
        }

        public void DrawQuestion(int questionIndex, IQuestionWorker questionWorker)
        {
            Console.Clear();

            var title = $"№{questionIndex + 1}";
            Console.Title = title;

            DrawTitle(title);

            var description = new StringBuilder(questionWorker.GetDescription());

            var answers = questionWorker.GetAnswersFormatted();
            if (answers != null)
            {
                description.Append('\n');
                foreach (var (index, answer) in answers.Select((x, i) => (i, x)))
                {
                    if (index > 0)
                        description.Append('\n');
                    description.Append(answer);
                }
                description.Append('\n');
            }

            List<string> messageStrings = SplitAndFormatMessage(description.ToString()).ToList();
            messageStrings.ForEach(s => Console.WriteLine(s));
        }

        public void DrawQuestionsCounter(int questionIndex, int totalQuestionsCount)
        {
            var barStep = (118.0 / totalQuestionsCount);

            var fillerLength = Convert.ToInt32((questionIndex) * barStep);

            var filler = string.Empty.PadRight(fillerLength, '#');

            if(questionIndex == totalQuestionsCount)
            {
                filler = filler.PadRight(118, '#');
            }
            else
            {
                filler = filler.PadRight(118, '.');
            }

            Console.WriteLine($"[{filler}]");
        }

        private static void DrawTitle(string title)
        {
            int titlePaddingLength = 120 - title.Length - 2;

            int leftPaddingLength = Floor(titlePaddingLength * 0.5f);
            int rightPaddingLength = Ceiling(titlePaddingLength * 0.5f);

            title = $"{TitlePaddingChar.Repeat(leftPaddingLength)} {title}";
            title = $"{title} {TitlePaddingChar.Repeat(rightPaddingLength)}";

            Console.WriteLine(title);

            static int Floor(double value) => Convert.ToInt32(Math.Floor(value));
            static int Ceiling(double value) => Convert.ToInt32(Math.Ceiling(value));
        }

        private static IEnumerable<string> SplitAndFormatMessage(string message)
        {
            string[] lines = message.Split('\n');
            var emptyLineFiller = $"| {string.Empty.PadRight(MessageLineWidth, ' ')} |";

            List<string> messageStrings = new();
            foreach (var line in lines)
            {
                int substrings = Convert.ToInt32(Math.Ceiling(line.Length / (double)MessageLineWidth));

                if (substrings == 0)
                {
                    messageStrings.Add(emptyLineFiller);
                }
                else
                {
                    for (int i = 0; i < substrings; i++)
                    {
                        var length = Math.Min(line.Length - i * MessageLineWidth, MessageLineWidth);
                        var substring = line.Substring(i * MessageLineWidth, length);
                        substring = $"| {substring.PadRight(MessageLineWidth, ' ')} |";
                        messageStrings.Add(substring);
                    }
                }
            }
            return messageStrings;
        }
    }
}
