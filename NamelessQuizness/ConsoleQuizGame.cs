using NamelessQuizness.Serialization;
using NamelessQuizness.Processors;
using NamelessQuizness.Renderers;
using NamelessQuizness.Definitions;
using System.Runtime.InteropServices;
using System.Text;

namespace NamelessQuizness
{
    public class ConsoleQuizGame : IQuizGame
    {
        public IQuizGameDef QuizGameDef { get; private set; }
        public ConsoleQuizGameDef ConsoleQuizGameDef { get; private set; }

        public ConsoleQuizGame(IQuizGameDef quizGameDef)
        {
            QuizGameDef = quizGameDef;

            if(QuizGameDef is ConsoleQuizGameDef consoleQuizGameDef)
            {
                ConsoleQuizGameDef = consoleQuizGameDef;
            } 
            else
            {
                throw new ArgumentNullException(nameof(quizGameDef),
                    "QuizGameDef must have a type ConsoleQuizGameDef");
            }
        }

        public void Play()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(120, 30);
            }

            Console.Title = ConsoleQuizGameDef.Title;

            if (Activator.CreateInstance(ConsoleQuizGameDef.GameRenderer) is not IRenderer renderer)
            {
                if (DefDatabase.Get("Error.IncorrectGameRendererType") is IMessageDef messageDef)
                {
                    Console.WriteLine(messageDef.Text);
                }
                return;
            }

            var quizes = DefDatabase.GetAll<IQuizDef>().ToList();

            if (!quizes.Any())
            {
                if (DefDatabase.Get("Error.QuizesNotLoaded") is IMessageDef messageDef)
                {
                    Console.WriteLine(messageDef.Text);
                }
                return;
            }

            StringBuilder quizesList = new();
            foreach (var (index, quiz) in quizes.Select((x, i) => (i, x)))
            {
                if (index > 0)
                {
                    quizesList.Append('\n');
                }
                quizesList.Append($"{(index + 1)}. {quiz.Title}");
            }

            var mainMenuText = string.Format(ConsoleQuizGameDef.MainMenuText, quizesList.ToString());
            var quizInputText = string.Format(ConsoleQuizGameDef.QuizInputText, ConsoleQuizGameDef.ExitWord);

            bool isExited = false;
            while (!isExited)
            {
                Console.Title = ConsoleQuizGameDef.Title;

                renderer.DrawMessage(mainMenuText, MessageFlags.NoWait);

                renderer.DrawInput(quizInputText, (inputValue) =>
                {
                    if (string.Equals(ConsoleQuizGameDef.ExitWord.ToLower(), inputValue?.ToLower()))
                    {
                        isExited = true;
                        return;
                    }

                    if (!int.TryParse(inputValue, out int result) || result <= 0 || result > quizes.Count)
                    {
                        return;
                    }

                    var selectedQuiz = quizes[result - 1];

                    if (selectedQuiz.QuizProcessor != null && Activator.CreateInstance(selectedQuiz.QuizProcessor) is IQuizProcessor quizProcessor)
                    {
                        quizProcessor.StartQuiz(selectedQuiz);
                    }
                    else
                    {
                        if (DefDatabase.Get("Error.IncorrectQuizProcessor") is IMessageDef messageDef)
                        {
                            renderer.DrawMessage(messageDef.Text);
                        }
                    }
                });
            }
        }
    }
}