using System;
using System.Collections.Generic;

namespace spike_lucene
{
    class Program
    {
        static void Main(string[] args)
        {
            var storage = new DocumentStorage();

            storage.Add(new SimpleDocument(1,"Title one", "Text in document 1","a/b/c",new []{"xml","json", "i os"}, new DateTime(2014,1,1)));
            storage.Add(new SimpleDocument(2,"Title two", "Text in document 2", "d/e/f",new []{"xml", "go"}, new DateTime(2015,1,1)));
            storage.Add(new SimpleDocument(3,"Title three", "Text in document 3", "g/h/i",new []{"json"}, new DateTime(2014,1,1)));

            Console.WriteLine("Text search");
            var textResults = storage.DocumentsByTextSearch("\"document 2\"");
            PrintResult(textResults);

            Console.WriteLine("Tag search");
            var tagsResult = storage.DocumentsByTags(new[]{"xml", "go"});
            PrintResult(tagsResult);

            Console.WriteLine("Resource search");
            var resourceResult = storage.DocumentsByResource("g/h");
            PrintResult(resourceResult);

            Console.WriteLine("Id search");
            var idResult = storage.DocumentById(2);
            PrintResult(idResult);

            
            Console.WriteLine("<end>");
            Console.ReadKey();
        }

        private static void PrintResult(IEnumerable<SimpleDocument> result)
        {
            foreach (var document in result)
            {
                PrintResult(document);
            }
            Console.WriteLine();
        }

        private static void PrintResult(SimpleDocument document)
        {
            Console.WriteLine("(" + document.Id + ") " + document.Title + "\n" + document.Body + "\n" + document.Resource + "\n" + document.Published);
            Console.WriteLine(string.Join(",", document.Tags));
            Console.WriteLine("-----");            
        }
    }
}
