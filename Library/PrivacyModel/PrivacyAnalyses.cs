using System.Collections.Generic;
using System.Text;
using Library.RDF;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Library.PrivacyModel
{
    public class PrivacyAnalyses{

        private Converter modelConverter;

        private string transportProtocol = "http://";

        private string portNumber = ":5002";

        private string internalAPIAddress = "/api/v1";

        private string serverAddressEndpoint = "";

        public PrivacyAnalyses()
        {
            string serverURL = System.Environment.GetEnvironmentVariable("BACKEND_URL");
            if (string.IsNullOrEmpty(serverURL))
                serverURL = "localhost";
            serverAddressEndpoint = transportProtocol + serverURL +portNumber + internalAPIAddress;
            modelConverter = new Library.RDF.Converter();
        }

        public async Task<string> RunAnalyses(List<Node> nodes, List<Link> links, RDFClient rDFClient)
        {
            var dtoJson = modelConverter.ConverterToRDF(nodes, links,rDFClient);

            using (var client = new HttpClient())
            {
                 HttpResponseMessage response = await client.PostAsync(serverAddressEndpoint, new StringContent(dtoJson, Encoding.UTF8, "application/json")).ConfigureAwait(false);
                 response.EnsureSuccessStatusCode();
                 string returnJson = await response.Content.ReadAsStringAsync();
                 return returnJson;
            }
            return null;
        }





    }
}