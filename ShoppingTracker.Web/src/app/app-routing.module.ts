import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FavoritesComponent } from './favorites/favorites.component';
import { SearchComponent } from './search/search.component';


const routes: Routes = [
  { path: '', redirectTo: '/favorites', pathMatch: 'full' },
  { path: 'favorites', component: FavoritesComponent },
  { path: 'search', component: SearchComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes,
      { enableTracing: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
