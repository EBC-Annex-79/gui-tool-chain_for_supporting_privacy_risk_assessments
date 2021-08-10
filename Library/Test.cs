using Library.RDF;

namespace Library
{
    public class Test
    {
        static RDFClient client;
        public Test()
        {

        }

        public static void Main(string[] args)
        {
            client = new RDFClient(RDFMode.SmartBuilding);
        }   
    }
}
