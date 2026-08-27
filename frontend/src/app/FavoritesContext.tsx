/* eslint-disable react-hooks/set-state-in-effect, react-refresh/only-export-components */
import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";
import { useAuth } from "./AuthContext";
import { favoritesApi } from "../services/favoritesApi";
import type { Favorite } from "../types/Favorite";

type FavoritesContextValue = {
  favoriteIds: string[];
  favorites: Favorite[];
  isLoading: boolean;
  error: string | null;
  toggleFavorite: (propertyId: string) => Promise<void>;
  removeFavorite: (propertyId: string) => Promise<void>;
};

const FavoritesContext = createContext<FavoritesContextValue | undefined>(
  undefined,
);

export const FavoritesProvider = ({ children }: { children: ReactNode }) => {
  const { user } = useAuth();
  const [favoriteIds, setFavoriteIds] = useState<string[]>([]);
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user?.isAuthenticated) {
      setFavoriteIds([]);
      setFavorites([]);
      return;
    }

    setIsLoading(true);
    void favoritesApi
      .getMine()
      .then((loadedFavorites) => {
        setFavorites(loadedFavorites);
        setFavoriteIds(
          loadedFavorites.map((favorite) => favorite.property.id),
        );
      })
      .catch(() => setError("No se pudieron cargar tus favoritos."))
      .finally(() => setIsLoading(false));
  }, [user]);

  const toggleFavorite = async (propertyId: string) => {
    const isFavorite = favoriteIds.includes(propertyId);
    try {
      setError(null);
      await favoritesApi.toggle(propertyId, isFavorite);
      setFavoriteIds((current) =>
        isFavorite
          ? current.filter((id) => id !== propertyId)
          : [...current, propertyId],
      );
      setFavorites((current) =>
        isFavorite
          ? current.filter((favorite) => favorite.property.id !== propertyId)
          : current,
      );
    } catch {
      setError("No se pudo actualizar el favorito.");
    }
  };

  const removeFavorite = async (propertyId: string) => {
    if (!favoriteIds.includes(propertyId)) {
      return;
    }
    await toggleFavorite(propertyId);
  };

  return (
    <FavoritesContext.Provider
      value={{
        favoriteIds,
        favorites,
        isLoading,
        error,
        toggleFavorite,
        removeFavorite,
      }}
    >
      {children}
    </FavoritesContext.Provider>
  );
};

export const useFavorites = () => {
  const context = useContext(FavoritesContext);
  if (!context) {
    throw new Error("useFavorites must be used inside FavoritesProvider.");
  }
  return context;
};
