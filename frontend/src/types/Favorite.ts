import type { ApiProperty } from "./ApiProperty";

export interface Favorite {
  id: string;
  createdAt: string;
  property: ApiProperty;
}
