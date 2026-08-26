import { useEffect, useState } from "react";
import { PropertyForm } from "../../components/forms/PropertyForm";
import {
  Footer,
  PageMessage,
  SiteHeader,
} from "../../components/common/SiteChrome";
import { inquiriesApi, propertiesApi } from "../../services/apiClient";
import type { ApiProperty, AuthUser, ContactInquiry } from "../../types/api";

export const BrokerPortal = () => {
  const [properties, setProperties] = useState<ApiProperty[]>([]);
  const [inquiries, setInquiries] = useState<ContactInquiry[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editingProperty, setEditingProperty] = useState<ApiProperty | null>(
    null,
  );
  const [isCreating, setIsCreating] = useState(false);

  const refreshProperties = async () =>
    setProperties(await propertiesApi.getMine());
  useEffect(() => {
    void Promise.resolve()
      .then(refreshProperties)
      .catch(() => setError("No se pudieron cargar tus propiedades."))
      .finally(() => setIsLoading(false));
  }, []);
  useEffect(() => {
    void inquiriesApi
      .getMine()
      .then(setInquiries)
      .catch(() => setInquiries([]));
  }, []);
  useEffect(() => {
    if (!isCreating && !editingProperty) return;
    const closeOnEscape = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        setIsCreating(false);
        setEditingProperty(null);
      }
    };
    document.addEventListener("keydown", closeOnEscape);
    return () => document.removeEventListener("keydown", closeOnEscape);
  }, [editingProperty, isCreating]);

  const changeStatus = async (
    propertyId: string,
    action: "publish" | "withdraw" | "sold" | "delete",
  ) => {
    try {
      await propertiesApi.changeStatus(propertyId, action);
      await refreshProperties();
    } catch {
      setError("No se pudo actualizar el estado de la propiedad.");
    }
  };
  const closeEditor = () => {
    setIsCreating(false);
    setEditingProperty(null);
  };
  const published = properties.filter(
    (property) => property.status === "Published" || property.status === "1",
  ).length;
  const drafts = properties.filter(
    (property) => property.status === "Draft" || property.status === "0",
  ).length;

  return (
    <div className="app-shell">
      <SiteHeader />
      <main className="area-page">
        <section className="area-heading">
          <div>
            <p className="eyebrow">Área privada · Broker</p>
            <h1>Tu espacio en el portal.</h1>
            <p className="intro-copy">
              Gestiona tus propiedades y revisa su estado de publicación.
            </p>
          </div>
          <button
            type="button"
            onClick={() => {
              setIsCreating(true);
              setEditingProperty(null);
            }}
          >
            Nueva propiedad
          </button>
        </section>
        <section className="metric-grid" aria-label="Resumen de propiedades">
          <div className="metric-card">
            <span>Total</span>
            <strong>{properties.length}</strong>
          </div>
          <div className="metric-card">
            <span>Publicadas</span>
            <strong>{published}</strong>
          </div>
          <div className="metric-card">
            <span>Borradores</span>
            <strong>{drafts}</strong>
          </div>
        </section>
        {error && <p className="error-message">{error}</p>}
        {isLoading && (
          <p className="empty-state">Cargando tus propiedades...</p>
        )}
        {!isLoading && !error && properties.length === 0 && (
          <p className="empty-state">
            Todavía no tienes propiedades asignadas.
          </p>
        )}
        {!isLoading && properties.length > 0 && (
          <section className="managed-properties">
            <div className="section-heading">
              <div>
                <p className="eyebrow">Inventario</p>
                <h2>Tus propiedades</h2>
              </div>
            </div>
            <div className="managed-list">
              {properties.map((property) => (
                <article className="managed-row" key={property.id}>
                  <div>
                    <strong>{property.title}</strong>
                    <span>
                      {property.bedrooms} habitaciones · {property.bathrooms}{" "}
                      baños
                    </span>
                  </div>
                  <span className="status">{property.status}</span>
                  <div className="managed-actions">
                    <button
                      type="button"
                      onClick={() => {
                        setEditingProperty(property);
                        setIsCreating(false);
                      }}
                    >
                      Gestionar
                    </button>
                    {(property.status === "Draft" ||
                      property.status === "0") && (
                      <button
                        type="button"
                        onClick={() =>
                          void changeStatus(property.id, "publish")
                        }
                      >
                        Publicar
                      </button>
                    )}
                    {(property.status === "Published" ||
                      property.status === "1") && (
                      <button
                        type="button"
                        onClick={() =>
                          void changeStatus(property.id, "withdraw")
                        }
                      >
                        Retirar
                      </button>
                    )}
                    {(property.status === "Published" ||
                      property.status === "1") && (
                      <button
                        type="button"
                        onClick={() => void changeStatus(property.id, "sold")}
                      >
                        Vender
                      </button>
                    )}
                    {(property.status === "Sold" ||
                      property.status === "2") && (
                      <button
                        type="button"
                        onClick={() => void changeStatus(property.id, "delete")}
                      >
                        Eliminar
                      </button>
                    )}
                  </div>
                </article>
              ))}
            </div>
          </section>
        )}
        <section className="inquiries-section">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Contacto</p>
              <h2>Consultas recibidas</h2>
            </div>
            <span className="result-count">{inquiries.length} consultas</span>
          </div>
          {inquiries.length === 0 ? (
            <p className="empty-state">Todavía no has recibido consultas.</p>
          ) : (
            <div className="inquiry-list">
              {inquiries.map((inquiry) => (
                <article className="inquiry-row" key={inquiry.id}>
                  <div>
                    <strong>{inquiry.visitorName}</strong>
                    <span>
                      {inquiry.propertyTitle} ·{" "}
                      {inquiry.propertyReferenceNumber}
                    </span>
                  </div>
                  <p>{inquiry.message}</p>
                  <a href={`mailto:${inquiry.visitorEmail}`}>
                    {inquiry.visitorEmail}
                  </a>
                  <time dateTime={inquiry.createdAt}>
                    {new Date(inquiry.createdAt).toLocaleDateString("es-ES")}
                  </time>
                </article>
              ))}
            </div>
          )}
        </section>
      </main>
      {(isCreating || editingProperty) && (
        <div
          className="modal-backdrop"
          role="presentation"
          onMouseDown={closeEditor}
        >
          <div
            className="property-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="property-modal-title"
            onMouseDown={(event) => event.stopPropagation()}
          >
            <button
              className="modal-close"
              type="button"
              aria-label="Cerrar formulario"
              onClick={closeEditor}
            >
              ×
            </button>
            <div id="property-modal-title">
              <PropertyForm
                property={editingProperty}
                onCancel={closeEditor}
                onSaved={async () => {
                  closeEditor();
                  await refreshProperties();
                }}
              />
            </div>
          </div>
        </div>
      )}
      <Footer />
    </div>
  );
};

export const BrokerAccessMessage = ({ user }: { user: AuthUser }) => {
  const canManageProperties = user.roles.some((role) =>
    ["Broker", "Administrator"].includes(role),
  );
  return canManageProperties ? (
    <BrokerPortal />
  ) : (
    <PageMessage
      title="Acceso restringido"
      message="Esta área está reservada para brokers y administradores."
    />
  );
};
