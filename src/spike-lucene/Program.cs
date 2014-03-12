using System;
using System.Collections.Generic;

namespace spike_lucene
{
    public class SimpleDocument
    {
        public string Title { get; private set; }
        public string Body { get; private set; }
        public string Resource { get; private set; }

        public IEnumerable<string> Tags { get; private set; } 

        public SimpleDocument(string title, string body, string resource, IEnumerable<string> tags )
        {
            Title = title;
            Body = body;
            Resource = resource;
            Tags = tags;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var storage = new DocumentStorage();

            storage.Add(new SimpleDocument("Title one", "Text in document 1","a/b/c",new []{"xml","json"}));
            storage.Add(new SimpleDocument("Title two", "Text in document 2", "d/e/f",new []{"xml"}));
            storage.Add(new SimpleDocument("Title three", "Text in document 3", "g/h/i",new []{"json"}));

            var textResults = storage.DocumentsByTextSearch("\"document 2\"");
            PrintResult(textResults);

            var tagResult = storage.DocumentsByTag("json");
            PrintResult(tagResult);

            var resourceResult = storage.DocumentsByResource("d/e/f");
            PrintResult(resourceResult);

            Console.WriteLine("<end>");
            Console.ReadKey();
        }

        private static void PrintResult(IEnumerable<SimpleDocument> result)
        {
            foreach (var document in result)
            {
                Console.WriteLine(document.Title + "\n" + document.Body + "\n" + document.Resource);
                Console.WriteLine("-----");
            }
            Console.WriteLine();
        }
    }
}
