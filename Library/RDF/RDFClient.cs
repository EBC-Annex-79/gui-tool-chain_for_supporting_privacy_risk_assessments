using System;
using System.Collections;
using System.Collections.Generic;
using VDS.RDF.Query;
using Library.PrivacyModel;

namespace Library.RDF
{
    public class RDFClient
    {
        private readonly RDFStoreManager _rDFStoreManager;

        private ArrayList domainDataStrucktor;

        public Dictionary<String, ModelItem> models;

        public RDFClient(RDFMode mode)
        {
            _rDFStoreManager = new RDFStoreManager(mode);
            domainDataStrucktor = new ArrayList();
            models = new Dictionary<string, ModelItem>();
        }

        public ArrayList FindDataTypesAndContexts()
        {
            FindContextTypesForDomain(models);
            FindDataTypesForDomain(models);
            _findDatatypeProperty(models);
            return domainDataStrucktor;
        }

        private void FindDataTypesForDomain(Dictionary<String, ModelItem> dataStruck)
        {
            string q = AppendPrefixToQuery();

            q += "" +
                "SELECT distinct ?root_data ?sub_data" +
                " WHERE {" +
                "   ?root_data a owl:Class ." +
                "   ?root_data rdfs:subClassOf* pv:Data ." +
                "   ?sub_data  rdfs:subClassOf ?root_data ." +
                "}";

            SparqlResultSet dataStrucks = _rDFStoreManager.QueryFromString(q);

            //Dict for holding logic
            ModelItem dataBaseClass = null;
            ModelItem dataItemSuper = null;

            foreach (var i in dataStrucks)
            {
                var address_root_data_type = i[0];
                var address_sub_data_type = i[1];

                string nameSuperDataType = NameOfSubjekt(address_root_data_type.ToString());
                string nameSubDataType = NameOfSubjekt(address_sub_data_type.ToString());

                if (!dataStruck.ContainsKey(nameSuperDataType))
                {
                    dataItemSuper = new ModelItem(nameSuperDataType, address_root_data_type.ToString());
                    dataItemSuper.RootSuperClass = "Data";
                    if (nameSuperDataType == "Data")
                    {
                        dataBaseClass = dataItemSuper;
                    }
                    dataStruck.Add(nameSuperDataType, dataItemSuper);
                }

                var dataItem = new ModelItem(nameSubDataType, address_sub_data_type.ToString());
                dataItem.RootSuperClass = "Data";

                //add attributes for super element to subelement
                if (dataItemSuper != null)
                    dataItem.AddAttributes(dataItemSuper.Attributes);

                if (!dataStruck.ContainsKey(nameSubDataType))
                    dataStruck.Add(nameSubDataType, dataItem);
                dataStruck[nameSuperDataType].AddSubModelItem(dataItem);
            }

            _findNamedIndividualDataTypes(dataStruck);
            domainDataStrucktor.Add(dataBaseClass);
        }

        public Dictionary<string, Relation> findOntologyRelations()
        {
            string q = AppendPrefixToQuery();
            q += "SELECT distinct ?relation ?domain ?range" +
                " WHERE {" +
                "   ?relation rdf:type owl:ObjectProperty . " +
                "   ?relation rdfs:domain	 ?domain . " +
                "   ?relation rdfs:range	 ?range . " +
                "}";

            SparqlResultSet dataStrucks = _rDFStoreManager.QueryFromString(q);

            Dictionary<string, Relation> dictRelations = new Dictionary<string, Relation>();

            foreach (var i in dataStrucks)
            {
                var relationName = i[0];
                var domain = i[1];
                var range = i[2];

                if (domain.ToString().Contains("Context") || domain.ToString().Contains("Data"))
                {
                    if (domain.ToString().Contains("Context") && range.ToString().Contains("Data") && !relationName.ToString().Contains("has"))
                        continue;
                    else if (domain.ToString().Contains("Context") && range.ToString().Contains("Context") && !relationName.ToString().Contains("star"))
                        continue;
                    else if (domain.ToString().Contains("Data") && range.ToString().Contains("Data") && !relationName.ToString().Contains("star"))
                        continue;

                    if (dictRelations.ContainsKey(domain.ToString() + range.ToString()))
                        Console.WriteLine("Error!");
                    else
                        dictRelations.Add(domain.ToString()+range.ToString(),new Relation(relationName.ToString(),domain.ToString(), range.ToString()));
                }
            }
            return dictRelations;
        }

