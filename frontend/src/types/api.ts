export interface PropertyCard {
  id: string;
  title: string;
  location: string;
  price: string;
  details: string;
  image: string;
  status?: string;
  latitude?: number;
  longitude?: number;
}

export interface PropertySearchCriteria {
  query?: string;
  propertyType?: number;
  city?: string;
  priceMin?: number;
  priceMax?: number;
  bedroomsMin?: number;
  bathroomsMin?: number;
  areaMin?: number;
  areaMax?: number;
  sort?: "Newest" | "Oldest" | "PriceLowToHigh" | "PriceHighToLow";
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

export interface AuthUser {
  isAuthenticated: boolean;
  userName: string | null;
  email: string | null;
  auth0UserId: string | null;
  roles: string[];
}

export interface AdminUser {
  id: string
  auth0UserId: string
  email: string
  firstName: string
  lastName: string
  isActive: boolean
  roles: string[]
}

export interface Broker {
  id: string
  userId: string
  fullName: string
  email: string
  phone: string
  bio: string
  isActive: boolean
}

export interface AuditLog {
  id: string
  entityName: string
  entityId: string
  action: string
  changedByUserId: string | null
  changedAt: string
  details: string
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

export interface Favorite {
  id: string;
  createdAt: string;
  property: ApiProperty;
}
