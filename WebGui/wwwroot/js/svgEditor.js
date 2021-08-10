// set up SVG for D3
//var activeClass = localStorage.getItem("activeClass");
//Source: http://bl.ocks.org/rkirsling/5001347


const width = d3.select(".page-header").node().getBoundingClientRect().width;
const height = window.innerHeight - 100;
var colors = d3.scaleOrdinal(d3.schemeSet1).domain([0,1,2,3,4,5,6,7,8]); 

const svg = d3.select('.board')
  .append('svg')
  // .on('contextmenu', () => { d3.event.preventDefault(); })
  .attr('width', width)
  .attr('height', height);

// set up initial nodes and links
//  - nodes are known by 'id', not by index in array.
//  - reflexive edges are indicated on the node (as a bold black circle).
//  - links are always source < target; edge directions are set by 'left' and 'right'.
const nodes = [
  // { id: 0, reflexive: false , label: "0", type: 0},
  //   { id: 1, reflexive: false, label: "1", type: 0 },
  //   { id: 2, reflexive: false, label: "2", type: 0 }
];

const attributes = []

let lastNodeId = 0;
const links = [
  // { source: nodes[0], target: nodes[1], left: false, right: true},
  // { source: nodes[1], target: nodes[2], left: false, right: true}
];

// init D3 force layout
const force = d3.forceSimulation()
  .force('link', d3.forceLink().id((d) => d.id).distance(150))
  .force('charge', d3.forceManyBody().strength(-500))
  .force('x', d3.forceX(width / 2))
  .force('y', d3.forceY(height / 2))
  .on('tick', tick);

// init D3 drag support
const drag = d3.drag()
  // Mac Firefox doesn't distinguish between left/right click when Ctrl is held... 
  .filter(() => d3.event.button === 0 || d3.event.button === 2)
  .on('start', (d) => {
    if (!d3.event.active) force.alphaTarget(0.3).restart();

    d.fx = d.x;
    d.fy = d.y;
  })
  .on('drag', (d) => {
    d.fx = d3.event.x;
    d.fy = d3.event.y;
  })
  .on('end', (d) => {
    if (!d3.event.active) force.alphaTarget(0);

    d.fx = null;
    d.fy = null;
  });

// define arrow markers for graph links
svg.append('svg:defs').append('svg:marker')
  .attr('id', 'end-arrow')
  .attr('viewBox', '0 -5 10 10')
  .attr('refX', 6)
  .attr('markerWidth', 3)
  .attr('markerHeight', 3)
  .attr('orient', 'auto')
  .append('svg:path')
  .attr('d', 'M0,-5L10,0L0,5')
  .attr('fill', '#000');

svg.append('svg:defs').append('svg:marker')
  .attr('id', 'start-arrow')
  .attr('viewBox', '0 -5 10 10')
  .attr('refX', 4)
  .attr('markerWidth', 3)
  .attr('markerHeight', 3)
  .attr('orient', 'auto')
  .append('svg:path')
  .attr('d', 'M10,-5L0,0L10,5')
  .attr('fill', '#000');

// line displayed when dragging new nodes
const dragLine = svg.append('svg:path')
  .attr('class', 'link dragline hidden')
  .attr('d', 'M0,0L0,0');

// handles to link and node element groups
let path = svg.append('svg:g').selectAll('path');
let circle = svg.append('svg:g').selectAll('g');

// mouse event vars
let selectedNode = null;
let selectedLink = null;
let mousedownLink = null;
let mousedownNode = null;
let mouseupNode = null;

//reset mouse click
function resetMouseVars() {
  mousedownNode = null;
  mouseupNode = null;
  mousedownLink = null;
}

