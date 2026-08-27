import { useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { useFavorites } from "../../app/FavoritesContext";
import { Footer, SiteHeader } from "../../components/common/SiteChrome";
import { propertiesApi } from "../../services/propertiesApi";
import type { ApiProperty } from "../../types/ApiProperty";
import type { PropertyCard } from "../../types/PropertyCard";
import type { PropertySearchCriteria } from "../../types/PropertySearchCriteria";
import { PropertyMap } from "./PropertyMap";

type CatalogView = "list" | "map";

export const PropertyCatalog = () => {
  const [search, setSearch] = useState("");
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [criteria, setCriteria] = useState<PropertySearchCriteria>({
    sort: "Newest",
  });
  const [properties, setProperties] = useState<PropertyCard[]>([]);
  const [view, setView] = useState<CatalogView>("list");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { favoriteIds, error: favoritesError, toggleFavorite } = useFavorites();

  useEffect(() => {
    void propertiesApi
      .search(criteria)
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
  }, [criteria]);

  const submitSearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setCriteria((current) => ({
      ...current,
      query: search.trim() || undefined,
    }));
  };

  const clearSearch = () => {
    setSearch("");
    setCriteria({ sort: "Newest" });
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
        <form className="search-panel" onSubmit={submitSearch}>
          <label htmlFor="property-search">Buscar propiedades</label>
          <div className="search-row">
            <input
              id="property-search"
              type="search"
              placeholder="Ej. Valencia, Chamberi..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
            <button type="submit">Buscar</button>
            <button type="button" onClick={clearSearch}>
              Limpiar
            </button>
          </div>
          <button
            className="advanced-toggle"
            type="button"
            onClick={() => setShowAdvanced((current) => !current)}
          >
            {showAdvanced ? "Ocultar filtros" : "Búsqueda avanzada"}
          </button>
          {showAdvanced && (
            <div className="advanced-search-grid">
              <label>
                Ciudad
                <input
                  value={criteria.city ?? ""}
                  onChange={(event) =>
                    setCriteria({ ...criteria, city: event.target.value })
                  }
                />
              </label>
              <label>
                Tipo
                <select
                  value={criteria.propertyType ?? ""}
                  onChange={(event) =>
                    setCriteria({
                      ...criteria,
                      propertyType: event.target.value
                        ? Number(event.target.value)
                        : undefined,
                    })
                  }
                >
                  <option value="">Todos</option>
                  <option value="0">Casa</option>
                  <option value="1">Apartamento</option>
                  <option value="2">Terreno</option>
                  <option value="3">Comercial</option>
                  <option value="4">Oficina</option>
                  <option value="5">Otro</option>
                </select>
              </label>
              {[
                ["priceMin", "Precio mínimo"],
                ["priceMax", "Precio máximo"],
                ["bedroomsMin", "Habitaciones mínimas"],
                ["bathroomsMin", "Baños mínimos"],
                ["areaMin", "Área mínima (m²)"],
                ["areaMax", "Área máxima (m²)"],
              ].map(([field, label]) => (
                <label key={field}>
                  {label}
                  <input
                    min="0"
                    type="number"
                    value={criteria[field as keyof PropertySearchCriteria] ?? ""}
                    onChange={(event) =>
                      setCriteria({
                        ...criteria,
                        [field]: event.target.value
                          ? Number(event.target.value)
                          : undefined,
                      })
                    }
                  />
                </label>
              ))}
              <label>
                Ordenar por
                <select
                  value={criteria.sort ?? "Newest"}
                  onChange={(event) =>
                    setCriteria({
                      ...criteria,
                      sort: event.target.value as PropertySearchCriteria["sort"],
                    })
                  }
                >
                  <option value="Newest">Más recientes</option>
                  <option value="Oldest">Más antiguas</option>
                  <option value="PriceLowToHigh">Precio menor</option>
                  <option value="PriceHighToLow">Precio mayor</option>
                </select>
              </label>
              <button type="submit">Aplicar filtros</button>
            </div>
          )}
        </form>
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
          {(error || favoritesError) && (
            <p className="error-message">{error ?? favoritesError}</p>
          )}
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
