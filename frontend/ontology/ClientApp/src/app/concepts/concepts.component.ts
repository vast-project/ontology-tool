import { Component, OnInit } from '@angular/core';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { Subject, Observable } from 'rxjs';
import { DataService } from '../data.service';
import { switchMap, debounceTime, tap, map } from 'rxjs/operators';

@Component({
  selector: 'app-concepts',
  templateUrl: './concepts.component.html',
  styleUrls: ['./concepts.component.scss']
})
export class ConceptsComponent implements OnInit {

  public searchKeywords:string = "";
  public searchAnnotations:string = "";

  public selectedKeyIndex:number=0;

  public resultsKeywords: Observable<any>;
  public resultsAnnotations: Observable<any>;

  constructor(private dataService : DataService) { }

  ngOnInit(): void {
    this.resultsKeywords=this.dataService.GetItems("", 1);
    this.resultsAnnotations=this.dataService.GetAnnotations("");
  }

  onKeyUpKey(): void {
    this.resultsKeywords=this.dataService.GetItems(this.searchKeywords);
  }
 
  onKeyUpAnn(): void {
    this.resultsAnnotations=this.dataService.GetAnnotations(this.searchAnnotations, 0, this.selectedKeyIndex);
  }

  onClickKey(itemId: any) {
    this.selectedKeyIndex = itemId;
    this.resultsAnnotations=this.dataService.GetAnnotations(this.searchAnnotations, 0, itemId);
  }

}