// update force layout (called automatically each iteration)
function tick() {
  // draw directed edges with proper padding from node centers
  path.attr('d', (d) => {
    const deltaX = d.target.x - d.source.x;
    const deltaY = d.target.y - d.source.y;
    const dist = Math.sqrt(deltaX * deltaX + deltaY * deltaY);
    const normX = deltaX / dist;
    const normY = deltaY / dist;
    const sourcePadding = d.left ? 17 : 12;
    const targetPadding = d.right ? 17 : 12;
    const sourceX = d.source.x + (sourcePadding * normX);
    const sourceY = d.source.y + (sourcePadding * normY);
    const targetX = d.target.x - (targetPadding * normX);
    const targetY = d.target.y - (targetPadding * normY);

    return `M${sourceX},${sourceY}L${targetX},${targetY}`;
  });

  circle.attr('transform', (d) => `translate(${d.x},${d.y})`);
}

// update graph (called when needed)
function restart() {
  

  // path (link) group
  path = path.data(links);

  // update existing links
  path.classed('selected', (d) => d === selectedLink)
    .style('marker-start', (d) => d.left ? 'url(#start-arrow)' : '')
    .style('marker-end', (d) => d.right ? 'url(#end-arrow)' : '');

  // remove old links
  path.exit().remove();

  // add new links
  path = path.enter().append('svg:path')
    .attr('class', 'link')
    .classed('selected', (d) => d === selectedLink)
    .style('marker-start', (d) => d.left ? 'url(#start-arrow)' : '')
    .style('marker-end', (d) => d.right ? 'url(#end-arrow)' : '')
    .on('mousedown', (d) => {
      if (d3.event.ctrlKey) return;

      // select link
      mousedownLink = d;
      selectedLink = (mousedownLink === selectedLink) ? null : mousedownLink;
      selectedNode = null;
      restart();
    })
    .merge(path);

  // circle (node) group
  // NB: the function arg is crucial here! nodes are known by id, not by index!
  circle = circle.data(nodes, (d) => d.id);
  // update existing nodes (reflexive & selected visual states)
  circle.selectAll('circle')
    .style('fill', (d) => (d === selectedNode) ? d3.rgb(colors(d.type)).brighter().toString() : colors(d.type))
    .classed('reflexive', (d) => d.reflexive);


  circle.select('text')
    .text((d) => d.label);

  // remove old nodes
  circle.exit().remove();

  // add new nodes
  const g = circle.enter().append('svg:g');

  g.append('svg:circle')
    .attr('class', 'node')
    .attr('r', 12)
    .style('fill', (d) => (d === selectedNode) ? d3.rgb(colors(d.type)).brighter().toString() : colors(d.type))
    .style('stroke', (d) => d3.rgb(colors(d.type)).darker().toString())
    .classed('reflexive', (d) => d.reflexive)
    .on('mouseover', function (d) {
      if (!mousedownNode || d === mousedownNode) return;
      // enlarge target node
      d3.select(this).attr('transform', 'scale(1.1)');
    })
    .on('mouseout', function (d) {
      if (!mousedownNode || d === mousedownNode) return;
      // unenlarge target node
      d3.select(this).attr('transform', '');
    })
    .on('mousedown', (d) => {
      if (d3.event.ctrlKey) return;

      // select node
      mousedownNode = d;
      selectedNode = (mousedownNode === selectedNode) ? null : mousedownNode;
      selectedLink = null;

      // reposition drag line
      dragLine
        .style('marker-end', 'url(#end-arrow)')
        .classed('hidden', false)
        .attr('d', `M${mousedownNode.x},${mousedownNode.y}L${mousedownNode.x},${mousedownNode.y}`);

      restart();
    })
    .on('mouseup', function (d) {
      if (!mousedownNode) return;

      // needed by FF
      dragLine
        .classed('hidden', true)
        .style('marker-end', '');

      // check for drag-to-self
      mouseupNode = d;
      if (mouseupNode === mousedownNode) {
        resetMouseVars();
        return;
      }

      // unenlarge target node
      d3.select(this).attr('transform', '');

      // add link to graph (update if exists)
      // NB: links are strictly source < target; arrows separately specified by booleans
      const isRight = mousedownNode.id < mouseupNode.id;
      const source = isRight ? mousedownNode : mouseupNode;
      const target = isRight ? mouseupNode : mousedownNode;

      const link = links.filter((l) => l.source === source && l.target === target)[0];
      if (link) {
        //Only one link direction
        link[isRight ? 'right' : 'left'] = true;
        link[!isRight ? 'right' : 'left'] = false;
      } else {
        links.push({
          source,
          target,
          left: !isRight,
          right: isRight,
          sourceId : source.id,
          targetId : target.id
        });
      }

      // select new link
      selectedLink = link;
      selectedNode = null;
      restart();
    });

  // show node IDs
  g.append('svg:text')
    .attr('x', 0)
    .attr('y', 4)
    .attr('class', 'id')
    .text((d) => d.label);

  circle = g.merge(circle);

  // set the graph in motion
  force
    .nodes(nodes)
    .force('link').links(links);

  force.alphaTarget(0.3).restart();
}

