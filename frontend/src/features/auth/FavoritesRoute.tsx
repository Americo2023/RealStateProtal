import { useEffect, useState } from "react";
import { apiUrl, authApi } from "../../services/apiClient";
import { PageMessage } from "../../components/common/SiteChrome";
import { FavoritesPortal } from "../favorites/FavoritesPortal";
import type { AuthUser } from "../../types/api";

export const FavoritesRoute = () => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    void authApi
      .getCurrentUser()
      .then(setUser)
      .catch(() => setUser(null))
      .finally(() => setIsLoading(false));
  }, []);

  useEffect(() => {
    if (!isLoading && !user) {
      window.location.assign(`${apiUrl}/auth/login?returnUrl=/favorites`);
    }
  }, [isLoading, user]);

  if (isLoading || !user) {
    return (
      <PageMessage
        title={isLoading ? "Comprobando sesión" : "Redirigiendo al login"}
        message="Necesitas iniciar sesión para ver tus favoritos."
      />
    );
  }

  return <FavoritesPortal user={user} />;
};