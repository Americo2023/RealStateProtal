import { useEffect } from "react";
import { apiUrl } from "../../services/apiClient";
import { useAuth } from "../../app/AuthContext";
import { PageMessage } from "../../components/common/SiteChrome";
import { FavoritesPortal } from "../favorites/FavoritesPortal";

export const FavoritesRoute = () => {
  const { user, isLoading } = useAuth();

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