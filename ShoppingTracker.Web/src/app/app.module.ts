import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER, OnDestroy } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';
import { OverlayModule } from '@angular/cdk/overlay';

import { AppConfig } from './app.config';
import { ApiModule as PsStoreFavoritesClient, Configuration, ConfigurationParameters } from './api/PsStoreFavoritesClient';

import { AppComponent } from './app.component';
import { FavoritesComponent } from './favorites/favorites.component';
import { SearchComponent } from './search/search.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';

import { HttpClientModule } from '@angular/common/http';
import { DialogComponent } from './dialog/dialog.component';

export function apiConfigFactory(): Configuration {
  const params: ConfigurationParameters = {
    basePath: AppConfig.settings.api.url
  }
  return new Configuration(params);
}

export function createApiConfigFactory() {
  return apiConfigFactory();
}

export function initializeApp(appConfig: AppConfig) {
  return () => appConfig.load();
}

@NgModule({
  declarations: [
    AppComponent,
    FavoritesComponent,
    SearchComponent,
    DialogComponent
  ],
  imports: [
    BrowserModule,
    OverlayModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule,
    MatCardModule,
    MatDialogModule,
    MatSelectModule,
    FormsModule,
    HttpClientModule,
    PsStoreFavoritesClient.forRoot(createApiConfigFactory)
  ],
  exports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatGridListModule,
    MatCardModule,
    MatSelectModule,
    DialogComponent
  ],
  providers: [AppConfig,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [AppConfig], multi: true
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
