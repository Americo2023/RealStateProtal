import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useParams } from "react-router-dom";
import {
  Footer,
  PageMessage,
  SiteHeader,
} from "../../components/common/SiteChrome";
import { inquiriesApi, propertiesApi } from "../../services/apiClient";
import type { ApiProperty, ContactInquiryFormData } from "../../types/api";

export const PropertyDetail = () => {
  const { propertyId } = useParams();
  const [property, setProperty] = useState<ApiProperty | null>(null);
  const [isLoading, setIsLoading] = useState(true);
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
      .finally(() => setIsLoading(false));
  }, [propertyId]);

  const submitInquiry = async (data: ContactInquiryFormData) => {
    if (!propertyId) return;
    await inquiriesApi.create(propertyId, data);
    setSent(true);
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
        message="Esta propiedad no está disponible."
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
