using System.Collections.Generic;
using Library.PrivacyModel;
using Newtonsoft.Json;

namespace Library.RDF
{
    public class Converter {

        

        private string NameSpace = "https://ontology.hviidnet.com/2020/01/03/privacyvunl-model.ttl#";

        public Converter(string nameSpace = null)
        {
            NameSpace = nameSpace != null ? nameSpace : "https://ontology.hviidnet.com/2020/01/03/privacyvunl-model.ttl#";
        }

        public string ConverterToRDF(List<Node> nodes, List<Link> links, RDFClient rDFClient)
        {
            string strNodes = ConverterNodesToRDF(nodes);
            string strLinks = ConverterLinksToRDF(links, rDFClient);

            return "{ \"nodes\" : " + strNodes + ", \"links\" :  " + strLinks + ", \"namespace\" : \"" + NameSpace  + "\" }";
        }

        private string ConverterNodesToRDF(List<Node> nodes){
            
            foreach (Node node in nodes)
            {
                node.Address = NameSpace + node.Name;
            }

            string jsonString = JsonConvert.SerializeObject(nodes);

            return jsonString;
        }

         private string ConverterLinksToRDF(List<Link> links, RDFClient rDFClient){
            List<OntologyLink> ontologyLinks = new List<OntologyLink>();
            Dictionary<string,Relation> relations = rDFClient.findOntologyRelations();

            foreach (Link link in links)
            {
                ModelItem sourceSuper = findSuperModel(link.Source.AddressType, rDFClient);
                ModelItem targetSuper = findSuperModel(link.Target.AddressType, rDFClient);
                if (relations.ContainsKey(sourceSuper.Address+targetSuper.Address))
                {
                    string relationName = relations[sourceSuper.Address+targetSuper.Address].RelationName;
                    ontologyLinks.Add(new OntologyLink(relationName,link.Source.Address,link.Target.Address));
                }
            }

            string jsonString = JsonConvert.SerializeObject(ontologyLinks);

            return jsonString;
        }

        private ModelItem findSuperModel(string classAddress,RDFClient rDFClient)
        {
            Dictionary<string, ModelItem>  dataStrucks = rDFClient.models;
            if (dataStrucks.ContainsKey(classAddress.Split("#")[1]) && dataStrucks.ContainsKey(dataStrucks[classAddress.Split("#")[1]].RootSuperClass))
            {
                return dataStrucks[dataStrucks[classAddress.Split("#")[1]].RootSuperClass];
            }

            return null;
        }


    }
}