import { useState } from "react";
import { useForm } from "react-hook-form";
import { propertiesApi } from "../../services/apiClient";
import type { ApiProperty, PropertyFormData } from "../../types/api";

const emptyPropertyForm: PropertyFormData = {
  referenceNumber: "",
  title: "",
  description: "",
  propertyType: 0,
  price: 0,
  currency: "EUR",
  bedrooms: 0,
  bathrooms: 0,
  rooms: 0,
  livingArea: 0,
  totalArea: 0,
  floor: null,
  numberOfFloors: null,
  constructionYear: null,
  energyClass: 8,
};
const propertyTypes = [
  "Casa",
  "Apartamento",
  "Terreno",
  "Comercial",
  "Oficina",
  "Otro",
];
const energyClasses = [
  "A+",
  "A",
  "B",
  "C",
  "D",
  "E",
  "F",
  "G",
  "No especificada",
];

type PropertyFormProps = {
  property: ApiProperty | null;
  onCancel: () => void;
  onSaved: () => Promise<void>;
};

export const PropertyForm = ({
  property,
  onCancel,
  onSaved,
}: PropertyFormProps) => {
  const [currentProperty, setCurrentProperty] = useState<ApiProperty | null>(
    property,
  );
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imageAltText, setImageAltText] = useState("");
  const [imageError, setImageError] = useState<string | null>(null);
  const initialValues: PropertyFormData = property
    ? {
        referenceNumber: property.referenceNumber,
        title: property.title,
        description: property.description,
        propertyType: property.propertyType,
        price: property.price,
        currency: property.currency,
        bedrooms: property.bedrooms,
        bathrooms: property.bathrooms,
        rooms: property.rooms,
        livingArea: property.livingArea,
        totalArea: property.totalArea,
        floor: property.floor,
        numberOfFloors: property.numberOfFloors,
        constructionYear: property.constructionYear,
        energyClass: property.energyClass,
      }
    : emptyPropertyForm;
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<PropertyFormData>({ defaultValues: initialValues });

  const save = async (form: PropertyFormData) => {
    setError(null);
    setIsSaving(true);
    try {
      await propertiesApi.save(
        property?.id ?? null,
        property ? { ...form, referenceNumber: undefined } : form,
      );
      await onSaved();
    } catch (saveError) {
      setError(
        saveError instanceof Error
          ? saveError.message
          : "No se pudo guardar la propiedad.",
      );
    } finally {
      setIsSaving(false);
    }
  };

  const uploadImage = async () => {
    if (!currentProperty || !imageFile) return;
    setImageError(null);
    const data = new FormData();
    data.append("file", imageFile);
    data.append("altText", imageAltText || currentProperty.title);
    data.append("isPrimary", String(currentProperty.images.length === 0));
    try {
      setCurrentProperty(
        await propertiesApi.addImage(currentProperty.id, data),
      );
      setImageFile(null);
      setImageAltText("");
    } catch {
      setImageError("No se pudo subir la imagen.");
    }
  };
  const removeImage = async (imageId: string) => {
    if (!currentProperty) return;
    try {
      await propertiesApi.removeImage(currentProperty.id, imageId);
      setCurrentProperty({
        ...currentProperty,
        images: currentProperty.images.filter((image) => image.id !== imageId),
      });
    } catch {
      setImageError("No se pudo eliminar la imagen.");
    }
  };
  const setPrimaryImage = async (imageId: string) => {
    if (!currentProperty) return;
    try {
      await propertiesApi.setPrimaryImage(currentProperty.id, imageId);
      setCurrentProperty({
        ...currentProperty,
        images: currentProperty.images.map((image) => ({
          ...image,
          isPrimary: image.id === imageId,
        })),
      });
    } catch {
      setImageError("No se pudo actualizar la imagen principal.");
    }
  };

  return (
    <section className="property-form-panel">
      <div className="section-heading">
        <div>
          <p className="eyebrow">
            {property ? "Editar propiedad" : "Nueva propiedad"}
          </p>
          <h2>{property ? property.title : "Datos de la propiedad"}</h2>
        </div>
      </div>
      <form
        className="property-form"
        onSubmit={(event) => void handleSubmit(save)(event)}
      >
        {!property && (
          <label>
            Referencia
            <input {...register("referenceNumber", { required: true })} />
          </label>
        )}
        <label>
          Título
          <input {...register("title", { required: true })} />
        </label>
        <label>
          Descripción
          <textarea {...register("description", { required: true })} />
        </label>
        <label>
          Tipo
          <select {...register("propertyType", { valueAsNumber: true })}>
            {propertyTypes.map((type, index) => (
              <option value={index} key={type}>
                {type}
              </option>
            ))}
          </select>
        </label>
        <label>
          Precio
          <input
            min="0.01"
            type="number"
            {...register("price", {
              required: true,
              valueAsNumber: true,
              min: 0.01,
            })}
          />
        </label>
        <label>
          Moneda
          <input
            maxLength={3}
            {...register("currency", { required: true, maxLength: 3 })}
          />
        </label>
        <label>
          Habitaciones
          <input
            min="0"
            type="number"
            {...register("bedrooms", { valueAsNumber: true, min: 0 })}
          />
        </label>
        <label>
          Baños
          <input
            min="0"
            type="number"
            {...register("bathrooms", { valueAsNumber: true, min: 0 })}
          />
        </label>
        <label>
          Estancias
          <input
            min="0"
            type="number"
            {...register("rooms", { valueAsNumber: true, min: 0 })}
          />
        </label>
        <label>
          Área habitable
          <input
            min="0.01"
            type="number"
            {...register("livingArea", {
              required: true,
              valueAsNumber: true,
              min: 0.01,
            })}
          />
        </label>
        <label>
          Área total
          <input
            min="0.01"
            type="number"
            {...register("totalArea", {
              required: true,
              valueAsNumber: true,
              min: 0.01,
            })}
          />
        </label>
        <label>
          Planta
          <input
            min="0"
            type="number"
            {...register("floor", { valueAsNumber: true })}
          />
        </label>
        <label>
          Plantas
          <input
            min="0"
            type="number"
            {...register("numberOfFloors", { valueAsNumber: true })}
          />
        </label>
        <label>
          Año de construcción
          <input
            min="1800"
            type="number"
            {...register("constructionYear", { valueAsNumber: true })}
          />
        </label>
        <label>
          Calificación energética
          <select {...register("energyClass", { valueAsNumber: true })}>
            {energyClasses.map((item, index) => (
              <option value={index} key={item}>
                {item}
              </option>
            ))}
          </select>
        </label>
        {Object.keys(errors).length > 0 && (
          <p className="error-message">
            Completa los campos obligatorios correctamente.
          </p>
        )}
        {error && <p className="error-message">{error}</p>}
        <div className="form-actions">
          <button type="button" onClick={onCancel}>
            Cancelar
          </button>
          <button type="submit" disabled={isSaving}>
            {isSaving ? "Guardando..." : "Guardar propiedad"}
          </button>
        </div>
      </form>
      {currentProperty && (
        <div className="image-manager">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Galería</p>
              <h3>Imágenes de la propiedad</h3>
            </div>
          </div>
          <div className="image-upload">
            <input
              type="file"
              accept="image/*"
              onChange={(event) =>
                setImageFile(event.target.files?.[0] ?? null)
              }
            />
            <input
              placeholder="Texto alternativo"
              value={imageAltText}
              onChange={(event) => setImageAltText(event.target.value)}
            />
            <button
              type="button"
              disabled={!imageFile}
              onClick={() => void uploadImage()}
            >
              Subir imagen
            </button>
          </div>
          {imageError && <p className="error-message">{imageError}</p>}
          <div className="managed-images">
            {currentProperty.images.map((image) => (
              <article key={image.id}>
                <img src={image.url} alt={image.altText} />
                <div>
                  <span>{image.isPrimary ? "Principal" : "Secundaria"}</span>
                  <button
                    type="button"
                    onClick={() => void setPrimaryImage(image.id)}
                    disabled={image.isPrimary}
                  >
                    Principal
                  </button>
                  <button
                    type="button"
                    onClick={() => void removeImage(image.id)}
                  >
                    Eliminar
                  </button>
                </div>
              </article>
            ))}
          </div>
        </div>
      )}
    </section>
  );
};