function mousedown() {
  // because :active only works in WebKit?
  svg.classed('active', true);

  if (d3.event.ctrlKey || mousedownNode || mousedownLink) return;

  //if there is selected an type to add to the graph
  if (activeClass != null) {
    // insert new node at point
    const point = d3.mouse(this);

    //add the new node
    var lastNodeId_temp = ++lastNodeId;
    const node = {
      id: lastNodeId_temp,
      reflexive: false,
      x: point[0],
      y: point[1],
      label: activeSupportedType.Name + "_" + lastNodeId_temp,
      type: activeSupportedType.SuperTypeColorId
    };
    // let attribute = JSON.parse(JSON.stringify("{"+activeSupportedType.Attributes+"}"));
    let attribute = JSON.parse('{"attribute" :' + JSON.stringify(activeSupportedType.Attributes)+'}');
    let attributeName = attribute["attribute"].find(sub => sub.DataTypeName === "name");
    attributeName["Value"] = activeSupportedType.Name + "_" + lastNodeId_temp;

    attribute["id"]= lastNodeId_temp;
    attribute["type"]= activeSupportedType.Name;
    attribute["superType"]= activeSupportedType.SuperTypeColorId;
    attribute["SuperAddress"]= activeSupportedType.SuperAddress;
    // console.log(activeSupportedType);

    nodes.push(node);
    attributes.push(attribute);
  }

  restart();
}

function mousemove() {
  if (!mousedownNode) return;

  // update drag line
  dragLine.attr('d', `M${mousedownNode.x},${mousedownNode.y}L${d3.mouse(this)[0]},${d3.mouse(this)[1]}`);
}

function mouseup() {
  if (mousedownNode) {
    // hide drag line
    dragLine
      .classed('hidden', true)
      .style('marker-end', '');
  }

  // because :active only works in WebKit?
  svg.classed('active', false);

  // clear mouse event vars
  resetMouseVars();
}

function spliceLinksForNode(node) {
  const toSplice = links.filter((l) => l.source === node || l.target === node);
  for (const l of toSplice) {
    links.splice(links.indexOf(l), 1);
  }
}

function spliceDataNode(node) {
  let attributeNode = attributes.find(sub => sub.id === node.id);
  if (attributeNode !== undefined) {
    attributes.splice(attributes.indexOf(attributeNode), 1);
  }
}

// only respond once per keydown
let lastKeyDown = -1;

