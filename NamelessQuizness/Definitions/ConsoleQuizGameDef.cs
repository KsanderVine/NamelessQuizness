using NamelessQuizness.Renderers;

namespace NamelessQuizness.Definitions
{
    public class ConsoleQuizGameDef : IQuizGameDef
    {
        public string DefKey { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ExitWord { get; set; } = string.Empty;

        public string MainMenuText { get; set; } = string.Empty;
        public string QuizInputText { get; set; } = string.Empty;

        public Type QuizGame { get; set; } = typeof(ConsoleQuizGame);
        public Type GameRenderer { get; set; } = typeof(ConsoleQuizRenderer);
    }
}