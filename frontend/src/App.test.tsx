import {
  describe,
  test,
  expect,
  vi,
  afterEach
} from "vitest";

import {
  render,
  screen,
  fireEvent,
  cleanup
} from "@testing-library/react";

import App from "./App";

import * as booksApi from "./api/books";
import type { Book } from "./types/book";

// Cleans up the DOM & Mocks after each test.
afterEach(() => {
  cleanup();
  vi.clearAllMocks();
});

vi.mock("./api/books");

// vi.mock() replaces the module's functions with mocks at runtime, but
// TypeScript still sees their original (real) types by default — it has no
// way to know at compile time that vi.mock swapped the implementation.
// vi.mocked() re-types an already-mocked import so methods like
// mockResolvedValueOnce are recognized.
const mockedGetBooks = vi.mocked(booksApi.getBooks);
const mockedCreateBook = vi.mocked(booksApi.createBook);

/**
 * Integration test for the whole App. Checks that when
 * a new book is created, it is rendered in the list.
 * This is important, because if the UI does not update after
 * creating a book, users won't see their new book in the list
 * and might think that the app is broken. We simulate user interactions
 * to create a new book and check that the new book appears in the UI.
 * Note: This test is more complex than unit tests, because it involves
 * multiple components and simulates user interactions. However, it
 * provides more confidence that the app works correctly from the
 * user's perspective.
 */
describe("App integration", () => {
  test("creates a new book and updates UI", async () => {
    // getBooks resolves the full Book shape, so the fixtures need every
    // field, unlike BookList's own tests which only need the fields it
    // actually renders.
    const booksBefore: Book[] = [
      {
        id: 1,
        isbn: null,
        title: "Clean Code",
        author: "Robert C. Martin",
        publicationYear: null,
        publisher: "",
        language: "",
        description: "",
        price: 30,
        condition: "Good"
      }
    ];

    const booksAfter: Book[] = [
      ...booksBefore,
      {
        id: 2,
        isbn: null,
        title: "Refactoring",
        author: "Martin Fowler",
        publicationYear: null,
        publisher: "",
        language: "",
        description: "",
        price: 40,
        condition: "Good"
      }
    ];

    mockedGetBooks
      // First getBooks() call returns initial page.
      .mockResolvedValueOnce({
        items: booksBefore,
        page: 1,
        pageSize: 10,
        totalCount: booksBefore.length
      })
      // Second getBooks() call returns updated page.
      .mockResolvedValueOnce({
        items: booksAfter,
        page: 1,
        pageSize: 10,
        totalCount: booksAfter.length
      });

    // The component never reads the resolved value, so an empty object
    // stands in here — cast because it doesn't need to satisfy the full
    // Book shape to make that true.
    mockedCreateBook.mockResolvedValue({} as Book);

    render(<App />);

    expect(
      screen.getByText("The first 10 books in the database")
    ).toBeInTheDocument();

    expect(
      // Wait for the first book to be rendered after initial fetch.
      await screen.findByText(
        "Clean Code — Robert C. Martin (€30)"
      )
    ).toBeInTheDocument();
    // Check that the second book is not rendered before creation.
    expect(
        screen.queryByText(
            "Refactoring — Martin Fowler (€40)"
        )
    ).not.toBeInTheDocument();


    fireEvent.change(
      screen.getByPlaceholderText("Title"),
      {
        target: { value: "Refactoring" }
      }
    );

    fireEvent.change(
      screen.getByPlaceholderText("Author"),
      {
        target: { value: "Martin Fowler" }
      }
    );

    fireEvent.change(
      screen.getByPlaceholderText("Price"),
      {
        target: { value: "40" }
      }
    );

    fireEvent.click(
      screen.getByText("Add Book")
    );

    // Verify that the new book to be rendered after creation.
    expect(
      await screen.findByText(
        "Refactoring — Martin Fowler (€40)"
      )
    ).toBeInTheDocument();
  });
});
