using System.Collections.Generic;

namespace spike_lucene
{
    public interface IDocumentStorage
    {
        void Add(SimpleDocument document);

        SimpleDocument DocumentById(int id);

        IEnumerable<SimpleDocument> DocumentsByTextSearch(string search);

        IEnumerable<SimpleDocument> DocumentsByTags(IEnumerable<string> tags);

        IEnumerable<SimpleDocument> DocumentsByResource(string resource);
    }
}