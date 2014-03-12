using System.Collections.Generic;

namespace spike_lucene
{
    public interface IDocumentStorage
    {
        void Add(SimpleDocument document);

        IEnumerable<SimpleDocument> DocumentsByTextSearch(string search);

        IEnumerable<SimpleDocument> DocumentsByTag(string tag);

        IEnumerable<SimpleDocument> DocumentsByResource(string resource);
    }
}