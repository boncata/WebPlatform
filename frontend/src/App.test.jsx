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
  waitFor,
  cleanup
} from "@testing-library/react";

import App from "./App";

import * as booksApi from "./api/books";

// Cleans up the DOM & Mocks after each test.
afterEach(() => {
  cleanup();
  vi.clearAllMocks();
});

vi.mock("./api/books");

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
    const booksBefore = [
      {
        id: 1,
        title: "Clean Code",
        author: "Robert C. Martin",
        price: 30
      }
    ];

    const booksAfter = [
      ...booksBefore,
      {
        id: 2,
        title: "Refactoring",
        author: "Martin Fowler",
        price: 40
      }
    ];

    booksApi.getBooks
      .mockResolvedValueOnce(booksBefore) // First getBooks() call returns initial list
      .mockResolvedValueOnce(booksAfter); // Second getBooks() call returns updated list

    booksApi.createBook
      .mockResolvedValue({});

    render(<App />);

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