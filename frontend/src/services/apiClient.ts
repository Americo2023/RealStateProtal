export const apiUrl = import.meta.env.VITE_API_URL ?? "http://localhost:5080";

const request = async <T>(path: string, options?: RequestInit): Promise<T> => {
  const response = await fetch(`${apiUrl}${path}`, options);
  if (!response.ok) {
    throw new Error(`La API respondió con ${response.status}.`);
  }
  return (await response.json()) as T;
};

export const authApi = {
  getCurrentUser: () =>
    request<import("../types/api").AuthUser>("/api/auth/me", {
      credentials: "include",
    }),
};

export const propertiesApi = {
  getPublished: (query?: string) =>
    request<import("../types/api").ApiProperty[]>(
      query
        ? `/api/properties?query=${encodeURIComponent(query)}`
        : "/api/properties",
    ),
  getMine: () =>
    request<import("../types/api").ApiProperty[]>("/api/properties/mine", {
      credentials: "include",
    }),
  getById: (propertyId: string) =>
    request<import("../types/api").ApiProperty>(
      `/api/properties/${propertyId}`,
    ),
  save: (propertyId: string | null, payload: object) =>
    request<import("../types/api").ApiProperty>(
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
    request<import("../types/api").ApiProperty>(
      `/api/properties/${propertyId}/images`,
      { method: "POST", body: data, credentials: "include" },
    ),
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

export const inquiriesApi = {
  create: (propertyId: string, payload: object) =>
    request<unknown>("/api/contact-inquiries", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ propertyId, ...payload }),
    }),
  getMine: () =>
    request<import("../types/api").ContactInquiry[]>(
      "/api/contact-inquiries/mine",
      { credentials: "include" },
    ),
};

export const favoritesApi = {
  getMine: () =>
    request<Array<{ property: { id: string } }>>("/api/favorites", {
      credentials: "include",
    }),
  toggle: (propertyId: string, isFavorite: boolean) =>
    request<unknown>(`/api/favorites/${propertyId}`, {
      method: isFavorite ? "DELETE" : "POST",
      credentials: "include",
    }),
};

export const adminApi = {
  getUsers: () =>
    request<import("../types/api").AdminUser[]>("/api/users", {
      credentials: "include",
    }),
  updateUser: (userId: string, payload: { isActive: boolean; roles: string[] }) =>
    request<import("../types/api").AdminUser>(`/api/users/${userId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(payload),
    }),
  getBrokers: () =>
    request<import("../types/api").Broker[]>("/api/brokers", {
      credentials: "include",
    }),
  updateBroker: (brokerId: string, payload: Omit<import("../types/api").Broker, "id" | "userId">) =>
    request<import("../types/api").Broker>(`/api/brokers/${brokerId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(payload),
    }),
  getProperties: () =>
    request<import("../types/api").ApiProperty[]>("/api/properties/all", {
      credentials: "include",
    }),
  getAuditLogs: () =>
    request<import("../types/api").AuditLog[]>("/api/audit-logs", {
      credentials: "include",
    }),
};
