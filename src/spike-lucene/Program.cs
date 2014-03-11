using System;

namespace spike_lucene
{
    public class SimpleDocument
    {
        public string Title { get; private set; }
        public string Body { get; private set; }
        public string Resource { get; private set; }

        public SimpleDocument(string title, string body, string resource)
        {
            Title = title;
            Body = body;
            Resource = resource;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var storage = new DocumentStorage();

            storage.Add(new SimpleDocument("Title one", "Text in document 1","/a/b/c"));
            storage.Add(new SimpleDocument("Title two", "Text in document 2", "/d/e/f"));
            storage.Add(new SimpleDocument("Title three", "Text in document 3", "/g/h/i"));

            var result = storage.Search("three");

            foreach (var document in result)
            {
                Console.WriteLine(document.Title + "\n" + document.Body + "\n" + document.Resource);
            }

            Console.ReadLine();

        }
    }
}