        private void _findNamedIndividualDataTypes(Dictionary<string, ModelItem> dataStruck)
        {
            string q = AppendPrefixToQuery();
            q += "" +
                "SELECT distinct ?individualData_name ?dataTypes" +
                " WHERE {" +
                "   ?individualData_name rdf:type owl:NamedIndividual . " +
                "   ?dataTypes rdfs:subClassOf pv:Data . " +
                "	?individualData_name  rdf:type ?dataTypes . " +
                "}";

            SparqlResultSet dataStrucks = _rDFStoreManager.QueryFromString(q);

            ModelItem dataItemSuper = null;

            foreach (var i in dataStrucks)
            {
                var address_data_type = i[0];
                var address_sub_data = i[1];

                string nameSuperDataType = NameOfSubjekt(address_data_type.ToString());
                string nameSubDataType = NameOfSubjekt(address_sub_data.ToString());

                if (!dataStruck.ContainsKey(nameSuperDataType))
                {
                    dataItemSuper = new ModelItem(nameSuperDataType, address_data_type.ToString());
                    dataItemSuper.RootSuperClass = "Data";
                    dataStruck.Add(nameSuperDataType, dataItemSuper);
                }
                else
                {
                    dataItemSuper = dataStruck[nameSuperDataType];
                }

                var dataItem = new ModelItem(nameSubDataType, address_sub_data.ToString());
                dataItem.RootSuperClass = "Data";
                if (dataItemSuper != null)
                    dataItem.AddAttributes(dataItemSuper.Attributes);

                if (!dataStruck.ContainsKey(nameSubDataType))
                    dataStruck.Add(nameSubDataType, dataItem);
                dataStruck[nameSuperDataType].AddSubModelItem(dataItem);
            }
        }

        private void _findDatatypeProperty(Dictionary<string, ModelItem> domainDataStrucktor)
        {
            string q = AppendPrefixToQuery();

            q += "SELECT DISTINCT ?property ?dataType ?subject " +
                 "WHERE { " +
                    "?subject rdf:type owl:DatatypeProperty ." +
                    "?subject rdfs:domain ?property . " +
                    "?subject rdfs:range ?dataType ." +
                 "}";

            SparqlResultSet datatypeProperty = _rDFStoreManager.QueryFromString(q);

            foreach (var i in datatypeProperty)
            {
                var subject = i[2];
                var propertyName = i[0];
                var propertyDataType = i[1];

                string name_subject = NameOfSubjekt(subject.ToString());
                string datatypeProperty_name = NameOfSubjekt(propertyName.ToString());
                string xsd_datatype = NameOfSubjekt(propertyDataType.ToString());

                if (domainDataStrucktor.ContainsKey(name_subject))
                {
                    PrivacyModel.Attribute attribute = new PrivacyModel.Attribute(datatypeProperty_name, xsd_datatype);

                    domainDataStrucktor[name_subject].AddAttributes(attribute);
                }
            }
        }

        private void FindContextTypesForDomain(Dictionary<String, ModelItem> contextStruck)
        {
            string q = AppendPrefixToQuery();

            q += "" +
                "SELECT distinct ?root_context ?sub_context" +
                " WHERE {" +
                "   ?root_context a owl:Class ." +
                "   ?root_context rdfs:subClassOf* pv2:Context ." +
                "   ?sub_context  rdfs:subClassOf ?root_context ." +
                "}";

            SparqlResultSet contextStrucks = _rDFStoreManager.QueryFromString(q);

            //Dict for holding logic
            ModelItem contextBase = null;
            ModelItem contextItemSuper = null;


            foreach (var i in contextStrucks)
            {
                var address_super_context = i[0];
                var address_sub_context = i[1];

                string name_super_context = NameOfSubjekt(address_super_context.ToString());
                string name_sub_context = NameOfSubjekt(address_sub_context.ToString());

                if (!contextStruck.ContainsKey(name_super_context))
                {
                    contextItemSuper = new ModelItem(name_super_context, address_super_context.ToString());
                    contextItemSuper.RootSuperClass = "Context";
                    if (name_super_context == "Context")
                    {
                        contextBase = contextItemSuper;
                    }
                    contextStruck.Add(name_super_context, contextItemSuper);
                }
                else{
                    contextItemSuper = contextStruck[name_super_context];
                }
                ModelItem contextItem =  null;
                if (!contextStruck.ContainsKey(name_sub_context))
                {
                    contextItem = new ModelItem(name_sub_context, address_sub_context.ToString());
                    contextItem.RootSuperClass= "Context";
                    contextStruck.Add(name_sub_context, contextItem);
                    }
                else
                    contextItem = contextStruck[name_sub_context];

                //add attributes for super element to subelement
                if (contextItemSuper != null)
                {
                    contextItem.AddAttributes(contextItemSuper.Attributes);
                    contextStruck[name_super_context].AddSubModelItem(contextItem);
                }

                

                

            }

            domainDataStrucktor.Add(contextBase);
        }

        private string NameOfSubjekt(string contextAddress)
        {
            string name = contextAddress.Split("#")[1];
            return name;
        }
        private string AppendPrefixToQuery()
        {
            string query = "";
            var namespaces = RDFNamespace.getRDFNamespace().GetNamespaces();

            foreach (KeyValuePair<string, string> de in namespaces)
            {
                query += "PREFIX " + de.Key + ": <" + de.Value + "> ";
            }

            return query;
        }
    }
}
