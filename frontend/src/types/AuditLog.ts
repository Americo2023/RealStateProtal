export interface AuditLog {
  id: string;
  entityName: string;
  entityId: string;
  action: string;
  changedByUserId: string | null;
  changedAt: string;
  details: string;
}
