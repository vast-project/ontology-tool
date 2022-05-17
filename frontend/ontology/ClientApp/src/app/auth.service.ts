import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { TokenService } from "./token.service";

const INTERNAL_LOGIN = "https://" +
  "ontology.vast" +
  "-project.eu/login";
const INTERNAL_LOGOUT = "https://ontology.vast-project.eu/logout";


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _userObject: Object | null = null;
  private _userToken: string | null = null;

  public isAuthenticated: boolean = false;

  constructor(private _tokenService: TokenService) {

  }

  public login = () => {
    return new Promise<void>(function (resolve, reject) {
      window.location.href = INTERNAL_LOGIN;
      resolve();
    });
  }

  public finishLogin = (token: string): Object | null => {
    var decodeToken = (token: string): any => {
      const _decodeToken = (token: any) => {
        try {
          return JSON.parse(atob(token));
        } catch (e) {
          return null;
        }
      };

      return token
        .split('.')
        .map(token => _decodeToken(token))
        .reduce((acc, curr) => {
          if (!!curr) acc = { ...acc, ...curr };
          return acc;
        }, Object.create(null));
    }

    this._userObject = decodeToken(token);
    this._userToken = token;

    this._tokenService.saveToken(this._userToken);
    this.isAuthenticated = true;

    return this._userObject;
  }

  public logout = () => {
    this._tokenService.removeToken();
    this.isAuthenticated = false;
    window.location.href = INTERNAL_LOGOUT;
  }


}
