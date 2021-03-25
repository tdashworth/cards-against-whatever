using System;
using System.Collections.Generic;

namespace CardsAgainstWhatever.Shared.Models
{
    public class CardDeck
    {

        private readonly Stack<QuestionCard> UnplayedQuestions;
        private readonly Stack<QuestionCard> PlayedQuestions;

        private readonly Stack<AnswerCard> UnplayedAnswers;
        private readonly Stack<AnswerCard> PlayedAnswers;

        public CardDeck(IEnumerable<QuestionCard> questionCards, IEnumerable<AnswerCard> answerCards)
        {
            UnplayedQuestions = new Stack<QuestionCard>(questionCards);
            UnplayedAnswers = new Stack<AnswerCard>(answerCards);
            PlayedQuestions = new Stack<QuestionCard>();
            PlayedAnswers = new Stack<AnswerCard>();
        }

        public QuestionCard PickUpQuestion()
        {
            if (UnplayedQuestions.Count == 0)
            {
                new Exception("There are no more question cards available.");
            }
            var question = UnplayedQuestions.Pop();
            PlayedQuestions.Push(question);

            return question;
        }

        public AnswerCard PickUpAnswer()
        {
            if (UnplayedAnswers.Count == 0)
            {
                new Exception("There are no more answer cards available.");
            }

            var answer = UnplayedAnswers.Pop();
            PlayedAnswers.Push(answer);

            return answer;
        }

        public List<AnswerCard> PickUpAnswers(int count)
        {
            var answers = new List<AnswerCard>();
            for (int i = 0; i < count; i++)
            {
                answers.Add(PickUpAnswer());
            }
            return answers;
        }
    }
}
