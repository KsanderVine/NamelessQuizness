using NamelessQuizness.Extensions;
using NamelessQuizness.Serialization;
using NamelessQuizness.Definitions;

namespace NamelessQuizness.Workers
{
    public class PickingQuestionWorker : IQuestionWorker
    {
        public class Answer
        {
            public int AnswerNumber { get; set; }
            public PickingQuestionDef.PickableAnswer PickableAnswer { get; set; }

            public Answer(int answerNumber, PickingQuestionDef.PickableAnswer pickableAnswer)
            {
                AnswerNumber = answerNumber;
                PickableAnswer = pickableAnswer;
            }
        }

        public IQuestionDef QuestionDef { get; private set; }

        private PickingQuestionDef PickingQuestionDef { get; set; }

        private List<Answer> Answers { get; set; }

        public PickingQuestionWorker(IQuestionDef questionDef)
        {
            QuestionDef = questionDef;

            if (questionDef is PickingQuestionDef pickingQuestionDef)
            {
                PickingQuestionDef = pickingQuestionDef;
            }
            else
            {
                throw new ArgumentNullException(nameof(questionDef),
                    "QuestionDef must have a type PickingQuestionDef");
            }

            var answers = new List<PickingQuestionDef.PickableAnswer>(PickingQuestionDef.Answers);
            Answers = answers.Shuffle().Select((a, i) => new Answer(i + 1, a)).ToList();
        }

        public IEnumerable<string>? GetAnswersFormatted()
        {
            foreach (var answer in Answers)
            {
                yield return $"{answer.AnswerNumber}) {answer.PickableAnswer.Text}";
            }
        }

        public string GetDescription()
        {
            return PickingQuestionDef.Text;
        }

        public bool IsAnswer(string? answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return false;

            return Answers
                .ElementAtOrDefault(ParseHelper.ParseInt(answer!) - 1)?
                .PickableAnswer.IsCorrectAnswer ?? false;
        }

        public string GetExpectedAnswer()
        {
            return Answers
                .First(a => a.PickableAnswer.IsCorrectAnswer)
                .AnswerNumber.ToString();
        }
    }
}
