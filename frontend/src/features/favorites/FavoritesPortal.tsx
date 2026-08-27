import { Link } from "react-router-dom";
import { useFavorites } from "../../app/FavoritesContext";
import { Footer, PageMessage, SiteHeader } from "../../components/common/SiteChrome";
import type { AuthUser } from "../../types/AuthUser";

type FavoritesPortalProps = {
  user: AuthUser;
};

export const FavoritesPortal = ({ user }: FavoritesPortalProps) => {
  const { favorites, isLoading, error, removeFavorite } = useFavorites();

  if (!user.roles.some((role) =>
    ["Registered User", "RegisteredUser", "Broker", "Administrator"].includes(role),
  )) {
    return (
      <PageMessage
        title="Favoritos no disponibles"
        message="Necesitas una cuenta registrada para guardar propiedades."
      />
    );
  }

  return (
    <div className="app-shell">
      <SiteHeader />
      <main className="favorites-page">
        <section className="area-heading">
          <div>
            <p className="eyebrow">Tu selección</p>
            <h1>Mis favoritos.</h1>
            <p className="intro-copy">
              Guarda las propiedades que quieres volver a visitar.
            </p>
          </div>
          <span className="result-count">{favorites.length} guardados</span>
        </section>
        {isLoading && <p className="empty-state">Cargando favoritos...</p>}
        {error && <p className="error-message">{error}</p>}
        {!isLoading && !error && favorites.length === 0 && (
          <p className="empty-state">
            Todavía no tienes propiedades favoritas. <Link to="/public">Explorar catálogo</Link>
          </p>
        )}
        <div className="favorite-list">
          {favorites.map((favorite) => {
            const property = favorite.property;
            const image =
              property.images.find((candidate) => candidate.isPrimary)?.url ??
              property.images[0]?.url;
            const isRetired = property.status !== "Published";

            return (
              <article className="favorite-item" key={favorite.id}>
                {image && <img src={image} alt={property.title} />}
                <div className="favorite-item-content">
                  <p className="property-location">
                    {property.address?.city ?? "Ubicación no disponible"}
                  </p>
                  <h2>{property.title}</h2>
                  <p className="property-details">
                    {property.price.toLocaleString("es-ES")} {property.currency} · {property.bedrooms} habitaciones
                  </p>
                  {isRetired && (
                    <p className="retired-label">
                      Propiedad retirada: {property.status}
                    </p>
                  )}
                  <div className="favorite-item-actions">
                    <Link to={`/properties/${property.id}`}>Ver detalle</Link>
                    <button type="button" onClick={() => void removeFavorite(property.id)}>
                      Quitar favorito
                    </button>
                  </div>
                </div>
              </article>
            );
          })}
        </div>
      </main>
      <Footer />
    </div>
  );
};