using NamelessQuizness.Workers;

namespace NamelessQuizness.Renderers
{
    public class ConsoleQuizRenderer : IQuizRenderer
    {
        public void DrawInput(string preInputMessage, Action<string?> callback)
        {
            Console.Write($"{preInputMessage}: ");
            var result = Console.ReadLine();
            callback(result);
        }

        public void DrawMessage(string message, MessageFlags messageFlags)
        {
            DrawMessage(message, messageFlags, null);
        }

        public void DrawMessage(string message, MessageFlags messageFlags, params object?[]? args)
        {
            if (!messageFlags.HasFlag(MessageFlags.NoClear))
                Console.Clear();

            Console.WriteLine(message, args);

            if(!messageFlags.HasFlag(MessageFlags.NoWait)) 
                Console.ReadKey();
        }

        public void DrawQuestionsCounter(int questionIndex, int totalQuestionsCount)
        {
            Console.WriteLine($"{(questionIndex + 1)}/{totalQuestionsCount}");
        }

        public void DrawQuestion(int questionIndex, IQuestionWorker questionWorker)
        {
            Console.Clear();
            Console.Title = $"№{questionIndex + 1}";

            var description = questionWorker.GetDescription();
            Console.WriteLine(description);

            var answers = questionWorker.GetAnswersFormatted();
            if (answers != null)
            {
                Console.WriteLine();
                foreach (var answer in answers)
                {
                    Console.WriteLine(answer);
                }
            }

            Console.WriteLine();
        }
    }
}
