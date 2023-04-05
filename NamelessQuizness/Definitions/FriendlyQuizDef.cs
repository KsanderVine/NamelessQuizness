using NamelessQuizness.Processors;
using NamelessQuizness.Renderers;

namespace NamelessQuizness.Definitions
{
    public class FriendlyQuizDef : IQuizDef
    {
        public string DefKey { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Greet { get; set; } = string.Empty;
        public string AfterQuestionMessage { get; set; } = string.Empty;
        public string VictoryMessage { get; set; } = string.Empty;
        public string DefeatMessage { get; set; } = string.Empty;

        public Type? QuizProcessor { get; set; } = typeof(FriendlyQuizProcessor);
        public Type? QuizRenderer { get; set; } = typeof(ConsoleQuizRenderer);

        public IQuestionsListDef? QuestionsList { get; set; }
    }
}