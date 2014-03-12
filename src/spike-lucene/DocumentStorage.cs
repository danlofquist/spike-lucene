using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace spike_lucene
{
    public class DocumentStorage : IDocumentStorage
    {
        private const int MaxResult = 10;
        private readonly Analyzer _standardAnalyzer;
        private static IndexWriter _indexWriter;

        public DocumentStorage()
        {
            _standardAnalyzer = new StandardAnalyzer(Version.LUCENE_30);
            _indexWriter = new IndexWriter(new RAMDirectory(), new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);
        }
        
        public void Add(SimpleDocument document)
        {
            var doc = new Document();

            doc.Add(new Field("title", document.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("body", document.Body, Field.Store.YES, Field.Index.ANALYZED));

            doc.Add(new Field("resource", document.Resource, Field.Store.YES, Field.Index.NOT_ANALYZED));

            foreach (var tag in document.Tags) {
                var tags = new Field("tags", tag, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES);
                doc.Add(tags);
            }

            _indexWriter.AddDocument(doc);
            _indexWriter.Optimize();
        }

        public IEnumerable<SimpleDocument> DocumentsByTextSearch(string search)
        {
            var reader = _indexWriter.GetReader();
            using (var searcher = new IndexSearcher(reader))
            {
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, new []{ "body", "title"}, _standardAnalyzer);
                var query = parser.Parse(search);

                var hits = searcher.Search(query, MaxResult);
                return BuildResult(hits, searcher);
            }
        }

        public IEnumerable<SimpleDocument> DocumentsByTag(string tag)
        {
            var reader = _indexWriter.GetReader();
            using (var searcher = new IndexSearcher(reader))
            {
                var searchTerm = new Term("tags", tag);
                var query = new TermQuery(searchTerm);
                var hits = searcher.Search(query, MaxResult);

                return BuildResult(hits, searcher);
            }
        }

        public IEnumerable<SimpleDocument> DocumentsByResource(string resource)
        {
            var reader = _indexWriter.GetReader();
            using (var searcher = new IndexSearcher(reader))
            {
                var termQuery = new TermQuery(new Term("resource", resource));
                var booleanQuery = new BooleanQuery {{termQuery, Occur.MUST}};
                var hits = searcher.Search(booleanQuery, MaxResult);

                return BuildResult(hits, searcher);
            }            
        }

        private static IEnumerable<SimpleDocument> BuildResult(TopDocs hits, IndexSearcher searcher)
        {
            for (var ii = 0; ii < hits.TotalHits; ii++)
            {
                var aDock = searcher.Doc(hits.ScoreDocs[ii].Doc);
                yield return new SimpleDocument(aDock.Get(@"title"), aDock.Get(@"body"), aDock.Get(@"resource"), null);
            }
        }

    }
}