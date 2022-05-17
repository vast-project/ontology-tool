import { Component, Input, Output, EventEmitter, OnInit } from "@angular/core";
import { AuthService} from '../auth.service';
import { TokenService} from '../token.service';

@Component({
  selector: 'app-vast-nav',
  templateUrl: './vast-nav.component.html',
  styleUrls: ['./vast-nav.component.scss']
})
export class VastNavComponent implements OnInit {
  @Input() isExpanded: boolean = false;
  @Output() toggleMenu = new EventEmitter();

  constructor(private _authService: AuthService, private _tokenService : TokenService) { }

  public routeLinks = [
    { link: "", name: "Dashboard", icon: "dashboard" },
    { link: "annotations", name: "Annotations", icon: "texture" },
    { link: "keywords", name: "Keywords", icon: "gamepad" },
    { link: "concepts", name: "Concepts", icon: "style" },
    { link: "design", name: "Statements", icon: "storage" },
    { link: "visualize", name: "Explore", icon: "device_hub" },
  ];

  ngOnInit(): void {
  }

  public isAuthenticated = () : boolean => {
    return this._authService.isAuthenticated;
  }

  public login = () => {
    this._authService.login();
  }

}
