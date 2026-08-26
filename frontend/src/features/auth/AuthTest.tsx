import { useEffect, useState } from "react";
import { apiUrl, authApi } from "../../services/apiClient";
import { Footer, SiteHeader } from "../../components/common/SiteChrome";
import type { AuthUser } from "../../types/api";

export const AuthTest = () => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [publicResult, setPublicResult] = useState("Sin probar");
  const [protectedResult, setProtectedResult] = useState("Sin probar");
  const [error, setError] = useState<string | null>(null);
  const loadUser = async () => {
    try {
      setError(null);
      setUser(await authApi.getCurrentUser());
    } catch {
      setUser(null);
      setError("No se pudo conectar con la API.");
    }
  };
  const testEndpoint = async (
    endpoint: string,
    setResult: (result: string) => void,
  ) => {
    try {
      const response = await fetch(`${apiUrl}${endpoint}`, {
        credentials: "include",
      });
      setResult(`${response.status} ${response.statusText}`);
    } catch {
      setResult("Error de conexión");
    }
  };
  useEffect(() => {
    void Promise.resolve().then(loadUser);
  }, []);
  return (
    <div className="app-shell auth-test-page">
      <SiteHeader />
      <main>
        <section className="auth-test-intro">
          <p className="eyebrow">Fase 8 · Auth0</p>
          <h1>Prueba de autenticación</h1>
          <p className="intro-copy">
            Comprueba la sesión del portal y el acceso a sus endpoints.
          </p>
        </section>
        <section
          className="auth-actions"
          aria-label="Acciones de autenticación"
        >
          <a className="primary-action" href={`${apiUrl}/auth/login`}>
            Iniciar sesión
          </a>
          <a className="secondary-action" href={`${apiUrl}/auth/logout`}>
            Cerrar sesión
          </a>
          <button type="button" onClick={() => void loadUser()}>
            Actualizar sesión
          </button>
        </section>
        {error && <p className="error-message">{error}</p>}
        <section className="auth-grid">
          <div className="auth-panel">
            <div className="panel-heading">
              <p className="eyebrow">Identidad</p>
              <span
                className={
                  user?.isAuthenticated ? "status active-status" : "status"
                }
              >
                {user?.isAuthenticated ? "Autenticado" : "No autenticado"}
              </span>
            </div>
            <dl className="identity-list">
              <div>
                <dt>Nombre de usuario</dt>
                <dd>{user?.userName ?? "No disponible"}</dd>
              </div>
              <div>
                <dt>Email</dt>
                <dd>{user?.email ?? "No disponible"}</dd>
              </div>
              <div>
                <dt>Auth0 User Id</dt>
                <dd>{user?.auth0UserId ?? "No disponible"}</dd>
              </div>
              <div>
                <dt>Roles</dt>
                <dd>{user?.roles.join(", ") || "Ninguno"}</dd>
              </div>
            </dl>
          </div>
          <div className="auth-panel endpoint-panel">
            <p className="eyebrow">Endpoints</p>
            <div className="endpoint-row">
              <span>Catálogo público</span>
              <strong>{publicResult}</strong>
              <button
                type="button"
                onClick={() =>
                  void testEndpoint("/api/properties", setPublicResult)
                }
              >
                Probar
              </button>
            </div>
            <div className="endpoint-row">
              <span>Perfil protegido</span>
              <strong>{protectedResult}</strong>
              <button
                type="button"
                onClick={() =>
                  void testEndpoint("/api/auth/me", setProtectedResult)
                }
              >
                Probar
              </button>
            </div>
          </div>
        </section>
      </main>
      <Footer />
    </div>
  );
};
