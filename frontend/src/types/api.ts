export interface PropertyCard {
  id: string;
  title: string;
  location: string;
  price: string;
  details: string;
  image: string;
  status?: string;
}

export interface PropertyImage {
  id: string;
  url: string;
  altText: string;
  isPrimary: boolean;
}

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
  address: { city: string; region: string } | null;
  images: PropertyImage[];
}

export interface AuthUser {
  isAuthenticated: boolean;
  userName: string | null;
  email: string | null;
  auth0UserId: string | null;
  roles: string[];
}

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

export interface ContactInquiry {
  id: string;
  propertyTitle: string;
  propertyReferenceNumber: string;
  visitorName: string;
  visitorEmail: string;
  visitorPhone: string | null;
  message: string;
  createdAt: string;
}

export interface ContactInquiryFormData {
  visitorName: string;
  visitorEmail: string;
  visitorPhone: string;
  message: string;
}
