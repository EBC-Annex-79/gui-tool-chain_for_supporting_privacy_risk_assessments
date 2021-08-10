using System;
using System.Collections;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Writing;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using System.Reflection;
using System.IO;

namespace Library.RDF
{
    public class RDFStoreManager
    {
        private readonly Graph _graph;
        private readonly TripleStore _tripleStore;
        private readonly TurtleParser _turtleParser;
        private readonly CompressingTurtleWriter _turtleWriter;
        private readonly ISparqlQueryProcessor _processor;
        private readonly SparqlQueryParser _sqlQueryParser;

        public RDFStoreManager(RDFMode mode)
        {
            _graph = new Graph();
            _tripleStore = new TripleStore();
            _tripleStore.Add(_graph);
            _turtleParser = new TurtleParser();
            _processor = new LeviathanQueryProcessor(this._tripleStore);
            _sqlQueryParser = new SparqlQueryParser();
            _turtleWriter = new CompressingTurtleWriter { CompressionLevel = WriterCompressionLevel.High };

            // var pathToFile = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).GetLeftPart(UriPartial.Path);
            // var path = new Uri(pathToFile.Substring(0, pathToFile.LastIndexOf('/'))+ "/../../../").AbsolutePath + "Schemas/";
            var path = AppDomain.CurrentDomain.BaseDirectory + "Schemas/";

            switch (mode)
            {
                case RDFMode.SmartBuilding:
                    LoadTurtleFile(path + "smartbuildingprivacyvunl.ttl");
                    goto case RDFMode.Genral;
                case RDFMode.SmartHome:
                    LoadTurtleFile(path + "smarthomeprivacyvunl.ttl");
                    goto case RDFMode.Genral;
                case RDFMode.Genral:
                    LoadTurtleFile(path + "privacyvunl.ttl");
                    LoadTurtleFile(path + "privacyvunlv2.ttl");
                    break;
                default:
                    break;
            }
        }

        public void AddGraphFromString(string description)
        {
            var reader = new StringReader(description);
            _turtleParser.Load(_graph, reader);
            reader.Dispose();
        }

        public void LoadTurtleFile(string filePath)
        {
            _turtleParser.Load(_graph, filePath);
        }

        public void AddGraph(IGraph graph)
        {
            _graph.Assert(graph.Triples);
        }

        public SparqlResultSet QueryFromFile(string filePath)
        {
            SparqlQuery query = _sqlQueryParser.ParseFromFile(filePath);
            return (SparqlResultSet)_processor.ProcessQuery(query);
        }

        public SparqlResultSet QueryFromString(string query)
        {
            return (SparqlResultSet)_processor.ProcessQuery(_sqlQueryParser.ParseFromString(query));
        }

        public void WriteToTurtleFile(string filepath)
        {
            _turtleWriter.Save(_graph, filepath);
        }
    }

    public static class RDFStoreManagerExtensions
    {
        public static void SaveToTurtleFile(this IGraph graph, string filepath)
        {
            var writer = new CompressingTurtleWriter { CompressionLevel = WriterCompressionLevel.High };
            writer.Save(graph, filepath);
        }
    }

    public enum RDFMode
    {
        SmartBuilding,SmartHome, Genral
    }
}


