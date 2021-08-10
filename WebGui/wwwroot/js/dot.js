var svgElement = null;

function redrawDotFile()
{

    var engine = document.querySelector('input[name="engine"]:checked').value;

    if ("graph" in analysesResults )
    {
        drawDotFile(analysesResults["graph"],engine)
    }
}

function storeGraph()
{
    var engine = document.querySelector('input[name="engine"]:checked').value;

    if ("graph" in analysesResults && svgElement != null)
    {
        var viz = new Viz();
  
        viz.renderImageElement(analysesResults["graph"], {
            engine: engine,
            images: [
                { path: "test.png", width: 1500, height: 5000 }
            ],
            scale : 10, //default 1
            quality : 1 //default 1
        })
        .then(function(element) {
            var link = document.createElement("a");

            document.body.appendChild(link); // for Firefox
        
            link.setAttribute("href", element["src"]);
            link.setAttribute("download", "fileName.png");
            link.click();
        })
    }
}

function storePrivacyReport()
{
    if ("privacy_report" in analysesResults)
    {
        let dataStr = JSON.stringify(analysesResults["privacy_report"]);
        let dataUri = 'data:application/json;charset=utf-8,'+ encodeURIComponent(dataStr);
    
        let exportFileDefaultName = 'privacy_report.json';
    
        let linkElement = document.createElement('a');
        linkElement.setAttribute('href', dataUri);
        linkElement.setAttribute('download', exportFileDefaultName);
        linkElement.click();
    }
}

function drawDotFile(digraph, engine="circo") {
    var viz = new Viz();
    var viewBoard = document.getElementById('ViewBoard');

    viz.renderSVGElement(digraph, 
        {
            engine: engine,
            // images: [
            //     { path: "test.png", width: '400px', height: '300px' }
            //   ]
        }).then(function (element) {
            svgElement = element;
            viewBoard.innerHTML = "";
            viewBoard.appendChild(element);
            panZoom = svgPanZoom(element, {
                zoomEnabled: true,
                controlIconsEnabled: true,
                fit: true,
                center: true,
                minZoom: 0.1
                });
                element.addEventListener('paneresize', function(e) {
                panZoom.resize();
                }, false);
                window.addEventListener('resize', function(e) {
                panZoom.resize();
                });
        })
        .catch(error => {
            // Create a new Viz instance (@see Caveats page for more info)
            viz = new Viz();

            // Possibly display the error
            console.error(error);
        });
}

function toggleBoards()
{
    const viewBoard = document.getElementById('ViewBoard');
    viewBoard.classList.toggle("hidden");

    const viewProperties = document.getElementById('viewProperties');
    viewProperties.classList.toggle("hidden");

    const ViewAnalysesBtn = document.getElementById('ViewAnalysesBtn');
    ViewAnalysesBtn.classList.toggle("hidden");

    const editorAnalysesBtn = document.getElementById('EditorAnalysesBtn');
    editorAnalysesBtn.classList.toggle("hidden");
    
    const editorProperties = document.getElementById('EditorProperties');
    editorProperties.classList.toggle("hidden");

    const editor = document.getElementById('editor');
    editor.classList.toggle("hidden");
}