import { Component, OnInit } from "@angular/core";
import { FavoritesService } from "../api/PsStoreFavoritesClient";
import { ShoppingFavorite, MessageType, Stores } from "../model/models";
import { MatDialog } from "@angular/material/dialog";
import { DialogComponent } from "../dialog/dialog.component";
@Component({
  selector: "app-favorites",
  templateUrl: "./favorites.component.html",
  styleUrls: ["./favorites.component.scss"]
})
export class FavoritesComponent implements OnInit {
  public favorites: Array<ShoppingFavorite> = null;
  public allFavorites: Array<ShoppingFavorite> = null;
  stores = Stores;
  enumStores = [];
  selectedStore = 0;

  constructor(
    private favoriteService: FavoritesService,
    public dialog: MatDialog
  ) {
    this.enumStores = Object.keys(this.stores).filter(f => !isNaN(Number(f)));
    this.selectedStore = 999;
  }

  ngOnInit(): void {
    this.favoriteService.favoritesGet().subscribe(favorites => {
      this.allFavorites = this.favorites = favorites;
    });
  }

  filterSearch() {
    if(this.selectedStore == 999) {
      this.favorites = this.allFavorites
    } else {
      this.favorites = this.allFavorites.filter(f => f.store == this.selectedStore);
    }
  }

  deleteFavorite(favorite: ShoppingFavorite) {
    this.favoriteService
      .favoritesDelete(favorite.rowKey, favorite.partitionKey)
      .subscribe(
        result => {
          if (result != null) {
            this.dialog.open(DialogComponent, {
              data: {
                message: `${favorite.name} eliminato con successo`,
                type: MessageType.success,
                title: "Eliminazione favorito"
              }
            });
            this.ngOnInit();
          } else {
            this.dialog.open(DialogComponent, {
              data: {
                message: "Operazione non completata",
                type: MessageType.error,
                title: "Eliminazione favorito"
              }
            });
          }
        },
        error => {
          this.dialog.open(DialogComponent, {
            data: {
              message: "Errore interno",
              type: MessageType.error,
              title: "Eliminazione favorito"
            }
          });
        }
      );
  }
}
