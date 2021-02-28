using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsAgainstWhatever.Shared
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
            var question = UnplayedQuestions.Pop();
            PlayedQuestions.Push(question);

            return question;
        }

        public AnswerCard PickUpAnswer()
        {
            var answer = UnplayedAnswers.Pop();
            PlayedAnswers.Push(answer);

            return answer;
        }

        public IEnumerable<AnswerCard> PickUpAnswers(int count)
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
