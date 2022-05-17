import { Component, OnInit } from '@angular/core';
import { debounceTime, tap, switchMap, finalize, distinctUntilChanged, filter } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { DataService } from '../data.service'

@Component({
  selector: 'app-design',
  templateUrl: './design.component.html',
  styleUrls: ['./design.component.scss']
})
export class DesignComponent implements OnInit {

  searchTermsCtrl = new FormControl();
  filteredTerms: any;
  selectedTerm: any = "";

  searchLinksCtrl = new FormControl();
  filteredLinks: any;
  selectedLink: any = "";

  searchConceptsCtrl = new FormControl();
  filteredConcepts: any;
  selectedConcept: any = "";

  myStatements: any;
  otherStatements: any;

  minLengthTerm = 0;

  constructor(
    private dataService: DataService
  ) { }

  displayWith(value: any) {
    return value?.name;
  }

  displayLinkWith(value: any) {
    if (value?.name && value?.ontologyId) {
      return value?.name + " (" + value?.ontologyId + ")";
    } else {
      return "";
    }
  }

  onSelectedKeyword() {
    this.selectedTerm = this.selectedTerm;
  }

  clearSelectionKeyword() {
    this.selectedTerm = "";
    this.filteredTerms = [];
  }

  onSelectedLink() {
    this.selectedLink = this.selectedLink;
  }

  clearSelectionLink() {
    this.selectedLink = "";
    this.filteredLinks = [];
  }

  onSelectedConcept() {
    this.selectedConcept = this.selectedConcept;
  }

  clearSelectionConcept() {
    this.selectedConcept = "";
    this.filteredConcepts = [];
  }

  private loadStatements() {
    this.dataService.GetOwnLinks("").subscribe(data => {
      this.myStatements = data;
    });
    this.dataService.GetOtherLinks("").subscribe(data => {
      this.otherStatements = data;
    });

  }

  addStatement() {
    var sourceName = this.selectedConcept?.name;
    if (!sourceName) {
      sourceName = this.selectedConcept;
    }

    this.dataService.AddStatement({
      sourceId: this.selectedTerm?.id,
      targetId: this.selectedConcept?.id,
      targetName: sourceName,
      relationshipId: this.selectedLink?.id,
      authorId: "1"
    }).subscribe(data => {
      this.clearSelectionConcept();
      this.clearSelectionKeyword();
      this.clearSelectionLink();
      this.loadStatements();
    });

  }

  ngOnInit() {
    this.searchTermsCtrl.valueChanges
      .pipe(
        filter(res => {
          return res !== null && res.length >= this.minLengthTerm
        }),
        distinctUntilChanged(),
        debounceTime(500),
        tap(() => {
          this.filteredTerms = [];
        }),
        switchMap(value => this.dataService.GetItems(value, -1)
        )
      )
      .subscribe((data: any) => {

        this.filteredTerms = data;
      });

    this.searchLinksCtrl.valueChanges
      .pipe(
        filter(res => {
          return res !== null && res.length >= this.minLengthTerm
        }),
        distinctUntilChanged(),
        debounceTime(500),
        tap(() => {
          this.filteredLinks = [];
        }),
        switchMap(value => this.dataService.GetLinkTypes(value)
        )
      )
      .subscribe((data: any) => {

        this.filteredLinks = data;
      });

    this.searchConceptsCtrl.valueChanges
      .pipe(
        filter(res => {
          return res !== null && res.length >= this.minLengthTerm
        }),
        distinctUntilChanged(),
        debounceTime(500),
        tap(() => {
          this.filteredConcepts = [];
        }),
        switchMap(value => this.dataService.GetItems(value, 1)
        )
      )
      .subscribe((data: any) => {

        this.filteredConcepts = data;
      });

    this.loadStatements();
  }
}
