export interface AuthUser {
  isAuthenticated: boolean;
  userName: string | null;
  email: string | null;
  auth0UserId: string | null;
  roles: string[];
}
