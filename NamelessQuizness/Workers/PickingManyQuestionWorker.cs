using NamelessQuizness.Extensions;
using NamelessQuizness.Serialization;
using NamelessQuizness.Definitions;

namespace NamelessQuizness.Workers
{
    public class PickingManyQuestionWorker : IQuestionWorker
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

        public PickingManyQuestionWorker(IQuestionDef questionDef)
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

            List<int> answers = answer
                .Split(',')
                .Select(x => ToInt(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            List<int> correctAnswers = Answers
                .Where(a => a.PickableAnswer.IsCorrectAnswer)
                .Select(a => a.AnswerNumber)
                .OrderBy(a => a)
                .ToList();

            return correctAnswers.SequenceEqual(answers);

            static int ToInt(string x)
            {
                if (!int.TryParse(x.Trim(), out int result))
                {
                    result = 0;
                }
                return result;
            }
        }

        public string GetExpectedAnswer()
        {
            List<int> correctAnswers = Answers
                .Where(a => a.PickableAnswer.IsCorrectAnswer)
                .Select(a => a.AnswerNumber)
                .OrderBy(a => a)
                .ToList();

            return string.Join(", ", correctAnswers);
        }
    }
}
