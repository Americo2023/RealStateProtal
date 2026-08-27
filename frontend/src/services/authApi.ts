import { request } from "./apiClient";
import type { AuthUser } from "../types/AuthUser";

export const authApi = {
  getCurrentUser: () =>
    request<AuthUser>("/api/auth/me", {
      credentials: "include",
    }),
};
