import { Component, ViewChild } from "@angular/core";
import { MatSidenav } from '@angular/material/sidenav';
import { AuthService } from './auth.service';
import { DataService } from "./data.service";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html"
})
export class AppComponent {
  title = "Ontology Design Tool";
  isSideNavExpanded: boolean = true;
  public userAuthenticated = false;
  public selectedContext;
  dataService: DataService;

  constructor(private _authService: AuthService, private _dataService: DataService) {
    this.selectedContext = _dataService.GetCurrentContext();
    this.dataService = _dataService;
  }

  public changeContext(contextId: number) {
    this.dataService.SetCurrentContext(contextId);
    window.location.reload();
  }

  public compareContexts(p1: number, p2: number): boolean {
    if (p1 && p2) {
        return p1 === p2;
    }
    return false;
}

  ngOnInit(): void {

  }

  toggleMenu() {
    this.isSideNavExpanded = !this.isSideNavExpanded;
  }
}
