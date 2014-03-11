using System.Collections.Generic;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace spike_lucene
{
    public class DocumentStorage : IDocumentStorage
    {
        private readonly RAMDirectory _ramStorage;
        private IndexWriter _indexWriter;

        public DocumentStorage()
        {
            _ramStorage = new RAMDirectory();
        }

        public void Add(SimpleDocument document)
        {
            _indexWriter = new IndexWriter(_ramStorage, new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);

            var doc = new Document();
            doc.Add(new Field("body", document.Body, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("resource", document.Resource, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("title", document.Title, Field.Store.YES, Field.Index.ANALYZED));

            _indexWriter.AddDocument(doc);

            _indexWriter.Optimize();
            _indexWriter.Dispose();
        }

        public IEnumerable<SimpleDocument> Search(string search)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            var searcher = new IndexSearcher(_ramStorage, true);

            var parser = new MultiFieldQueryParser(Version.LUCENE_30, new []{ "body", "title"}, analyzer);
            var query = parser.Parse(search);

            var hits = searcher.Search(query, 10);
            for (var ii = 0; ii < hits.TotalHits; ii++)
            {
                var aDock = searcher.Doc(hits.ScoreDocs[ii].Doc);
                yield return new SimpleDocument(aDock.Get(@"title"), aDock.Get(@"body"), aDock.Get(@"resource"));
            }

            searcher.Dispose();            
        }

    }
}