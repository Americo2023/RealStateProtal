import { useEffect } from "react";
import { apiUrl } from "../../services/apiClient";
import { useAuth } from "../../app/AuthContext";
import { PageMessage } from "../../components/common/SiteChrome";
import { BrokerAccessMessage } from "../properties/BrokerPortal";

export const ProtectedRoute = () => {
  const { user, isLoading } = useAuth();
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
