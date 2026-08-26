import { useEffect, useState } from "react";
import { apiUrl, authApi } from "../../services/apiClient";
import { PageMessage } from "../../components/common/SiteChrome";
import { BrokerAccessMessage } from "../properties/BrokerPortal";
import type { AuthUser } from "../../types/api";

export const ProtectedRoute = () => {
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
    if (!isLoading && !user)
      window.location.assign(`${apiUrl}/auth/login?returnUrl=/private`);
  }, [isLoading, user]);
  if (isLoading)
    return (
      <PageMessage
        title="Comprobando sesión"
        message="Un momento, estamos verificando tu acceso."
      />
    );
  if (!user)
    return (
      <PageMessage
        title="Redirigiendo al login"
        message="Necesitas iniciar sesión para acceder a esta área."
      />
    );
  return <BrokerAccessMessage user={user} />;
};
