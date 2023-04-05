using NamelessQuizness.Definitions;
using NamelessQuizness.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NamelessQuizness.Workers
{
    public class FillTheGapsQuestionWorker : IQuestionWorker
    {
        private class TextToken
        {
            public readonly string originalText;
            public readonly string clearToken;
            public bool IsGap { get; set; }

            public string GapOrText => IsGap ? "..." : originalText;

            public TextToken(string originalText, bool isGap = false)
            {
                this.originalText = clearToken = originalText;
                IsGap = isGap;

                if (Regex.Match(originalText) is Match match && match.Success)
                {
                    clearToken = match.Value.Trim();
                }
            }
        }

        private static readonly Random Random = new();
        private static readonly Regex Regex = new(@"^([\w-]{2,})+$", RegexOptions.CultureInvariant);

        public IQuestionDef QuestionDef { get; private set; }

        private FillTheGapsQuestionDef FillTheGapsQuestionDef { get; set; }

        private List<TextToken> Tokens { get; set; } = new List<TextToken>();

        private string Description { get; set; } = string.Empty;

        public FillTheGapsQuestionWorker(IQuestionDef questionDef)
        {
            QuestionDef = questionDef;

            if (questionDef is FillTheGapsQuestionDef fillTheGapsQuestionDef)
            {
                FillTheGapsQuestionDef = fillTheGapsQuestionDef;
            }
            else
            {
                throw new ArgumentNullException(nameof(questionDef),
                    "QuestionDef must have a type FillTheGapsQuestionDef");
            }

            PrepareWorker();
        }

        private void PrepareWorker ()
        {
            Tokens = FillTheGapsQuestionDef.Text
                .Split(' ', options: StringSplitOptions.RemoveEmptyEntries)
                .Select(t => new TextToken(t))
                .ToList();

            MakeGaps(FillTheGapsQuestionDef.GapsCount, Tokens);

            List<string> correctAnswers = Tokens
                .Where(t => t.IsGap)
                .Select(t => t.clearToken.ToLower())
                .ToList();

            correctAnswers.AddRange(GetDecoys(FillTheGapsQuestionDef.DecoysCount));
            correctAnswers = correctAnswers.Shuffle().ToList();
            var answers = string.Join(", ", correctAnswers);

            StringBuilder description = new();

            description.Append(string.Join(' ', Tokens.Select(t => t.GapOrText)));

            description.Append('\n');

            description.Append(string.Format(FillTheGapsQuestionDef.ConditionMessage?.Text ?? String.Empty, answers));

            Description = description.ToString();

            List<string> GetDecoys(int count)
            {
                return FillTheGapsQuestionDef.DecoysListDef?.Words.ToList()
                    .Shuffle().Take(count)
                    .Select(w => w.Trim().ToLower())
                    .ToList() ?? new List<string>();
            }
        }

        private void MakeGaps (int count, List<TextToken> tokens)
        {
            int gaps = count;

            foreach (var token in tokens)
            {
                if (gaps == 0)
                    break;

                if (token.IsGap)
                    continue;

                if (!Regex.IsMatch(token.clearToken))
                    continue;

                if (Random.Next(0, 100) > 10)
                    continue;

                gaps--;
                token.IsGap = true;
            }

            if (gaps > 0)
            {
                MakeGaps(gaps, tokens);
            }
        }

        public IEnumerable<string>? GetAnswersFormatted() => null;

        public string GetDescription() => Description;

        public bool IsAnswer(string? answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return false;

            List<string> correctAnswers = Tokens
                .Where(t => t.IsGap)
                .Select(t => t.clearToken.ToLower())
                .ToList();

            List<string> answers = answer
                .Split(',')
                .Select(t => t.Trim().ToLower())
                .ToList();

            return correctAnswers.SequenceEqual(answers);
        }

        public string GetExpectedAnswer ()
        {
            List<string> correctAnswers = Tokens
                .Where(t => t.IsGap)
                .Select(t => t.clearToken)
                .ToList();

            return string.Join(", ", correctAnswers);
        }
    }
}
