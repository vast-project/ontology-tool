import { Component, OnInit } from "@angular/core";
import { DashboardGraphComponent } from '../dashboard-graph/dashboard-graph.component';
import { DataService } from '../data.service';

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
})

export class HomeComponent implements OnInit {
  public dataTiles = [{ title: "", subtitle: "", data: [""] }];
  public recentlyAdded = [{ description: "", collection: "", document: "" }];

  constructor(private dataService: DataService) { }

  ngOnInit(): void {
    this.dataService.GetMainStats().subscribe((data: any) => {
      this.dataTiles = data;
    });

  }


}
