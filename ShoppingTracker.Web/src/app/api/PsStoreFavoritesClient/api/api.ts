export * from './amazon.service';
import { AmazonService } from './amazon.service';
export * from './favorites.service';
import { FavoritesService } from './favorites.service';
export * from './psStore.service';
import { PsStoreService } from './psStore.service';
export const APIS = [AmazonService, FavoritesService, PsStoreService];
