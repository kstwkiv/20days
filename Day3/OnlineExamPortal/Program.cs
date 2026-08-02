using System;
using System.Collections.Generic;

namespace ExamPortal
{
    class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public string Topic { get; set; }
        public string Type { get; set; }
    }

    class Portal
    {
        private List<Question> questions = new List<Question>();

        public void AddQuestion(Question q)
        {
            questions.Add(q);
        }

        // Find out the total number of question
        public int GetTotalQuestions()
        {
            return questions.Count;
        }

        // List all question belonging to a topic
        public List<Question> GetQuestionsByTopic(string topic)
        {
            List<Question> result = new List<Question>();
            foreach (var q in questions)
            {
                if (q.Topic == topic)
                {
                    result.Add(q);
                }
            }
            return result;
        }

        // List all question belonging to a topic and category
        public List<Question> GetQuestionsByTopicAndCategory(string topic, string category)
        {
            List<Question> result = new List<Question>();
            foreach (var q in questions)
            {
                if (q.Topic == topic && q.Category == category)
                {
                    result.Add(q);
                }
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Portal portal = new Portal();
            portal.AddQuestion(new Question { Id = 1, Text = "What is C#?", Category = "Programming", Topic = "C# Basics", Type = "Paragraph" });
            portal.AddQuestion(new Question { Id = 2, Text = "Which loop executes at least once?", Category = "Programming", Topic = "Loops", Type = "MCQ" });
            portal.AddQuestion(new Question { Id = 3, Text = "What is CLR?", Category = "Framework", Topic = "C# Basics", Type = "MCQ" });

            Console.WriteLine($"Total questions: {portal.GetTotalQuestions()}");

            Console.WriteLine("\nQuestions in topic 'C# Basics':");
            foreach (var q in portal.GetQuestionsByTopic("C# Basics"))
            {
                Console.WriteLine($"- {q.Text}");
            }

            Console.WriteLine("\nQuestions in topic 'C# Basics' and category 'Framework':");
            foreach (var q in portal.GetQuestionsByTopicAndCategory("C# Basics", "Framework"))
            {
                Console.WriteLine($"- {q.Text}");
            }
        }
    }
}