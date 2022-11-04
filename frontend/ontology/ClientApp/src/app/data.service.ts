import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private headers = new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${this.tokenService.getToken()}`
  });

  private REST_API_SERVER = "https://ontology.vast-project.eu";
  //private REST_API_SERVER = "https://localhost:7164";

  constructor(private httpClient: HttpClient, private tokenService: TokenService) { }

  public GetMainStats() {
    return this.httpClient.get(this.REST_API_SERVER + "/api/item/stats");
  }

  public GetRecent() {
    return this.httpClient.get(this.REST_API_SERVER + "/api/item/recent");
  }

  public GetCollections(search: string) {
    return this.httpClient.get(this.REST_API_SERVER + "/api/annotation/collection?search=" + search);
  }

  public GetDocuments(search: string, colid: number = 0) {
    if (colid > 0) {
      return this.httpClient.get(this.REST_API_SERVER + "/api/annotation/document?search=" + search + "&collection=" + colid);
    } else {
      return this.httpClient.get(this.REST_API_SERVER + "/api/annotation/document?search=" + search);
    }

  }

  public GetAnnotations(search: string, docid: number = 0, keyId: number = 0, colId: number = 0) {
    var url: string = this.REST_API_SERVER + "/api/annotation/item?search=" + search;
    if (docid > 0) {
      url = url + "&document=" + docid;
    }
    if (keyId > 0) {
      url = url + "&keywordConcept=" + keyId;
    }
    if (colId > 0) {
      url = url + "&collection=" + colId;
    }

    return this.httpClient.get(url);
  }

  public GetItems(search: string, itemType: number = 0) {
    var url: string = this.REST_API_SERVER + "/api/item?search=" + search + "&type=" + itemType + "&pageSize=150";

    return this.httpClient.get(url);
  }

  public GetLinkTypes(search: string) {
    var url: string = this.REST_API_SERVER + "/api/statement/rel-types?search=" + search;

    return this.httpClient.get(url);
  }

  public GetLinks(search: string, sourceId: number = 0, targetId: number = 0) {
    var url: string = this.REST_API_SERVER + "/api/statement?search=" + search;

    return this.httpClient.get(url);
  }
  public GetOwnLinks(search: string, sourceId: number = 0, targetId: number = 0) {
    var url: string = this.REST_API_SERVER + "/api/statement/me?search=" + search;

    return this.httpClient.get(url);
  }
  public GetOtherLinks(search: string, sourceId: number = 0, targetId: number = 0, linkTypeId: number = 0) {
    var url: string = this.REST_API_SERVER + "/api/statement/other?search=" + search;
    if (sourceId > 0) {
      url = url + "&sourceId=" + sourceId;
    }
    if (targetId > 0) {
      url = url + "&targetId=" + targetId;
    }
    if (linkTypeId > 0) {
      url = url + "&linkTypeId=" + linkTypeId;
    }

    return this.httpClient.get(url);
  }

  public AddStatement(statement: any) {
    var url: string = this.REST_API_SERVER + "/api/statement";

    return this.httpClient.post(url, statement);
  }

  public Vote(statementId: number, negative: boolean) {
    var url: string = this.REST_API_SERVER + "/api/statement/vote";

    return this.httpClient.post(url, {
      statementId: statementId,
      negative: negative
    });
  }
}
