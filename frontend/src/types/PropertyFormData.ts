export interface PropertyFormData {
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
}
