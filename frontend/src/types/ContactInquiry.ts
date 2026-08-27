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
