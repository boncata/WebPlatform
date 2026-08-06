import axios from "axios";

import type { Book, PagedResult } from "../types/book";

// Constant for the backend URL
const API_URL = "http://localhost:5130/api/books";

// The fields BookForm currently collects and sends when creating a book.
// Once the form is reworked (Phase 3), this will likely be replaced by a
// type inferred from that form's Zod schema instead.
export interface NewBookPayload {
  title: string;
  author: string;
  price: number;
}

/**
 * Function for the GET request. "export" allows for code outside
 * of this file to access this function. The function is assigned
 * to a constant variable getBooks, because functions are variables.
 * Default to the first 10 books from the database, as the
 * backend supports pagination.
 * @returns The book data from the request response.
 */
export const getBooks = async (
  page: number = 1,
  pageSize: number = 10
): Promise<PagedResult<Book>> => {
  const response = await axios.get<PagedResult<Book>>(API_URL, {
    params: { page, pageSize }
  });
  return response.data;
};

/**
 * Function for the POST request.
 * @param book The book data from the user.
 * @returns The saved book data from the request response.
 */
export const createBook = async (book: NewBookPayload): Promise<Book> => {
  const response = await axios.post<Book>(API_URL, book);
  return response.data;
};
