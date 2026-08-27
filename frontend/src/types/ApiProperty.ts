import type { PropertyImage } from "./PropertyImage";

export interface ApiProperty {
  id: string;
  referenceNumber: string;
  title: string;
  description: string;
  propertyType: number;
  price: number;
  currency: string;
  bedrooms: number;
  bathrooms: number;
  rooms: number;
  livingArea: number;
  totalArea: number;
  floor: number | null;
  numberOfFloors: number | null;
  constructionYear: number | null;
  energyClass: number;
  status: string;
  address: {
    street: string;
    streetNumber: string;
    postalCode: string;
    city: string;
    region: string;
    country: string;
    latitude: number;
    longitude: number;
  } | null;
  images: PropertyImage[];
}
