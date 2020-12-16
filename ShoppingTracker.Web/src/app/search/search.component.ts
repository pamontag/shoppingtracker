import { Component, OnInit } from "@angular/core";
import { PsStoreService } from "../api/PsStoreFavoritesClient";
import { GamePrice, MessageType } from "../model/models";
import { MatDialog } from "@angular/material/dialog";
import { DialogComponent } from "../dialog/dialog.component";

@Component({
  selector: "app-search",
  templateUrl: "./search.component.html",
  styleUrls: ["./search.component.scss"]
})
export class SearchComponent implements OnInit {
  public searchText: string = null;
  public games: Array<GamePrice> = null;

  constructor(
    private psstoreService: PsStoreService,
    public dialog: MatDialog
  ) {}

  ngOnInit(): void {}

  searchGames() {
    this.psstoreService.psStoreGet(this.searchText).subscribe(games => {
      this.games = games;
    });
  }

  addToFavorites(game: GamePrice) {
    this.psstoreService.psStorePost(game).subscribe(
      favorite => {
        if (favorite != null) {
          this.dialog.open(DialogComponent, {
            data: {
              message: `${favorite.name} aggiunto con successo`,
              type: MessageType.success,
              title: "Aggiunta favorito"
            }
          });
        } else {
          this.dialog.open(DialogComponent, {
            data: {
              message: "Operazione non completata",
              type: MessageType.error,
              title: "Aggiunta favorito"
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
