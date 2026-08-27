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
