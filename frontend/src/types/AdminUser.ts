export interface AdminUser {
  id: string;
  auth0UserId: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
  roles: string[];
}