function keydown() {
  // d3.event.preventDefault();

  if (lastKeyDown !== -1) return;
  lastKeyDown = d3.event.keyCode;

  // ctrl
  if (d3.event.keyCode === 17) {
    circle.call(drag);
    svg.classed('ctrl', true);
    return;
  }

  if (!selectedNode && !selectedLink) return;

  switch (d3.event.keyCode) {
    case 8: // backspace
    case 46: // delete
      var inputFields = document.getElementsByClassName("form-control");
      activeElement = false;
      for (let index = 0; index < inputFields.length; index++) {
        if (document.activeElement === inputFields[index]) {
          activeElement = true;
          break;
        }
      }
      if (!activeElement) {
        if (selectedNode) {
          nodes.splice(nodes.indexOf(selectedNode), 1);
          spliceDataNode(selectedNode);
          spliceLinksForNode(selectedNode);
          document.getElementById('dataProperties').innerHTML = "";
        } else if (selectedLink) {
          links.splice(links.indexOf(selectedLink), 1);
        }
        selectedLink = null;
        selectedNode = null;
        restart();
        break;
      }
      // case 66: // B
      //   if (selectedLink) {
      //     // set link direction to both left and right
      //     // selectedLink.left = true;
      //     // selectedLink.right = true;
      //   }
      //   restart();
      //   break;
      case 76: // L
        if (selectedLink) {
          // set link direction to left only
          selectedLink.left = true;
          selectedLink.right = false;
        }
        restart();
        break;
      case 82: // R
        if (selectedNode) {
          // toggle node reflexivity
          selectedNode.reflexive = !selectedNode.reflexive;
        } else if (selectedLink) {
          // set link direction to right only
          selectedLink.left = false;
          selectedLink.right = true;
        }
        restart();
        break;
      case 13: //character return
        savePropertiesForDataNode();
  }
}

function keyup() {
  lastKeyDown = -1;

  // ctrl
  if (d3.event.keyCode === 17) {
    circle.on('.drag', null);
    svg.classed('ctrl', false);
  }
}
let activeDataNode = null;

function onclick() {
  if (selectedNode !== null) {
    for (const attribute of attributes) {
      if (attribute.id === selectedNode.id) {
        if (activeDataNode === attribute) {
          return;
        }
        activeDataNode = attribute;
        break;
      }
    }
    createPropertiesForDataNode(activeDataNode);
  } else {
    activeDataNode = null;
    document.getElementById('dataProperties').innerHTML = "";
  }
}

function createPropertiesForDataNode(dataNode) {
  document.getElementById('dataProperties').innerHTML = "";
  dataNode["attribute"].forEach(element => {
    if (element.hasOwnProperty('DataType') && element.hasOwnProperty('DataTypeName') && element.hasOwnProperty('Value')) {

      const div = document.createElement('div');
      div.className = 'form-group';

      let value = element['Value'];
      if (value === null) {
        value = "";
      }

      div.innerHTML = `<label for="` + element['DataTypeName'] + `">` + element['DataTypeName'] + `</label>
        <input type="text" class="form-control" value="` + value + `" id="` + element['DataTypeName'] + `" placeholder="` + element['DataTypeName'] + `">
      `;
      document.getElementById('dataProperties').appendChild(div);
    }
    else if(element.hasOwnProperty('type'))
    {
      const div = document.createElement('div');
      div.className = 'form-group';
      
      div.innerHTML = `<label for="` + element['type'] + `">Node Type: </label>
      <label for="` + element['type'] + `">`+element['type']+`</label>`
    document.getElementById('dataProperties').appendChild(div);
    }
  });
  const div = document.createElement('div');
  div.innerHTML = '<button type="button" onClick="savePropertiesForDataNode()" class="btn btn-primary">Save</button>'
  document.getElementById('dataProperties').appendChild(div);
}

function savePropertiesForDataNode() {
  if (activeDataNode !== null) {
    var inputFields = document.getElementsByClassName("form-control");
    for (let index = 0; index < inputFields.length; index++) {
      const element = inputFields[index];
      let attributeName = activeDataNode["attribute"].find(sub => sub.DataTypeName === element.id);
      attributeName["Value"] = element.value;
      if (element.id === "name")
      {
        selectedNode["label"] = element.value;
      }
    }
    restart();
  }
}

// app starts here
svg.on('mousedown', mousedown)
  .on('mousemove', mousemove)
  .on('click', onclick)
  .on('mouseup', mouseup);
d3.select(window)
  .on('keydown', keydown)
  .on('keyup', keyup);
restart();