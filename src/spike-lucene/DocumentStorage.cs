using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
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
            if (_indexWriter == null)
                _indexWriter = new IndexWriter(new RAMDirectory(), new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);
        }
        
        public void Add(SimpleDocument document)
        {
            var doc = new Document();

            doc.Add(new Field("id", document.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            doc.Add(new Field("title", document.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("body", document.Body, Field.Store.YES, Field.Index.ANALYZED));

            var parts = document.Resource.Split('/');
            foreach (var part in parts) {
                doc.Add(new Field("resources", part, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
            }

            foreach (var tag in document.Tags) {
                doc.Add(new Field("tags", tag, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
            }

            doc.Add(new Field("date", DateTools.DateToString(document.Published, DateTools.Resolution.DAY), Field.Store.YES, Field.Index.ANALYZED));

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

        public IEnumerable<SimpleDocument> DocumentsByTags(IEnumerable<string> tags)
        {
            var reader = _indexWriter.GetReader();
            using (var searcher = new IndexSearcher(reader))
            {
                var parser = new QueryParser(Version.LUCENE_30, "tags", _standardAnalyzer);
                var baked = string.Join(" or ", tags);
                var query = parser.Parse(baked);

                var hits = searcher.Search(query, MaxResult);

                return BuildResult(hits, searcher);
            }            
        }

        public IEnumerable<SimpleDocument> DocumentsByResource(string resource)
        {
            var reader = _indexWriter.GetReader();
            using (var searcher = new IndexSearcher(reader))
            {
                var parser = new QueryParser(Version.LUCENE_30, "resources", _standardAnalyzer);

                var parts = resource.Split('/');
                var baked = string.Join(" ", parts);

                var query = parser.Parse(baked);

                var hits = searcher.Search(query, MaxResult);

                return BuildResult(hits, searcher);
            }            
        }

        public SimpleDocument DocumentById(int id)
        {
            var reader = _indexWriter.GetReader();
            using (var searcher = new IndexSearcher(reader))
            {
                var termQuery = new TermQuery(new Term("id", id.ToString()));
                var booleanQuery = new BooleanQuery { { termQuery, Occur.MUST } };
                var hits = searcher.Search(booleanQuery, 1);

                return BuildResult(hits, searcher).FirstOrDefault();
            }            
        }

        private static IEnumerable<SimpleDocument> BuildResult(TopDocs hits, IndexSearcher searcher)
        {
            for (var ii = 0; ii < hits.TotalHits; ii++)
            {
                var aDock = searcher.Doc(hits.ScoreDocs[ii].Doc);                
                var resource = string.Join("/", aDock.GetValues("resources"));
                var date = DateTools.StringToDate(aDock.Get("date"));

                yield return new SimpleDocument(int.Parse(aDock.Get(@"id")), aDock.Get(@"title"), aDock.Get(@"body"), resource, aDock.GetValues("tags"), date);
            }
        }

    }
}