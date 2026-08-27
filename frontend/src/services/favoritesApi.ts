import { request } from "./apiClient";
import type { Favorite } from "../types/Favorite";

export const favoritesApi = {
  getMine: () =>
    request<Favorite[]>("/api/favorites", {
      credentials: "include",
    }),
  toggle: (propertyId: string, isFavorite: boolean) =>
    request<unknown>(`/api/favorites/${propertyId}`, {
      method: isFavorite ? "DELETE" : "POST",
      credentials: "include",
    }),
};
