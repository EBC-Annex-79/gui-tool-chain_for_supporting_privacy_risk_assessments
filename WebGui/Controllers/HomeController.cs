using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebGui.Models;
using Library.RDF;
using System.Collections;
using Library.PrivacyModel;
using Newtonsoft.Json;
using WebGui.Models.DTO;

namespace WebGui.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private RDFClient rDFClient;

        public ArrayList ContextAndData;
        public SortedDictionary<string,SupportedType> DictContextAndData;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            rDFClient = new RDFClient(RDFMode.SmartBuilding);
            DictContextAndData = new SortedDictionary<string, SupportedType>();
        }

        [HttpPost]
        public string RunAnalysesOfGraph([FromBody] DataObj data)
        {
            List<DataAttribute> dataAttributes = data.dataAttributes;
            List<Models.DTO.Link> modelLinks = data.links;

            Library.PrivacyModel.PrivacyAnalyses privacyAnalyse = new PrivacyAnalyses();

            Dictionary<int, Node> nodes = convertDataAttributesToNodes(dataAttributes);

            List<Library.PrivacyModel.Link> links = convertDTOLinkToLink(modelLinks,nodes);

            var responseMessage = privacyAnalyse.RunAnalyses(nodes.Values.ToList(),links, rDFClient) ;
            string graph = responseMessage.Result.ToString();
            return graph;
        }

        private List<Library.PrivacyModel.Link> convertDTOLinkToLink(List<Models.DTO.Link> modelLinks, Dictionary<int, Node> nodes)
        {
            List<Library.PrivacyModel.Link> links = new List<Library.PrivacyModel.Link>();

            foreach (Models.DTO.Link modelLink in modelLinks)
            {
                Library.PrivacyModel.Link link = new Library.PrivacyModel.Link();
                if (modelLink.right)
                {
                    link.Source = nodes[modelLink.sourceId];
                    link.Target = nodes[modelLink.targetId];
                }
                else
                {
                    link.Source = nodes[modelLink.targetId];
                    link.Target = nodes[modelLink.sourceId];
                }
                links.Add(link);
            }

            return links;
        }

        private Dictionary<int, Node> convertDataAttributesToNodes(List<DataAttribute> dataAttributes){
            LinkedList<SupportedType>  supportedTypes =findSupportedTypes();
            
            Dictionary<int, Node> nodes = new Dictionary<int, Node>();

            foreach (DataAttribute dataAttribute in dataAttributes)
            {
                List<Library.PrivacyModel.Attribute> attributes = new List<Library.PrivacyModel.Attribute>();
                string name = "";

                foreach (Models.DTO.Attribute attribute in dataAttribute.Attribute)
                {
                    if (attribute.DataTypeName == "name")
                        name = attribute.Value;
                    else
                    {
                        Library.PrivacyModel.Attribute nodeAttribute = new Library.PrivacyModel.Attribute(attribute.DataTypeName,attribute.DataType,attribute.Value);
                        attributes.Add(nodeAttribute);
                    }
                }

                string superType = FindSuperTypeColorId(dataAttribute.superType);
                string addressType = DictContextAndData[dataAttribute.Type].Address;
                Node node = new Node(dataAttribute.ID,name,addressType,dataAttribute.superAddress);


                node.Attributes.AddRange(attributes);
                nodes.Add(dataAttribute.ID, node);
            }
            return nodes;
        }

        private LinkedList<SupportedType>  findSupportedTypes(){
            ContextAndData = rDFClient.FindDataTypesAndContexts();
            LinkedList<SupportedType> dataTypes = FindTypesForContextAndData(ContextAndData);
            return dataTypes;
        }       

        [HttpGet]
        public IActionResult Index()
        {
            LinkedList<SupportedType> dataTypes = findSupportedTypes();
            this.ViewBag.DataTypes = dataTypes;
            this.ViewBag.DictContextAndData =  JsonConvert.SerializeObject(DictContextAndData);
            return View();
        }

         public IActionResult Test()
        {
            LinkedList<SupportedType> dataTypes = findSupportedTypes();
            this.ViewBag.DataTypes = dataTypes;
            this.ViewBag.DictContextAndData =  JsonConvert.SerializeObject(DictContextAndData);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private LinkedList<SupportedType> FindTypesForContextAndData(ArrayList contextAndData)
        {
            SortedList<String,SupportedType> supportedTypes = new SortedList<String,SupportedType>();

            foreach (ModelItem item in contextAndData)
            {
                int? SuperTypeColorId = FindSuperTypeColorId(item.Name);
                SupportedType supported = new SupportedType(item.Name, item.Address, SuperTypeColorId, false);
                supported.AddAttributes(item.Attributes);
                
                foreach (ModelItem sub_item in item.SubDataStrucktor)
                {
                    _subContextAndData(sub_item, supported, SuperTypeColorId, item.Address );
                }
                supportedTypes.Add(supported.Name,supported);
                DictContextAndData.Add(supported.Name, supported);
            }
            LinkedList<SupportedType> returnSupportedTypes = new LinkedList<SupportedType>(supportedTypes.Values.ToList());
            return returnSupportedTypes;
        }

        private void _subContextAndData(ModelItem ContextAndData, SupportedType parent, int? superTypeColorId, string superTypeAddress)
        {
            int? SuperTypeColorId = superTypeColorId;
            bool clickable = true;
            if (SuperTypeColorId == null){
                SuperTypeColorId = FindSuperTypeColorId(ContextAndData.Name);
                if (SuperTypeColorId != null)
                    clickable = false;
            }

            SupportedType supported = new SupportedType(ContextAndData.Name, ContextAndData.Address, SuperTypeColorId, clickable,superTypeAddress);
            supported.AddAttributes(parent.Attributes);
            supported.AddAttributes(ContextAndData.Attributes);
            
            foreach (ModelItem sub_item in ContextAndData.SubDataStrucktor)
            {
                if (SuperTypeColorId != 8) //if this is a data item
                {
                    _subContextAndData(sub_item, supported, SuperTypeColorId,ContextAndData.Address);
                }
                else
                {
                    _subContextAndData(sub_item, supported, SuperTypeColorId,superTypeAddress);//ContextAndData.Address);
                }
            }
            parent.AddSubSupportedType(supported);
            DictContextAndData.Add(supported.Name, supported);
        }

        private int? FindSuperTypeColorId(string name)
        {
            switch (name)
            {
                case "Context":
                    return 8;
                case "TimeSeries":
                    return 1;
                case "External":
                    return 2;
                case "Graph":
                    return 3;
                case "Metadata":
                    return 4;
                case "Transformation":
                    return 6;
                case "PrivacyRisk":
                    return 7;
                case "PrivacyAttack":
                    return 0;
                default:
                    return null;
            }
        }

        private string FindSuperTypeColorId(int number)
        {
            switch (number)
            {
                case 8:
                    return "Context";
                case 1:
                    return "TimeSeries";
                case 2:
                    return "External";
                case 3:
                    return "Graph";
                case 4:
                    return "Metadata";
                case 6:
                    return "Transformation";
                case 5:
                    return "PrivacyRisk";
                case 0:
                    return "PrivacyAttack";
                default:
                    return null;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class DataObj{
        public List<Models.DTO.Link> links { get; set; }

        public List<DataAttribute> dataAttributes { get; set; }
    }
    
}
