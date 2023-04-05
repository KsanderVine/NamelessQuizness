using NamelessQuizness.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessQuizness.Workers;
using NamelessQuizness.Renderers;
using NamelessQuizness.Definitions;

namespace NamelessQuizness.Processors
{

    public class FriendlyQuizProcessor : IQuizProcessor
    {
        public void StartQuiz(IQuizDef quizDef)
        {
            if (quizDef is not FriendlyQuizDef)
            {
                throw new ArgumentNullException(nameof(quizDef),
                    "IQuizDef must have a type FriendlyQuizDef");
            }

            if (quizDef.QuizRenderer is null)
            {
                throw new NullReferenceException("Invalid or unknown QuizRenderer type");
            }

            FriendlyQuizDef friendlyQuizDef = (quizDef as FriendlyQuizDef)!;

            List<IQuestionWorker> questionWorkers = GetQuestionWorkers(quizDef)
                .Shuffle()
                .ToList();

            IQuizRenderer quizRenderer = (IQuizRenderer)Activator.CreateInstance(quizDef.QuizRenderer)!;
            quizRenderer.DrawMessage(friendlyQuizDef.Greet);

            int victoryPoints = 0;
            List<(int, IQuestionWorker, string?)> notAnswered = new();

            foreach (var (index, questionWorker) in questionWorkers.Select((x, i) => (i, x)))
            {
                quizRenderer.DrawQuestion(index, questionWorker);
                quizRenderer.DrawQuestionsCounter(index, questionWorkers.Count);
                quizRenderer.DrawInput(GetInputMessage(questionWorker), (answer) =>
                {
                    var result = questionWorker.IsAnswer(answer);

                    if (result)
                    {
                        victoryPoints++;
                    }
                    else
                    {
                        notAnswered.Add((index, questionWorker, answer?.Trim()));
                    }

                    if (questionWorkers.Count - (index + 1) is int questionsLeft && questionsLeft > 0)
                    {
                        quizRenderer.DrawMessage(friendlyQuizDef.AfterQuestionMessage,
                            MessageFlags.None, questionsLeft, questionWorkers.Count);
                    }
                });
            }

            if (victoryPoints == questionWorkers.Count)
            {
                quizRenderer.DrawMessage(friendlyQuizDef.VictoryMessage,
                    MessageFlags.None, victoryPoints, questionWorkers.Count);
            }
            else
            {
                StringBuilder questionsList = new();

                foreach (var (index, question, answer) in notAnswered)
                {
                    var description = $"{(index + 1)}. {GetShortDescription(question.GetDescription(), int.MaxValue)}";
                    questionsList.Append($"{description}\n\n" +
                        $"- Ваш ответ: {GetShortDescription(answer ?? string.Empty, int.MaxValue)}\n\n" +
                        $"- Ожидаемый ответ: {GetShortDescription(question.GetExpectedAnswer(), int.MaxValue)}\n\n\n");
                }

                quizRenderer.DrawMessage(friendlyQuizDef.DefeatMessage, 
                    MessageFlags.None, questionsList.ToString(), victoryPoints, questionWorkers.Count);
            }

            static string GetShortDescription (string description, int lengthLimit = 50)
            {
                if (description.Length <= lengthLimit)
                    return description;

                return $"{description[..(lengthLimit-3)]}...";
            }

            static IEnumerable<IQuestionWorker> GetQuestionWorkers(IQuizDef quizDef)
            {
                if (quizDef.QuestionsList != null)
                {
                    foreach (var questionDef in quizDef.QuestionsList.Questions)
                    {
                        if (GetQuestionWorkerOrFail(questionDef.QuestionWorker, questionDef) is IQuestionWorker questionWorker)
                        {
                            yield return questionWorker;
                        }
                    }
                }
            }

            static IQuestionWorker GetQuestionWorkerOrFail(Type? questionWorkerType, IQuestionDef questionDef)
            {
                if (questionWorkerType is null)
                {
                    throw new NullReferenceException("Question worker type is null or unknown");
                }
                else
                {
                    if (Activator.CreateInstance(questionWorkerType, questionDef) is IQuestionWorker questionWorker)
                    {
                        return questionWorker;
                    }
                    throw new NullReferenceException("Question worker not instantiated");
                }
            }

            static string GetInputMessage(IQuestionWorker questionWorker)
            {
                return questionWorker.QuestionDef.InputMessage?.Text ?? String.Empty;
            }
        }
    }
}
