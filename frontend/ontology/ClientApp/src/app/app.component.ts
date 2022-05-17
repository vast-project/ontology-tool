import { Component, ViewChild } from "@angular/core";
import { MatSidenav } from '@angular/material/sidenav';
import { AuthService} from './auth.service';

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html"
})
export class AppComponent {
  title = "Ontology Design Tool";
  isSideNavExpanded: boolean = true;
  public userAuthenticated = false;

  constructor(private _authService: AuthService){

  }

  ngOnInit(): void {
    
  }

  toggleMenu() {
    this.isSideNavExpanded = !this.isSideNavExpanded;
  }
}
