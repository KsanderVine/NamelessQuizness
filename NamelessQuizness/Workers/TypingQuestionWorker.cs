using NamelessQuizness.Definitions;

namespace NamelessQuizness.Workers
{
    public class TypingQuestionWorker : IQuestionWorker
    {
        public IQuestionDef QuestionDef { get; private set; }
        private TypingQuestionDef TypingQuestionDef { get; set; }

        public TypingQuestionWorker(IQuestionDef questionDef)
        {
            QuestionDef = questionDef;

            if (questionDef is TypingQuestionDef typingQuestionDef)
            {
                TypingQuestionDef = typingQuestionDef;
            }
            else
            {
                throw new ArgumentNullException(nameof(questionDef),
                    "QuestionDef must have a type TypingQuestionDef");
            }
        }

        public string GetDescription()
        {
            return TypingQuestionDef.Text;
        }

        public IEnumerable<string>? GetAnswersFormatted()
        {
            return null;
        }

        public bool IsAnswer(string? answer)
        {
            answer = answer?.Trim().ToLower();

            foreach (var acceptedAnswer in TypingQuestionDef.AcceptedAnswers)
            {
                if(string.Equals(acceptedAnswer.ToLower(), answer))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetExpectedAnswer()
        {
            return TypingQuestionDef.AcceptedAnswers.First().ToLower();
        }
    }
}
