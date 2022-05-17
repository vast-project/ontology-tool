import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login-success',
  templateUrl: './login-success.component.html',
  styleUrls: ['./login-success.component.scss']
})
export class LoginSuccessComponent implements OnInit {
  private routeState: any = null;
  private providedToken: string | null = null;
  constructor(private _authService: AuthService, private _router: Router, private _route: ActivatedRoute) {

  }

  ngOnInit(): void {
    var token = this._route.snapshot.paramMap?.get("token")?.toString();
    if (token) {
      this._authService.finishLogin(token);
      this._router.navigate(['/'], { replaceUrl: true });
    }
  }

}
