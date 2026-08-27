import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useParams } from "react-router-dom";
import {
  Footer,
  PageMessage,
  SiteHeader,
} from "../../components/common/SiteChrome";
import { inquiriesApi } from "../../services/inquiriesApi";
import { propertiesApi } from "../../services/propertiesApi";
import type { ApiProperty } from "../../types/ApiProperty";
import type { ContactInquiryFormData } from "../../types/ContactInquiryFormData";
import { PropertyMap } from "./PropertyMap";

export const PropertyDetail = () => {
  const { propertyId } = useParams();
  const [property, setProperty] = useState<ApiProperty | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [loadError, setLoadError] = useState(false);
  const [submitError, setSubmitError] = useState(false);
  const [sent, setSent] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ContactInquiryFormData>();

  useEffect(() => {
    if (!propertyId) return;
    void propertiesApi
      .getById(propertyId)
      .then(setProperty)
      .catch(() => setLoadError(true))
      .finally(() => setIsLoading(false));
  }, [propertyId]);

  const submitInquiry = async (data: ContactInquiryFormData) => {
    if (!propertyId) return;
    try {
      setSubmitError(false);
      await inquiriesApi.create(propertyId, data);
      setSent(true);
    } catch {
      setSubmitError(true);
    }
  };

  if (isLoading)
    return (
      <PageMessage
        title="Cargando propiedad"
        message="Estamos preparando los detalles."
      />
    );
  if (!property)
    return (
      <PageMessage
        title="Propiedad no encontrada"
        message={
          loadError
            ? "No se pudieron cargar los detalles de esta propiedad."
            : "Esta propiedad no está disponible."
        }
      />
    );
  const image =
    property.images.find((candidate) => candidate.isPrimary)?.url ??
    property.images[0]?.url;

  return (
    <div className="app-shell">
      <SiteHeader />
      <main className="detail-page">
        <Link className="back-link" to="/public">
          Volver al catálogo
        </Link>
        {image && (
          <img
            className="detail-image"
            src={image}
            alt={property.images[0]?.altText ?? property.title}
          />
        )}
        <div className="detail-layout">
          <article>
            <p className="eyebrow">
              {property.address?.city ?? "Ubicación no disponible"}
            </p>
            <h1>{property.title}</h1>
            <p className="detail-price">
              {property.price.toLocaleString("es-ES")} {property.currency}
            </p>
            <p className="intro-copy">{property.description}</p>
            <p className="detail-specs">
              {property.bedrooms} habitaciones · {property.bathrooms} baños ·{" "}
              {property.livingArea} m2
            </p>
            {property.address && (
              <address className="detail-address">
                {property.address.street} {property.address.streetNumber},{" "}
                {property.address.postalCode} {property.address.city},{" "}
                {property.address.region}
              </address>
            )}
            {property.address && (
              <section className="map-section" aria-label="Ubicación de la propiedad">
                <p className="eyebrow">Ubicación</p>
                <PropertyMap
                  latitude={property.address.latitude}
                  longitude={property.address.longitude}
                  title={property.title}
                />
              </section>
            )}
          </article>
          <section className="contact-panel">
            <p className="eyebrow">Contacto</p>
            {sent ? (
              <p className="success-message">Tu consulta ha sido enviada.</p>
            ) : (
              <form
                onSubmit={(event) => void handleSubmit(submitInquiry)(event)}
              >
                <input
                  placeholder="Nombre"
                  {...register("visitorName", { required: true })}
                />
                <input
                  type="email"
                  placeholder="Email"
                  {...register("visitorEmail", { required: true })}
                />
                <input placeholder="Teléfono" {...register("visitorPhone")} />
                <textarea
                  placeholder="Mensaje"
                  {...register("message", { required: true })}
                />
                {(errors.visitorName ||
                  errors.visitorEmail ||
                  errors.message) && (
                  <p className="error-message">
                    Completa los campos obligatorios.
                  </p>
                )}
                {submitError && (
                  <p className="error-message">
                    No se pudo enviar la consulta. Inténtalo de nuevo.
                  </p>
                )}
                <button type="submit">Contactar al broker</button>
              </form>
            )}
          </section>
        </div>
      </main>
      <Footer />
    </div>
  );
};
