using System;
using System.Collections.Generic;

namespace spike_lucene
{
    public class SimpleDocument
    {
        public int Id { get; private set; }

        public string Title { get; private set; }
        public string Body { get; private set; }
        public string Resource { get; private set; }

        public DateTime Published { get; private set; }

        public IEnumerable<string> Tags { get; private set; } 

        public SimpleDocument(int id, string title, string body, string resource, IEnumerable<string> tags, DateTime published )
        {
            Id = id;
            Title = title;
            Body = body;
            Resource = resource;
            Tags = tags;
            Published = published;
        }
    }
}