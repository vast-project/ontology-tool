import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip'
import { BrowserAnimationsModule  } from '@angular/platform-browser/animations'
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from "@angular/material/form-field";

import { AppComponent } from "./app.component";
import { HomeComponent } from "./home/home.component";
import { HTTP_INTERCEPTORS, HttpClientModule } from "@angular/common/http";
import { AuthInterceptor } from "./auth.interceptor";
import { VastNavComponent } from './vast-nav/vast-nav.component';
import { LoginSuccessComponent } from './login-success/login-success.component';
import { DashboardGraphComponent } from './dashboard-graph/dashboard-graph.component';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { AnnotationsComponent } from './annotations/annotations.component';
import { ConceptsComponent } from './concepts/concepts.component';
import { KeywordsComponent } from './keywords/keywords.component';
import { DesignComponent } from './design/design.component';
import { VisualizeComponent } from './visualize/visualize.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    VastNavComponent,
    LoginSuccessComponent,
    DashboardGraphComponent,
    AnnotationsComponent,
    ConceptsComponent,
    KeywordsComponent,
    DesignComponent,
    VisualizeComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatButtonModule,
    MatMenuModule,
    MatToolbarModule,
    MatIconModule,
    MatCardModule,
    MatSidenavModule,
    MatListModule,
    MatTooltipModule,
    BrowserAnimationsModule,
    PerfectScrollbarModule,
    MatAutocompleteModule,
    RouterModule.forRoot([
      { path: "", component: HomeComponent, pathMatch: "full" },
      { path: "login-success/:token", component: LoginSuccessComponent },
      { path: "annotations", component: AnnotationsComponent },
      { path: "concepts", component: ConceptsComponent },
      { path: "keywords", component: KeywordsComponent },
      { path: "design", component: DesignComponent },
      { path: "visualize", component: VisualizeComponent },
    ])
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
