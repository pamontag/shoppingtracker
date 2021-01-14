import { Component, OnInit } from "@angular/core";
import { PsStoreService, AmazonService } from "../api/PsStoreFavoritesClient";
import { ProductPrice, MessageType, Stores } from "../model/models";
import { MatDialog } from "@angular/material/dialog";
import { DialogComponent } from "../dialog/dialog.component";

@Component({
  selector: "app-search",
  templateUrl: "./search.component.html",
  styleUrls: ["./search.component.scss"]
})
export class SearchComponent implements OnInit {
  public searchText: string = null;
  public products: Array<ProductPrice> = null;
  stores = Stores;
  enumStores = [];
  selectedStore = 0;
  constructor(
    private psstoreService: PsStoreService,
    private amazonService: AmazonService,
    public dialog: MatDialog
  ) {
    this.enumStores = Object.keys(this.stores).filter(f => !isNaN(Number(f)));
    this.enumStores = this.enumStores.filter(f => f != 999);
  }

  ngOnInit(): void {}

  emptySearch() {
    this.products = null;
  }

  searchproducts() {
    switch (+this.selectedStore) {
      case Stores.PSStore: {
        this.psstoreService.psStoreGet(this.searchText).subscribe(products => {
          this.products = products;
        });
        break;
      }
      case Stores.Amazon: {
        this.amazonService.amazonGet(this.searchText).subscribe(products => {
          this.products = products;
        });
        break;
      }
      default: {
        this.dialog.open(DialogComponent, {
          data: {
            message: "Provider non gestito",
            type: MessageType.error,
            title: "Ricerca prodotti"
          }
        });
        break;
      }
    }
  }

  addToFavorites(product: ProductPrice) {
    switch (+this.selectedStore) {
      case Stores.PSStore: {
        this.psstoreService.psStorePost(product).subscribe(
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
        break;
      }
      case Stores.Amazon: {
        this.amazonService.amazonPost(product).subscribe(
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
        break;
      }
      default: {
        break;
      }
    }
  }
}
