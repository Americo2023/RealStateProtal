import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Footer, SiteHeader } from "../../components/common/SiteChrome";
import { favoritesApi, propertiesApi } from "../../services/apiClient";
import type { ApiProperty, PropertyCard } from "../../types/api";
import { PropertyMap } from "./PropertyMap";

type CatalogView = "list" | "map";

export const PropertyCatalog = () => {
  const [search, setSearch] = useState("");
  const [properties, setProperties] = useState<PropertyCard[]>([]);
  const [favoriteIds, setFavoriteIds] = useState<string[]>([]);
  const [view, setView] = useState<CatalogView>("list");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    void propertiesApi
      .getPublished(search.trim())
      .then((result: ApiProperty[]) =>
        setProperties(
          result.map((property) => ({
            id: property.id,
            title: property.title,
            location: property.address
              ? `${property.address.city}, ${property.address.region}`
              : "Ubicación no disponible",
            price: `${property.price.toLocaleString("es-ES")} ${property.currency}`,
            details: `${property.bedrooms} habitaciones · ${property.bathrooms} baños · ${property.livingArea} m2`,
            image:
              property.images.find((image) => image.isPrimary)?.url ??
              property.images[0]?.url ??
              "",
            status: property.status,
            latitude: property.address?.latitude,
            longitude: property.address?.longitude,
          })),
        ),
      )
      .catch(() => setError("No se pudo cargar el catálogo."))
      .finally(() => setIsLoading(false));
  }, [search]);

  useEffect(() => {
    void favoritesApi
      .getMine()
      .then((favorites) =>
        setFavoriteIds(favorites.map((favorite) => favorite.property.id)),
      )
      .catch(() => undefined);
  }, []);
  const toggleFavorite = async (propertyId: string) => {
    const isFavorite = favoriteIds.includes(propertyId);
    try {
      await favoritesApi.toggle(propertyId, isFavorite);
      setFavoriteIds((current) =>
        isFavorite
          ? current.filter((id) => id !== propertyId)
          : [...current, propertyId],
      );
    } catch {
      setError("No se pudo actualizar el favorito.");
    }
  };

  return (
    <div className="app-shell">
      <SiteHeader />
      <main>
        <section className="intro" id="about">
          <div>
            <p className="eyebrow">Encuentra tu proximo lugar</p>
            <h1>Espacios para vivir bien.</h1>
            <p className="intro-copy">
              Explora propiedades seleccionadas y encuentra un hogar que encaje
              contigo.
            </p>
          </div>
          <div className="intro-note">
            <strong>Catalogo abierto</strong>
            <span>Propiedades verificadas por nuestro equipo.</span>
          </div>
        </section>
        <section className="search-panel" aria-label="Buscar propiedades">
          <label htmlFor="property-search">Buscar por ciudad o zona</label>
          <div className="search-row">
            <input
              id="property-search"
              type="search"
              placeholder="Ej. Valencia, Chamberi..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
            <button type="button" onClick={() => setSearch("")}>
              Limpiar
            </button>
          </div>
        </section>
        <section className="property-section" id="properties">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Seleccion actual</p>
              <h2>Propiedades destacadas</h2>
            </div>
            <span className="result-count">{properties.length} resultados</span>
          </div>
          <div className="catalog-view-toggle" aria-label="Vista del catálogo">
            <button
              className={view === "list" ? "active" : ""}
              type="button"
              onClick={() => setView("list")}
            >
              Lista
            </button>
            <button
              className={view === "map" ? "active" : ""}
              type="button"
              onClick={() => setView("map")}
            >
              Mapa
            </button>
          </div>
          {view === "list" && <div className="property-grid">
            {properties.map((property) => (
              <article className="property-card" key={property.id}>
                {property.image && (
                  <img src={property.image} alt={property.title} />
                )}
                <div className="property-content">
                  <Link
                    className="property-link"
                    to={`/properties/${property.id}`}
                  >
                    <p className="property-location">{property.location}</p>
                    <h3>{property.title}</h3>
                  </Link>
                  <p className="property-details">{property.details}</p>
                  <div className="property-card-footer">
                    <strong className="property-price">{property.price}</strong>
                    <button
                      className="favorite-button"
                      type="button"
                      onClick={() => void toggleFavorite(property.id)}
                    >
                      {favoriteIds.includes(property.id)
                        ? "Quitar"
                        : "Favorito"}
                    </button>
                  </div>
                </div>
              </article>
            ))}
          </div>}
          {view === "map" && (
            <PropertyMap
              locations={properties.flatMap((property) =>
                property.latitude !== undefined && property.longitude !== undefined
                  ? [{ latitude: property.latitude, longitude: property.longitude, title: property.title }]
                  : [],
              )}
            />
          )}
          {isLoading && <p className="empty-state">Cargando catálogo...</p>}
          {error && <p className="error-message">{error}</p>}
          {!isLoading && !error && properties.length === 0 && (
            <p className="empty-state">
              No encontramos propiedades para esa búsqueda.
            </p>
          )}
        </section>
      </main>
      <Footer />
    </div>
  );
};
