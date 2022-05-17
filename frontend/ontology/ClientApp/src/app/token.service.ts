import { Injectable } from "@angular/core";
import { HttpClientModule } from "@angular/common/http";

const ACCESS_TOKEN = "vast_access_token";
const REFRESH_TOKEN = "vast_refresh_token";

@Injectable({
  providedIn: "root"
})
export class TokenService {

  constructor() { }

  getToken(): string {
    return localStorage.getItem(ACCESS_TOKEN) || "";
  }

  getRefreshToken(): string {
    return localStorage.getItem(REFRESH_TOKEN) || "";
  }

  saveToken(token : string): void {
    localStorage.setItem(ACCESS_TOKEN, token);
  }

  saveRefreshToken(refreshToken : string): void {
    localStorage.setItem(REFRESH_TOKEN, refreshToken);
  }

  removeToken(): void {
    localStorage.removeItem(ACCESS_TOKEN);
  }

  removeRefreshToken(): void {
    localStorage.removeItem(REFRESH_TOKEN);
  }
}
