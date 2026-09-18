import { Component, OnInit } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { DataService } from '../data.service';
import { switchMap, debounceTime, tap, map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-keywords',
    templateUrl: './keywords.component.html',
    styleUrls: ['./keywords.component.scss'],
    standalone: false
})
export class KeywordsComponent implements OnInit {
  public searchKeywords: string = "";
  public searchAnnotations: string = "";

  public selectedKeyIndex: number = 0;
  keyword: string | null = null;

  public resultsKeywords: Observable<any>;
  public resultsAnnotations: Observable<any>;

  constructor(private dataService: DataService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.keyword = params.get('keyword');
      console.warn(this.keyword);
      if (this.keyword) {
        const keywordNumber = this.keyword ? parseInt(this.keyword, 10) : -1;
        console.warn("Number:" + keywordNumber);
        if (!isNaN(keywordNumber) && keywordNumber >= 0) {
          console.warn("Fetching data");
          this.selectedKeyIndex = keywordNumber;
          this.resultsKeywords = this.dataService.GetItems("");
          this.resultsAnnotations = this.dataService.GetAnnotations(this.searchAnnotations, 0, this.selectedKeyIndex);
        } else {
          this.resultsKeywords = this.dataService.GetItems("");
          this.resultsAnnotations = this.dataService.GetAnnotations("");
        }
      } else {
        this.resultsKeywords = this.dataService.GetItems("");
        this.resultsAnnotations = this.dataService.GetAnnotations("");
      }
    });
  }

  onKeyUpKey(): void {
    this.resultsKeywords = this.dataService.GetItems(this.searchKeywords);
  }

  onKeyUpAnn(): void {
    this.resultsAnnotations = this.dataService.GetAnnotations(this.searchAnnotations, 0, this.selectedKeyIndex);
  }

  onClickKey(itemId: any) {
    this.selectedKeyIndex = itemId;
    this.resultsAnnotations = this.dataService.GetAnnotations(this.searchAnnotations, 0, itemId);
  }


}
