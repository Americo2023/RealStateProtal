import { request } from "./apiClient";
import type { ContactInquiry } from "../types/ContactInquiry";
import type { ContactInquiryFormData } from "../types/ContactInquiryFormData";

export const inquiriesApi = {
  create: (propertyId: string, payload: ContactInquiryFormData) =>
    request<unknown>("/api/contact-inquiries", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ propertyId, ...payload }),
    }),
  getMine: () =>
    request<ContactInquiry[]>("/api/contact-inquiries/mine", {
      credentials: "include",
    }),
};
