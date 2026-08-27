import { request } from "./apiClient";
import type { AdminUser } from "../types/AdminUser";
import type { ApiProperty } from "../types/ApiProperty";
import type { AuditLog } from "../types/AuditLog";
import type { Broker } from "../types/Broker";

export const adminApi = {
  getUsers: () =>
    request<AdminUser[]>("/api/users", {
      credentials: "include",
    }),
  updateUser: (
    userId: string,
    payload: { isActive: boolean; roles: string[] },
  ) =>
    request<AdminUser>(`/api/users/${userId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(payload),
    }),
  getBrokers: () =>
    request<Broker[]>("/api/brokers", {
      credentials: "include",
    }),
  updateBroker: (
    brokerId: string,
    payload: Omit<Broker, "id" | "userId">,
  ) =>
    request<Broker>(`/api/brokers/${brokerId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(payload),
    }),
  getProperties: () =>
    request<ApiProperty[]>("/api/properties/all", {
      credentials: "include",
    }),
  getAuditLogs: () =>
    request<AuditLog[]>("/api/audit-logs", {
      credentials: "include",
    }),
};
