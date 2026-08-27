import { useEffect, useRef } from "react";
import L from "leaflet";
import "leaflet/dist/leaflet.css";

type PropertyMapProps = {
  latitude?: number;
  longitude?: number;
  title?: string;
  locations?: Array<{
    latitude: number;
    longitude: number;
    title: string;
  }>;
};

export const PropertyMap = ({
  latitude,
  longitude,
  title,
  locations = [],
}: PropertyMapProps) => {
  const mapElement = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!mapElement.current) {
      return () => undefined;
    }

    const points = locations.length
      ? locations
      : latitude !== undefined && longitude !== undefined && title
        ? [{ latitude, longitude, title }]
        : [];
    if (points.length === 0) {
      return () => undefined;
    }

    const map = L.map(mapElement.current, {
      scrollWheelZoom: false,
    }).setView([points[0].latitude, points[0].longitude], 14);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      attribution: "&copy; OpenStreetMap contributors",
    }).addTo(map);
    const markers = points.map((point) =>
      L.marker([point.latitude, point.longitude])
        .addTo(map)
        .bindPopup(point.title),
    );
    if (markers.length === 1) {
      markers[0].openPopup();
    } else {
      map.fitBounds(
        L.latLngBounds(points.map((point) => [point.latitude, point.longitude])),
        { padding: [24, 24] },
      );
    }

    return () => {
      map.remove();
    };
  }, [latitude, longitude, title, locations]);

  return (
    <div
      className="property-map"
      ref={mapElement}
      aria-label={title ? `Ubicación de ${title}` : "Mapa de propiedades"}
    />
  );
};
