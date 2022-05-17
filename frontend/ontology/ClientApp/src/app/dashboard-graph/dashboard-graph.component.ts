import { Component, OnInit } from '@angular/core';
import { EventEmitter } from '@angular/core';
import * as d3 from 'd3';
import { forceSimulation, SimulationLinkDatum, SimulationNodeDatum } from 'd3-force'


class Node implements SimulationNodeDatum {
  public x?: number;
  public y?: number;
  constructor (public id: number) {}
}

class Link implements SimulationLinkDatum<Node> {
  constructor (public source: Node, public target: Node)  {}
}

@Component({
  selector: 'app-dashboard-graph',
  templateUrl: './dashboard-graph.component.html',
  styleUrls: ['./dashboard-graph.component.scss']
})
export class DashboardGraphComponent implements OnInit {
  ngOnInit(): void {
  }
  
}
