import { request } from "./apiClient";
import type { ApiProperty } from "../types/ApiProperty";
import type { PropertySearchCriteria } from "../types/PropertySearchCriteria";

export const propertiesApi = {
  search: (criteria: PropertySearchCriteria) => {
    const params = new URLSearchParams();
    Object.entries(criteria).forEach(([key, value]) => {
      if (value !== undefined && value !== "") {
        params.set(key, String(value));
      }
    });
    const query = params.toString();
    return request<ApiProperty[]>(
      `/api/properties${query ? `?${query}` : ""}`,
    );
  },
  getPublished: (query?: string) =>
    request<ApiProperty[]>(
      query
        ? `/api/properties?query=${encodeURIComponent(query)}`
        : "/api/properties",
    ),
  getMine: () =>
    request<ApiProperty[]>("/api/properties/mine", {
      credentials: "include",
    }),
  getById: (propertyId: string) =>
    request<ApiProperty>(`/api/properties/${propertyId}`),
  save: (propertyId: string | null, payload: object) =>
    request<ApiProperty>(
      `/api/properties${propertyId ? `/${propertyId}` : ""}`,
      {
        method: propertyId ? "PUT" : "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(payload),
      },
    ),
  changeStatus: (
    propertyId: string,
    action: "publish" | "withdraw" | "sold" | "delete",
  ) =>
    request<unknown>(
      `/api/properties/${propertyId}${action === "delete" ? "" : `/${action}`}`,
      {
        method: action === "delete" ? "DELETE" : "POST",
        credentials: "include",
      },
    ),
  addImage: (propertyId: string, data: FormData) =>
    request<ApiProperty>(`/api/properties/${propertyId}/images`, {
      method: "POST",
      body: data,
      credentials: "include",
    }),
  removeImage: (propertyId: string, imageId: string) =>
    request<unknown>(`/api/properties/${propertyId}/images/${imageId}`, {
      method: "DELETE",
      credentials: "include",
    }),
  setPrimaryImage: (propertyId: string, imageId: string) =>
    request<unknown>(
      `/api/properties/${propertyId}/images/${imageId}/primary`,
      { method: "POST", credentials: "include" },
    ),
};
