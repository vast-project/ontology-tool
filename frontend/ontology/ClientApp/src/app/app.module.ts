import { BrowserModule } from "@angular/platform-browser";
import { NgModule, APP_ID } from "@angular/core";
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
import { MatSelectModule } from "@angular/material/select";
import { MatSnackBarModule } from "@angular/material/snack-bar";


import { AppComponent } from "./app.component";
import { HomeComponent } from "./home/home.component";
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { AuthInterceptor } from "./auth.interceptor";
import { VastNavComponent } from './vast-nav/vast-nav.component';
import { LoginSuccessComponent } from './login-success/login-success.component';
import { DashboardGraphComponent } from './dashboard-graph/dashboard-graph.component';

import { NgScrollbarModule } from 'ngx-scrollbar';
import { AnnotationsComponent } from './annotations/annotations.component';
import { ConceptsComponent } from './concepts/concepts.component';
import { KeywordsComponent } from './keywords/keywords.component';
import { DesignComponent } from './design/design.component';
import { VisualizeComponent } from './visualize/visualize.component';
import { VoteComponent } from './vote/vote.component';

@NgModule({ declarations: [
        AppComponent,
        HomeComponent,
        VastNavComponent,
        LoginSuccessComponent,
        DashboardGraphComponent,
        AnnotationsComponent,
        ConceptsComponent,
        KeywordsComponent,
        DesignComponent,
        VisualizeComponent,
        VoteComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatButtonModule,
        MatSnackBarModule,
        MatMenuModule,
        MatToolbarModule,
        MatIconModule,
        MatCardModule,
        MatSidenavModule,
        MatSelectModule,
        MatListModule,
        MatTooltipModule,
        BrowserAnimationsModule,
        NgScrollbarModule,
        MatAutocompleteModule,
        RouterModule.forRoot([
            { path: "", component: HomeComponent, pathMatch: "full" },
            { path: "login-success/:token", component: LoginSuccessComponent },
            { path: "annotations", component: AnnotationsComponent },
            { path: "concepts", component: ConceptsComponent },
            { path: "keywords", component: KeywordsComponent },
            { path: "keywords/:keyword", component: KeywordsComponent },
            //{ path: "design", component: DesignComponent },
            //{ path: "vote", component: VoteComponent },
            //{ path: "visualize", component: VisualizeComponent },
        ])], providers: [
        // BrowserModule.withServerTransition() was removed in Angular 18; the app id it
        // configured is now supplied through the APP_ID token.
        { provide: APP_ID, useValue: 'ng-cli-universal' },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule { }
