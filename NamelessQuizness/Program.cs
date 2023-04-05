using NamelessQuizness.Definitions;
using NamelessQuizness.Serialization;

namespace NamelessQuizness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!XmlDefsLoader.TryLoadAllDefs())
            {
                Console.WriteLine("Definitions not loaded");
                return;
            }

            if (DefDatabase.GetAll<IQuizGameDef>().FirstOrDefault() is IQuizGameDef quizGameDef)
            {
                if (Activator.CreateInstance(quizGameDef.QuizGame, quizGameDef) is IQuizGame quizGame)
                {
                    quizGame.Play();
                }
            }
        }
    }
}