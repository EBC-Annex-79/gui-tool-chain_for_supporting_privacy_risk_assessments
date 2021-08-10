using System;
using System.Collections.Generic;

namespace Library.RDF
{
    public class RDFNamespace
    {
        private static RDFNamespace rDFNamespace;

        public static RDFNamespace getRDFNamespace()
        {
            if (rDFNamespace == null)
                rDFNamespace = new RDFNamespace();
            return rDFNamespace;
        }

        public string NSPrivacyVunlV1
        {
            get;
            private set;
        }

        public string PrefixePrivacyVunlV1
        {
            get;
            private set;
        }

        public string NSPrivacyVunlV2
        {
            get;
            private set;
        }

        public string PrefixePrivacyVunlV2
        {
            get;
            private set;
        }

        public string NSRDF
        {
            get;
            private set;
        }

        public string PrefixeRDF
        {
            get;
            private set;
        }

        public string NSRDFS
        {
            get;
            private set;
        }

        public string PrefixeRDFS
        {
            get;
            private set;
        }

        public string NSOWL
        {
            get;
            private set;
        }

        public string PrefixeOWL
        {
            get;
            private set;
        }

        public string NSXSD
        {
            get;
            private set;
        }

        public string PrefixeXSD
        {
            get;
            private set;
        }



        private RDFNamespace()
        {
            NSPrivacyVunlV1 = "https://ontology.hviidnet.com/2020/01/03/privacyvunl.ttl#";
            PrefixePrivacyVunlV1 = "pv";
            NSPrivacyVunlV2 = "https://ontology.hviidnet.com/2020/01/03/privacyvunlV2.ttl#";
            PrefixePrivacyVunlV2 = "pv2";
            NSRDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
            PrefixeRDF= "rdf";
            NSRDFS = "http://www.w3.org/2000/01/rdf-schema#";
            PrefixeRDFS = "rdfs";
            NSOWL = "http://www.w3.org/2002/07/owl#";
            PrefixeOWL = "owl";
            NSXSD = "http://www.w3.org/2001/XMLSchema#";
            PrefixeXSD = "xsd";

        }

        public Dictionary<string, string> GetNamespaces()
        {
            Dictionary<string, string> namespaces = new Dictionary<string, string>();
            namespaces.Add(PrefixePrivacyVunlV1, NSPrivacyVunlV1);
            namespaces.Add(PrefixePrivacyVunlV2, NSPrivacyVunlV2);
            namespaces.Add(PrefixeRDF,NSRDF);
            namespaces.Add(PrefixeRDFS,NSRDFS);
            namespaces.Add(PrefixeOWL, NSOWL);
            namespaces.Add(PrefixeXSD, NSXSD);
            return namespaces;
        }
    }
}
