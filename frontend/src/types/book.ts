// Mirrors the backend's BookCondition enum (WebPlatform.Api/Models/BookCondition.cs),
// which now serializes as its string name rather than its numeric value.
export type BookCondition =
  | "New"
  | "LikeNew"
  | "Excellent"
  | "Good"
  | "Fair"
  | "Poor";

// Mirrors the backend's BookResponse DTO (WebPlatform.Api/Dtos/BookResponse.cs).
export interface Book {
  id: number;
  isbn: string | null;
  title: string;
  author: string;
  publicationYear: number | null;
  publisher: string;
  language: string;
  description: string;
  price: number;
  condition: BookCondition;
}

// Mirrors the backend's generic PagedResult<T> wrapper (WebPlatform.Api/Dtos/PagedResult.cs).
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}
