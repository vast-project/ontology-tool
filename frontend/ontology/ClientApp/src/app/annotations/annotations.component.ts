import { Component, OnInit } from '@angular/core';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { Subject, Observable } from 'rxjs';
import { DataService } from '../data.service';
import { switchMap, debounceTime, tap, map } from 'rxjs/operators';


@Component({
  selector: 'app-annotations',
  templateUrl: './annotations.component.html',
  styleUrls: ['./annotations.component.scss']
})
export class AnnotationsComponent implements OnInit {
  public searchCollections:string = "";
  public searchDocuments:string = "";
  public searchAnnotations:string = "";

  public selectedColIndex:number=0;
  public selectedDocIndex:number=0;

  public resultsCollection: Observable<any>;
  public resultsDocuments: Observable<any>;
  public resultsAnnotations: Observable<any>;

  constructor(private dataService : DataService) { }

  ngOnInit(): void {
    this.resultsCollection=this.dataService.GetCollections("");
    this.resultsDocuments=this.dataService.GetDocuments("");
    this.resultsAnnotations=this.dataService.GetAnnotations("");
  }

  onKeyUpCol(): void {
    this.resultsCollection=this.dataService.GetCollections(this.searchCollections);
  }
  onKeyUpDoc(): void {
    this.resultsDocuments=this.dataService.GetDocuments(this.searchDocuments, this.selectedColIndex);
  }
  onKeyUpAnn(): void {
    this.resultsAnnotations=this.dataService.GetAnnotations(this.searchAnnotations, this.selectedDocIndex);
  }

  onClickCol(itemId: any) {
    this.selectedColIndex = itemId;
    this.selectedDocIndex = 0;
    this.resultsDocuments=this.dataService.GetDocuments(this.searchDocuments, itemId);
    this.resultsAnnotations=this.dataService.GetAnnotations(this.searchAnnotations, 0);
  }

  onClickDoc(itemId: any) {
    this.selectedDocIndex = itemId;
    this.resultsAnnotations=this.dataService.GetAnnotations(this.searchAnnotations, itemId);
  }
}
