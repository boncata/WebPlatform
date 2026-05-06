import axios from "axios";

// Constant for the backend URL
const API_URL = "http://localhost:5130/api/books";

/**
 * Function for the GET request. "export" allows for code outside
 * of this file to access this function. The function is assigned
 * to a constant variable getBooks, because functions are variables.
 * @returns The book data from the request response.
 */
export const getBooks = async () => {
  const response = await axios.get(API_URL);
  return response.data;
};

/**
 * Function for the POST request.
 * @param {*} book The book data from the user.
 * @returns The saved book data from the request response.
 */
export const createBook = async (book) => {
  const response = await axios.post(API_URL, book);
  return response.data;
};