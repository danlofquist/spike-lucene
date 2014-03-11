using System.Collections.Generic;

namespace spike_lucene
{
    public interface IDocumentStorage
    {
        void Add(SimpleDocument document);

        IEnumerable<SimpleDocument> Search(string search); 
    }
}