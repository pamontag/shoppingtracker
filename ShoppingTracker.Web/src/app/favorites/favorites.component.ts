import { Component, OnInit } from "@angular/core";
import { FavoritesService } from "../api/PsStoreFavoritesClient";
import { ShoppingFavorite, MessageType } from "../model/models";
import { MatDialog } from "@angular/material/dialog";
import { DialogComponent } from "../dialog/dialog.component";
@Component({
  selector: "app-favorites",
  templateUrl: "./favorites.component.html",
  styleUrls: ["./favorites.component.scss"]
})
export class FavoritesComponent implements OnInit {
  public favorites: Array<ShoppingFavorite> = null;

  constructor(
    private favoriteService: FavoritesService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.favoriteService.favoritesGet().subscribe(favorites => {
      this.favorites = favorites;
    });
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
